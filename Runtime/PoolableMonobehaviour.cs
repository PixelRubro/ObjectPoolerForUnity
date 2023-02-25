using System;
using UnityEngine;
using PixelSparkStudio.ObjectPooler.Exceptions;

namespace PixelSparkStudio.ObjectPooler
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

        [SerializeField]
        private float _deactivationCountdownDuration = 2f;

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

        #endregion

        #region Properties

        public int Id => _id;

        #endregion

        #region Unity Events

        protected virtual void Start()
        {
            CheckInitialization();
        }

        protected virtual void OnEnable()
        {
            Activate();
        }

        protected virtual void OnDisable()
        {
            DeactivateGameObject();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// For a poolable object that has its deactivation driven by "ticks" this method 
        /// decrements a tick. When there are no ticks left, the object is deactivated.
        /// </summary>
        public void Tick()
        {
            if (_hasDeactivationByTicks)
            {
                _ticksLeft--;

                if (_ticksLeft <= 0)
                {
                    StartDeactivation();
                }
            }
        }

        /// <summary>
        /// Start deactivation process.
        /// </summary>
        public void StartDeactivation()
        {
            OnDeactivationProgrammed?.Invoke();
            Invoke(nameof(DeactivateGameObject), _deactivationCountdownDuration);
        }

        /// <summary>
        /// Start deactivation process.
        /// </summary>
        public void StartDeactivation(float time)
        {
            OnDeactivationProgrammed?.Invoke();
            Invoke(nameof(DeactivateGameObject), time);
        }
        
        /// <summary>
        /// Deactivate object at once.
        /// </summary>
        public void DeactivateImmediate()
        {
            DeactivateGameObject();
        }

        #endregion

        #region Internal methods

        internal void Initialize(int id)
        {
            if (_isInitialized)
            {
                return;
            }

            _id = id;
            _isInitialized = true;
            OnInitialize?.Invoke();
        }

        internal void Activate()
        {
            gameObject.SetActive(true);
            _ticksLeft = _ticksToDeactivate;
            OnActivate?.Invoke();

            if (_hasDeactivationTimer)
            {
                Invoke(nameof(StartDeactivation), _deactivationTimerDuration);
            }
        }

        internal void Activate(float lifespan)
        {
            gameObject.SetActive(true);
            _hasDeactivationTimer = true;
            _hasDeactivationByTicks = false;
            OnActivate?.Invoke();
            Invoke(nameof(StartDeactivation), lifespan);
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

        private void DeactivateGameObject()
        {
            if (gameObject.activeInHierarchy == false)
            {
                return;
            }

            gameObject.SetActive(false);
            OnDeactivation?.Invoke();
        }

        #endregion
    }
}
