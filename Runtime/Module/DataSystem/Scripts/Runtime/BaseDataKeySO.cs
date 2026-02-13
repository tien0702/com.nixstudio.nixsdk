using UnityEngine;

public class BaseDataKeySO<T> : BaseDataSO where T : class, new()
{
    [SerializeField] protected string _Key;
    [SerializeField] protected T _Data;

    public override void Save()
    {
        DataSystem.Save(_Key, _Data, Encrypt);
    }

    public override void Load()
    {
        _Data = DataSystem.Load<T>(_Key, Encrypt);
    }

    public override void Delete()
    {
        DataSystem.Delete(_Key);
    }
}