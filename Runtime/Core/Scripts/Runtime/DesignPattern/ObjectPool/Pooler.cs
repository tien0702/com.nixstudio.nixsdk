using System.Collections.Generic;
using UnityEngine;

namespace NIX.Core.DesignPatterns
{
    public enum SpawnType
    {
        // Spawn object into world
        InWorld,

        // Create a holder, spawn to holder
        InGroup,

        // Spawn into gameObject
        InObject
    }

    public class Pooler : MonoBehaviour
    {
        #region Shared Pools

        private static Dictionary<string, Pooler> _registeredPools = new();

        public static void RegisterPooler(string id, Pooler pool)
        {
            if (!_registeredPools.TryAdd(id, pool))
            {
                Debug.LogErrorFormat("Failed to register pooler for id: {0}, {1}", id, pool);
            }
        }

        public static void UnregisterPooler(string id)
        {
            _registeredPools.Remove(id);
        }

        public static Pooler GetPool(string id)
        {
            return _registeredPools.GetValueOrDefault(id);
        }

        public static bool TryGetPool(string id, out Pooler pooler)
        {
            return _registeredPools.TryGetValue(id, out pooler);
        }

        public static T GetSharedObject<T>(string poolId) where T : Component
        {
            if (TryGetPool(poolId, out Pooler pooler)) return pooler.GetObject<T>();
            Debug.LogErrorFormat("Failed to get shared object for pool id: {0}, {1}", poolId, typeof(T));
            return null;
        }

        public static List<T> GetSharedObjects<T>(string poolId, int count) where T : Component
        {
            if (TryGetPool(poolId, out Pooler pooler)) return pooler.GetObjects<T>(count);
            Debug.LogErrorFormat("Failed to get shared object for pool id: {0}, {1}", poolId, typeof(T));
            return null;
        }

        #endregion

        [SerializeField] protected string _Id;
        [SerializeField] protected GameObject _Prefab;
        [SerializeField] protected SpawnType _SpawnType = SpawnType.InObject;
        [SerializeField] protected int _InitialCapacity = 3;
        [SerializeField] protected int _MaxCapacity = 50;

        protected List<GameObject> _PooledObjects = new();
        protected Transform _GroupHolder;

        protected virtual void Awake()
        {
            var spawnHolder = GetSpawnHolder();
            for (int i = 0; i < _InitialCapacity; ++i)
            {
                var obj = Instantiate(_Prefab, spawnHolder);
                obj.SetActive(false);
                _PooledObjects.Add(obj);
            }
        }

        protected virtual void OnEnable()
        {
            if (!string.IsNullOrEmpty(_Id))
            {
                RegisterPooler(_Id, this);
            }
        }

        protected virtual void OnDisable()
        {
            if (!string.IsNullOrEmpty(_Id))
            {
                UnregisterPooler(_Id);
            }
        }

        protected virtual Transform GetSpawnHolder()
        {
            switch (_SpawnType)
            {
                case SpawnType.InWorld: return null;
                case SpawnType.InGroup:
                    if (_GroupHolder == null)
                    {
                        _GroupHolder = new GameObject(_Prefab.name).transform;
                    }

                    return _GroupHolder;
                case SpawnType.InObject: return transform;
            }

            return null;
        }

        public virtual T GetObject<T>() where T : Component
        {
            foreach (var pooledObj in _PooledObjects)
            {
                if (pooledObj.activeSelf) continue;
                pooledObj.SetActive(true);
                return pooledObj.GetComponent<T>();
            }

            return CreateNewObject().GetComponent<T>();
        }

        public virtual List<T> GetObjects<T>(int count) where T : Component
        {
            var result = new List<T>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(GetObject<T>());
            }

            return result;
        }

        public virtual List<T> GetActiveObjects<T>() where T : Component
        {
            var result = new List<T>();
            for (int i = 0; i < _PooledObjects.Count; ++i)
            {
                if (_PooledObjects[i].activeSelf) result.Add(_PooledObjects[i].GetComponent<T>());
            }

            return result;
        }

        public virtual GameObject CreateNewObject()
        {
            var newObj = Instantiate(_Prefab, GetSpawnHolder());
            _PooledObjects.Add(newObj);
            return newObj;
        }

        public virtual void ReturnAllToPool()
        {
            foreach (var pooledObj in _PooledObjects)
            {
                pooledObj.SetActive(false);
            }
        }
    }
}