using UnityEngine;

public class BaseDataSO : ScriptableObject
{
    public DataEncryptionType Encrypt = DataEncryptionType.Json;

    public virtual void Save()
    {
    }

    public virtual void Load()
    {
    }

    public virtual void Delete()
    {
    }
}