using System.Collections.Generic;
using System.Threading.Tasks;
using NIX.Core.DesignPatterns;
using UnityEngine;

namespace NIX.Module.StorageService
{
    public class StorageService : IService
    {
        #region Storages

        protected readonly Dictionary<string, IDataStorage> _dataStorage = new();

        public void AddStorage(IDataStorage storage)
        {
            _dataStorage[storage.StorageKey] = storage;
        }

        // Async functions
        public async Task<bool> SaveAsync<T>(string storageKey, string savedKey, T data)
        {
            if (_dataStorage.TryGetValue(storageKey, out var storage))
            {
                return await storage.SaveAsync(savedKey, data);
            }

            Debug.LogError("Type is not added to data storage");
            return false;
        }

        public async Task<T> LoadAsync<T>(string storageKey, string savedKey)
        {
            if (_dataStorage.TryGetValue(storageKey, out var storage))
            {
                return await storage.LoadAsync<T>(savedKey);
            }

            Debug.LogError("Type is not added to data storage");
            return default(T);
        }

        public async Task<bool> DeleteAsync(string storageKey, string savedKey)
        {
            if (_dataStorage.TryGetValue(storageKey, out var storage))
            {
                return await storage.DeleteAsync(savedKey);
            }

            Debug.LogError("Type is not added to data storage");
            return false;
        }

        #endregion
    }
}