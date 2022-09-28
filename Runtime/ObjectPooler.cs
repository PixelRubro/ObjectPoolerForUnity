using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoftBoiledGames.ObjectPooler.Exceptions;

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

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_isExpandable))]
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

        private int _currentId = 1;

        private Dictionary<int, PoolableMonobehaviour> _pooledObjectsDictionary = new Dictionary<int, PoolableMonobehaviour>();

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
            FillPool(_size);
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

        public void Add(PoolableMonobehaviour poolableObject, bool forceDeactivation = true)
        {
            _size++;
            RegisterObject(poolableObject);
            Return(poolableObject, forceDeactivation);
        }

        public void Return(PoolableMonobehaviour poolableObject, bool forceDeactivation = true)
        {
            if (_pooledObjectsDictionary.ContainsKey(poolableObject.Id) == false)
            {
                throw new ForeignObjectException($"There's no object with id {poolableObject.Id} in this pool.");
            }

            if (_pooledObjectsDictionary[poolableObject.Id] != poolableObject)
            {
                throw new ForeignObjectException($"The entry for id {poolableObject.Id} does not match the provided object.");
            }

            if ((poolableObject.gameObject.activeInHierarchy == true) && (forceDeactivation))
            {
                poolableObject.gameObject.SetActive(false);
            }

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
        }

        private void ExpandPool()
        {
            _size += _expansionSize;
            FillPool(_expansionSize);
            OnExpand?.Invoke();
        }

        private void FillPool(int objectCount)
        {
            for (int i = 0; i < objectCount; i++)
            {
                var poolableObject = CreateObject();
                RegisterObject(poolableObject);
                _pool.Push(poolableObject);
            }
        }

        #endregion

        #region Poolable object handling

        private PoolableMonobehaviour CreateObject()
        {
            var parent = GetSpawnParent();
            var poolableObject = Instantiate<PoolableMonobehaviour>(_poolableObjectPrefab, parent, false);
            poolableObject.Initialize(_currentId++);
            poolableObject.gameObject.SetActive(false);
            poolableObject.OnDeactivation += () => Return(poolableObject);
            return poolableObject;
        }

        private void RegisterObject(PoolableMonobehaviour poolableObject)
        {
            var id = poolableObject.Id;

            if (_pooledObjectsDictionary.ContainsKey(id))
            {
                throw new DuplicateRegisteringException($"There's already an object registered with id {id}");
            }
            
            _pooledObjectsDictionary.Add(id, poolableObject);
        }

        private PoolableMonobehaviour GetInactivePooledObject()
        {
            return _pool.Pop();
        }

        private T GetInactivePooledObject<T>() where T : PoolableMonobehaviour
        {
            var pooledObject = _pool.Pop();
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
