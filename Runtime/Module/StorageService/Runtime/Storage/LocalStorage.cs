using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace NIX.Module.StorageService
{
    public class PublicFieldsOnlyContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = new List<JsonProperty>();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var f in fields)
            {
                var prop = base.CreateProperty(f, memberSerialization);
                prop.Readable = true;
                prop.Writable = true;
                props.Add(prop);
            }

            return props;
        }
    }
    public class LocalStorage : IDataStorage
    {
        protected string _StorageKey;
        protected readonly JsonSerializerSettings _JsonSetting = new JsonSerializerSettings
        {
            ContractResolver = new PublicFieldsOnlyContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.None
        };
        public string StorageKey => _StorageKey;

        public LocalStorage()
        {
            _StorageKey = "Player";
        }
        
        public LocalStorage(string storageKey)
        {
            _StorageKey = storageKey;
        }

        public async Task<bool> SaveAsync<T>(string savedKey, T value)
        {
            string directoryPath = GetDirectoryPath();
            EnsureDirectoryExists(directoryPath);

            string json = JsonConvert.SerializeObject(value, _JsonSetting);
            await File.WriteAllTextAsync(GetFilePath(savedKey), json);
            return true;
        }

        public async Task<T> LoadAsync<T>(string savedKey)
        {
            string path = GetFilePath(savedKey);
            if (File.Exists(path))
            {
                string json = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<T>(json, _JsonSetting);
            }

            return default;
        }

        public async Task<bool> DeleteAsync(string savedKey)
        {
            string path = GetFilePath(savedKey);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await Task.CompletedTask;
            return true;
        }

        private string GetFilePath(string savedKey) =>
            Path.Combine(Application.persistentDataPath, StorageKey + "/" + savedKey + ".json");

        private string GetDirectoryPath() =>
            Path.Combine(Application.persistentDataPath, StorageKey);

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}