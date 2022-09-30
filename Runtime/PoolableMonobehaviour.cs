using System;
using UnityEngine;
using SoftBoiledGames.ObjectPooler.Exceptions;

namespace SoftBoiledGames.ObjectPooler
{
    public abstract class PoolableMonobehaviour : MonoBehaviour
    {
        #region Actions

        public Action OnInitialize;

        public Action OnActivate;

        public Action OnDeactivationProgrammed;

        public Action OnDeactivation;

        #endregion

        #region Serialized fields

        [SerializeField]
        private float _deactivationCountdownDuration = 2f;

        [SerializeField]
        [InspectorAttributes.LeftToggle]
        private bool _hasDeactivationTimer = true;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_hasDeactivationTimer))]
        private float _deactivationTimerDuration = 2f;

        [SerializeField]
        [InspectorAttributes.HideIf(nameof(_hasDeactivationTimer))]
        [InspectorAttributes.LeftToggle]
        private bool _hasDeactivationByTicks = false;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_hasDeactivationByTicks))]
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

        public void StartDeactivation()
        {
            OnDeactivationProgrammed?.Invoke();
            Invoke(nameof(DeactivateGameObject), _deactivationCountdownDuration);
        }

        public void StartDeactivation(float time)
        {
            OnDeactivationProgrammed?.Invoke();
            Invoke(nameof(DeactivateGameObject), time);
        }

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
