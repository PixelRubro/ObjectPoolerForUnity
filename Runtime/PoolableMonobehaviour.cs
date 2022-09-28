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
        private bool _hasDeactivationTimer = true;

        [SerializeField]
        [InspectorAttributes.ShowIf(nameof(_hasDeactivationTimer))]
        private float _deactivationTimerDuration = 2f;

        [SerializeField]
        private bool _hasDeactivationByTicks = false;

        [SerializeField]
        private int _ticksToDeactivate = 3;

        #endregion

        #region Unserialized fields

        private float _activeTime;

        private int _id;

        private bool _isInitialized;

        private int _ticksLeft;

        #endregion

        #region Properties

        public int Id => _id;

        #endregion

        #region Unity Events

        protected void Start()
        {
            CheckInitialization();
        }

        protected void OnEnable()
        {
            OnActivate?.Invoke();
        }

        protected void OnDisable()
        {
            OnDeactivation?.Invoke();
        }

        protected void Update()
        {
            TickDeactivationTime();
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

        public void Initialize(int id)
        {
            if (_isInitialized)
            {
                return;
            }

            _id = id;
            _isInitialized = true;
            OnInitialize?.Invoke();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            _activeTime = _deactivationTimerDuration;
            _ticksLeft = _ticksToDeactivate;
            OnActivate?.Invoke();
        }

        public void Activate(float lifespan)
        {
            gameObject.SetActive(true);
            _hasDeactivationTimer = true;
            _hasDeactivationByTicks = false;
            _activeTime = lifespan;
            OnActivate?.Invoke();
        }

        public void Activate(int ticks)
        {
            gameObject.SetActive(true);
            _hasDeactivationTimer = false;
            _hasDeactivationByTicks = true;
            _ticksLeft = ticks;
            OnActivate?.Invoke();
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

        public void StartDeactivation(Action callback)
        {
            OnDeactivationProgrammed?.Invoke();
            callback.Invoke();
            Invoke(nameof(DeactivateGameObject), _deactivationCountdownDuration);
        }

        public void StartDeactivation(Action callback, float time)
        {
            OnDeactivationProgrammed?.Invoke();
            callback.Invoke();
            Invoke(nameof(DeactivateGameObject), time);
        }

        public void DeactivateImmediate()
        {
            DeactivateGameObject();
        }

        #endregion

        #region Private Methods

        private void TickDeactivationTime()
        {
            if (!_hasDeactivationTimer)
            {
                return;
            }

            _activeTime -= Time.deltaTime;

            if (_activeTime <= 0f)
            {
                StartDeactivation();
            }
        }

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
