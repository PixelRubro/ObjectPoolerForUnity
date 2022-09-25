using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler
{
    public class ObjectPooler : MonoBehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields

        [SerializeField]
        private PoolableMonobehaviour _poolableObjectPrefab;

        [SerializeField]
        private int _capacity = 16;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _isExpandable = true;

        [InspectorAttributes.ShowIf(nameof(_isExpandable))]
        [SerializeField]
        private int _expansionSize = 3;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _spawnAsChild = false;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _overrideObjectLifespan = false;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_overrideObjectLifespan))]
        private float _objectLifespan = 1.5f;

        #endregion

        #region Non-serialized fields

        private PoolableMonobehaviour[] _pooledObjects;

        private Transform _transform;

        #endregion

        #region Properties

        public int Capacity => _capacity;

        #endregion

        #region Constant fields
        #endregion

        #region Unity events

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            CreatePool();
        }

        #endregion

        #region Public methods

        public PoolableMonobehaviour Pop()
        {
            var pooledObject = GetInactivePooledObject();

            if(pooledObject != null)
            {
                ResetObject(pooledObject);
                ActivateObject(pooledObject);
                return pooledObject;
            }
            
            if(_isExpandable)
            {
                ExpandPool();
                return Pop();
            }

            return null;
        }

        public T Pop<T>() where T : PoolableMonobehaviour
        {
            var pooledObject = GetInactivePooledObject<T>();

            if (pooledObject != null)
            {
                ResetObject(pooledObject);
                ActivateObject(pooledObject);
                return pooledObject;
            }
            
            if(_isExpandable)
            {
                ExpandPool();
                return Pop<T>();
            }

            return null;
        }

        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        private PoolableMonobehaviour GetInactivePooledObject()
        {
            return _pooledObjects.FirstOrDefault<PoolableMonobehaviour>((obj) => obj.gameObject.activeInHierarchy == false);
        }

        private T GetInactivePooledObject<T>() where T : PoolableMonobehaviour
        {
            var pooledObject = _pooledObjects.FirstOrDefault<PoolableMonobehaviour>((obj) => obj is T && obj.gameObject.activeInHierarchy == false);
            return pooledObject as T;
        }

        private void ExpandPool()
        {
            _capacity += _expansionSize;
            var oldObjects = _pooledObjects;
            _pooledObjects = new PoolableMonobehaviour[_capacity];
            var parent = FindSpawnParent();

            for (int i = 0; i < oldObjects.Length; i++)
            {
                _pooledObjects[i] = oldObjects[i];
            }

            for (int i = oldObjects.Length; i < _capacity; i++)
            {
                _pooledObjects[i] = Instantiate<PoolableMonobehaviour>(_poolableObjectPrefab, parent, false);
                _pooledObjects[i].gameObject.SetActive(false);
            }
        }

        private void CreatePool()
        {
            _pooledObjects = new PoolableMonobehaviour[_capacity];
            var parent = FindSpawnParent();
            
            for (int i = 0; i < _capacity; i++)
            {
                _pooledObjects[i] = Instantiate<PoolableMonobehaviour>(_poolableObjectPrefab, parent, false);
                _pooledObjects[i].gameObject.SetActive(false);
            }
        }

        private Transform FindSpawnParent()
        {
            return _spawnAsChild ? _transform : null;
        }

        private void ActivateObject(PoolableMonobehaviour pooledObject)
        {
            if (_overrideObjectLifespan)
            {
                pooledObject.Activate(_objectLifespan);
            }
            else
            {
                pooledObject.Activate();
            }

            if (_spawnAsChild && pooledObject.transform.parent == null)
            {
                pooledObject.transform.parent = _transform;
            }
        }

        private void ResetObject(PoolableMonobehaviour poolableObject)
        {
            poolableObject.transform.position = Vector3.zero;
            poolableObject.transform.rotation = Quaternion.identity;
        }

        #endregion  
    }
}
