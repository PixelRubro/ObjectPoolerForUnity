using System;
using System.Collections;
using UnityEngine;
using VermillionVanguard.ObjectPooler.Exceptions;

namespace VermillionVanguard.ObjectPooler
{
    public abstract class PoolableMonobehaviour : MonoBehaviour
    {
        #region Actions

        /// <summary>
        /// Triggered when the object is initialized by the pool.
        /// </summary>
        public Action OnInitialize;

        /// <summary>
        /// Triggered when the object is activated by the pool.
        /// </summary>
        public Action OnActivate;

        /// <summary>
        /// Triggered when the object has its deactivation process initiated.
        /// </summary>
        public Action OnDeactivationProgrammed;

        /// <summary>
        /// Triggered when the object has its game object deactivated.
        /// </summary>
        public Action OnDeactivation;

        #endregion

        #region Serialized fields

        // [SerializeField]
        // private float _deactivationCountdownDuration = 2f;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.LeftToggle]
#endif
        private bool _hasDeactivationTimer = true;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.ShowIf(nameof(_hasDeactivationTimer))]
#endif
        private float _deactivationTimerDuration = 2f;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.HideIf(nameof(_hasDeactivationTimer))]
        [InspectorAttributes.LeftToggle]
#endif
        private bool _hasDeactivationByTicks = false;

        [SerializeField]
#if UNITY_EDITOR
        [InspectorAttributes.ShowIf(nameof(_hasDeactivationByTicks))]
#endif
        private int _ticksToDeactivate = 3;

        #endregion

        #region Unserialized fields

        private int _id;

        private bool _isInitialized;

        private int _ticksLeft;

        private float _timeLeft;

        private Transform _transform;

        private QuickObjectPooler _pool;

        #endregion

        #region Properties

        public int Id => _id;

        #endregion

        #region Unity Events

        // private void Awake()
        // {
        //     AssignComponents();
        // }

        protected virtual void Start()
        {
            CheckInitialization();
        }

        protected virtual void Update()
        {
            ProcessDeactivationTime();
        }

        protected virtual void OnEnable()
        {
            Activate();
        }

        protected virtual void OnDisable()
        {
            OnDeactivation?.Invoke();
        }

        protected void OnDestroy()
        {
            OnDeactivation = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// For a poolable object that has its deactivation driven by "ticks" this method 
        /// decrements a tick. When there are no ticks left, the object is deactivated.
        /// </summary>
        public void Tick()
        {
            if (!_hasDeactivationByTicks) return;
            
            _ticksLeft--;

            if (_ticksLeft > 0) return;
            
            StartDeactivation();
        }

        /// <summary>
        /// Start deactivation process.
        /// </summary>
        public void StartDeactivation()
        {
            OnDeactivationProgrammed?.Invoke();
        }
        
        /// <summary>
        /// Deactivate object at once.
        /// </summary>
        public void DeactivateImmediate()
        {
            DeactivateGameObject();
        }

        public void SetPosition(Vector3 spawnPosition)
        {
            _transform.position = spawnPosition;
        }

        #endregion

        #region Internal methods

        internal void Initialize(int id, QuickObjectPooler pool)
        {
            if (_isInitialized)
            {
                return;
            }
            
            AssignComponents();
            _id = id;
            _pool = pool;
            _isInitialized = true;
            OnInitialize?.Invoke();
        }

        internal void Activate()
        {
            gameObject.SetActive(true);
            _ticksLeft = _ticksToDeactivate;
            OnActivate?.Invoke();
            _timeLeft = _deactivationTimerDuration;
        }

        internal void Activate(float lifespan)
        {
            gameObject.SetActive(true);
            _hasDeactivationTimer = true;
            _hasDeactivationByTicks = false;
            OnActivate?.Invoke();
            // StartCoroutine(KillObjectInTime(lifespan));
            _timeLeft = lifespan;
        }

        internal void Activate(int ticks)
        {
            gameObject.SetActive(true);
            _hasDeactivationTimer = false;
            _hasDeactivationByTicks = true;
            _ticksLeft = ticks;
            OnActivate?.Invoke();
        }

        #endregion

        #region Private Methods

        private void CheckInitialization()
        {
            if ((!_isInitialized) || (_id == 0))
            {
                throw new ObjectInitializationException("This object was not initialized by an object pool.");
            }
        }

        private void ProcessDeactivationTime()
        {
            if (_hasDeactivationTimer == false)
            {
                return;
            }

            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f)
            {
                DeactivateGameObject();
            }
        }

        private void AssignComponents()
        {
            _transform = transform;
        }

        private void DeactivateGameObject()
        {
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }
            
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }

            OnDeactivation?.Invoke();
            _pool.Return(this);
        }

        private IEnumerator KillObjectInTime(float lifespan)
        {
            yield return new WaitForSeconds(lifespan);
            
            StartDeactivation();
        }

        #endregion
    }
}
