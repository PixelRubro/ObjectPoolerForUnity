using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler
{
    public class ObjectPooler : MonoBehaviour
    {
        #region Actions

        public Action OnExpand;

        #endregion

        #region Serialized fields

        [SerializeField]
        private PoolableMonobehaviour _poolableObjectPrefab;

        [SerializeField]
        private int _size = 12;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _isExpandable = true;

        [InspectorAttributes.ShowIf(nameof(_isExpandable))]
        [SerializeField]
        private int _expansionSize = 3;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        [InspectorAttributes.ShowIf(nameof(_hasCustomParent), false)]
        private bool _spawnAsChild = false;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _hasCustomParent = false;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_hasCustomParent))]
        private Transform _customParent;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _overrideObjectLifespan = false;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_overrideObjectLifespan))]
        private float _objectLifespan = 1.5f;

        #endregion

        #region Non-serialized fields

        private Stack<PoolableMonobehaviour> _pool;

        private Transform _transform;

        #endregion

        #region Properties

        public int Size => _size;

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

        public void ReturnObjectToPool(PoolableMonobehaviour poolableObject)
        {
            if (poolableObject.gameObject.activeInHierarchy == false)
            {
                _pool.Push(poolableObject);
            }
        }

        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        #region Pool operations

        private void CreatePool()
        {
            _pool = new Stack<PoolableMonobehaviour>();

            for (int i = 0; i < _size; i++)
            {
                var poolableObject = CreateObject();
                poolableObject.gameObject.SetActive(false);
                _pool.Push(poolableObject);
            }
        }

        private void ExpandPool()
        {
            _size += _expansionSize;

            for (int i = 0; i < _expansionSize; i++)
            {
                var poolableObject = CreateObject();
                poolableObject.gameObject.SetActive(false);
                _pool.Push(poolableObject);
            }

            OnExpand?.Invoke();
        }

        #endregion

        #region Poolable object handling

        private PoolableMonobehaviour CreateObject()
        {
            var parent = GetSpawnParent();
            var poolableObject = Instantiate<PoolableMonobehaviour>(_poolableObjectPrefab, parent, false);
            poolableObject.OnDeactivate += () => ReturnObjectToPool(poolableObject);
            return poolableObject;
        }

        private PoolableMonobehaviour GetInactivePooledObject()
        {
            return _pool.FirstOrDefault<PoolableMonobehaviour>((obj) => obj.gameObject.activeInHierarchy == false);
        }

        private T GetInactivePooledObject<T>() where T : PoolableMonobehaviour
        {
            var pooledObject = _pool.FirstOrDefault<PoolableMonobehaviour>((obj) => obj is T && obj.gameObject.activeInHierarchy == false);
            return pooledObject as T;
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

        private Transform GetSpawnParent()
        {
            if (_hasCustomParent)
            {
                return _customParent;
            }

            if (_spawnAsChild)
            {
                return _transform;
            }

            return null;
        }

        #endregion  
    }
}
