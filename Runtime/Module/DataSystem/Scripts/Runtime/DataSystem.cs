using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public enum DataEncryptionType
{
    Json = 0,
    Base64 = 1,
    Xor = 2,
    Aes = 3
}

public static class DataSystem
{
    private const string EXTENSION = ".json";
    private const string BACKUP_EXTENSION = ".bak";

    private static string BasePath => Application.persistentDataPath;

    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };

    #region PUBLIC API

    public static void Save<T>(
        string key,
        T data,
        DataEncryptionType encryption = DataEncryptionType.Json)
    {
        try
        {
            string path = GetPath(key);
            string backupPath = path + BACKUP_EXTENSION;

            string json = JsonConvert.SerializeObject(data, JsonSettings);
            string finalContent = ApplyEncryption(json, encryption);

            AtomicWrite(path, backupPath, finalContent);

#if UNITY_EDITOR
            Debug.Log($"[DataSystem] Saved: {path}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"[DataSystem] Save failed: {e}");
        }
    }

    public static T Load<T>(
        string key,
        DataEncryptionType encryption = DataEncryptionType.Json)
    {
        string path = GetPath(key);
        string backupPath = path + BACKUP_EXTENSION;

        if (!File.Exists(path))
            return default;

        try
        {
            string content = File.ReadAllText(path);
            string json = RemoveEncryption(content, encryption);

            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }
        catch
        {
            Debug.LogWarning("[DataSystem] Main file corrupted, trying backup...");

            if (File.Exists(backupPath))
            {
                try
                {
                    string content = File.ReadAllText(backupPath);
                    string json = RemoveEncryption(content, encryption);

                    return JsonConvert.DeserializeObject<T>(json, JsonSettings);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[DataSystem] Backup load failed: {e}");
                }
            }

            return default;
        }
    }

    public static bool Exists(string key)
    {
        return File.Exists(GetPath(key));
    }

    public static void Delete(string key)
    {
        string path = GetPath(key);
        string backupPath = path + BACKUP_EXTENSION;

        if (File.Exists(path))
            File.Delete(path);

        if (File.Exists(backupPath))
            File.Delete(backupPath);
    }

    #endregion

    #region ENCRYPTION

    private static string ApplyEncryption(string json, DataEncryptionType type)
    {
        switch (type)
        {
            case DataEncryptionType.Json:
                return json;

            case DataEncryptionType.Base64:
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            case DataEncryptionType.Xor:
                return Xor(json, "MySecretKey");

            case DataEncryptionType.Aes:
                return EncryptAes(json);

            default:
                return json;
        }
    }

    private static string RemoveEncryption(string content, DataEncryptionType type)
    {
        switch (type)
        {
            case DataEncryptionType.Json:
                return content;

            case DataEncryptionType.Base64:
                return Encoding.UTF8.GetString(Convert.FromBase64String(content));

            case DataEncryptionType.Xor:
                return Xor(content, "MySecretKey");

            case DataEncryptionType.Aes:
                return DecryptAes(content);

            default:
                return content;
        }
    }

    #endregion

    #region XOR

    private static string Xor(string data, string key)
    {
        var result = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
            result.Append((char)(data[i] ^ key[i % key.Length]));

        return result.ToString();
    }

    #endregion

    #region AES

    private static readonly string AesKey = "1234567890123456";
    private static readonly string AesIV = "6543210987654321";

    private static string EncryptAes(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(AesKey);
        aes.IV = Encoding.UTF8.GetBytes(AesIV);

        using var encryptor = aes.CreateEncryptor();
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        return Convert.ToBase64String(encrypted);
    }

    private static string DecryptAes(string cipherText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(AesKey);
        aes.IV = Encoding.UTF8.GetBytes(AesIV);

        using var decryptor = aes.CreateDecryptor();
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decrypted);
    }

    #endregion

    #region INTERNAL

    private static string GetPath(string key)
    {
        return Path.Combine(BasePath, key + EXTENSION);
    }

    private static void AtomicWrite(string path, string backupPath, string content)
    {
        string tempPath = path + ".tmp";

        File.WriteAllText(tempPath, content);

        if (File.Exists(path))
        {
            File.Copy(path, backupPath, true);
            File.Delete(path);
        }

        File.Move(tempPath, path);
    }

    #endregion
}