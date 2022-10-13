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

        /// <summary>
        /// Triggers when the pool expands itself.
        /// </summary>
        public Action OnExpand;

        #endregion

        #region Serialized fields

        [SerializeField]
        private PoolableMonobehaviour _poolableObjectPrefab;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.DisableInPlayMode]
#endif
        private int _size = 12;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.LeftToggle]
#endif
        private bool _isExpansible = true;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.ShowIf(nameof(_isExpansible))]
#endif
        private int _expansionSize = 3;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.LeftToggle]
#endif
        private bool _spawnAsChild = false;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.HideIf(nameof(_spawnAsChild))]
        [InspectorAttributes.LeftToggle]
#endif
        private bool _hasCustomParent = false;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.ShowIf(nameof(_hasCustomParent))]
#endif
        private Transform _customParent;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.LeftToggle]
#endif
        private bool _overrideObjectLifespan = false;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.ShowIf(nameof(_overrideObjectLifespan))]
#endif
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

        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        public PoolableMonobehaviour Pop()
        {
            var pooledObject = GetInactivePooledObject();

            if(pooledObject != null)
            {
                ResetObject(pooledObject);
                ActivateObject(pooledObject);
                return pooledObject;
            }
            
            if(_isExpansible)
            {
                ExpandPool();
                return Pop();
            }

            return null;
        }

        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Pop<T>() where T : PoolableMonobehaviour
        {
            var pooledObject = GetInactivePooledObject<T>();

            if (pooledObject != null)
            {
                ResetObject(pooledObject);
                ActivateObject(pooledObject);
                return pooledObject;
            }
            
            if(_isExpansible)
            {
                ExpandPool();
                return Pop<T>();
            }

            return null;
        }

        /// <summary>
        /// Add a new object to the pool.
        /// </summary>
        public void Add(PoolableMonobehaviour poolableObject)
        {
            _size++;
            RegisterObject(poolableObject);
            Return(poolableObject);
        }

        /// <summary>
        /// Return to the pool an object that belongs to it.
        /// </summary>
        /// <param name="poolableObject"></param>
        public void Return(PoolableMonobehaviour poolableObject)
        {
            if (_pooledObjectsDictionary.ContainsKey(poolableObject.Id) == false)
            {
                throw new ForeignObjectException($"There's no object with id {poolableObject.Id} in this pool.");
            }

            if (_pooledObjectsDictionary[poolableObject.Id] != poolableObject)
            {
                throw new ForeignObjectException($"The entry for id {poolableObject.Id} does not match the provided object.");
            }

            if (poolableObject.gameObject.activeInHierarchy == true)
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
            if (_pool.Count <= 0)
            {
                return null;
            }
            
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
