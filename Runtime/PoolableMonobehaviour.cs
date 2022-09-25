using System;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler
{
    public abstract class PoolableMonobehaviour : MonoBehaviour
    {
        #region Actions

        public Action OnInstantiate;

        public Action OnActivate;

        public Action OnDeactivate;

        #endregion

        #region Serialized fields

        [SerializeField]
        private bool _hasDeactivationTimer = true;

        [SerializeField]
        private float _lifespan = 2f;

        #endregion

        #region Unserialized fields

        private float _activeTime;

        #endregion

        #region Unity Events

        protected void Start()
        {
            OnInstantiate?.Invoke();
        }

        protected void OnEnable()
        {
            OnActivate?.Invoke();
        }

        protected void OnDisable()
        {
            OnDeactivate?.Invoke();
        }

        protected void Update()
        {
            TickDeactivationTime();
        }

        #endregion

        #region Public Methods

        public void Activate()
        {
            gameObject.SetActive(true);
            _activeTime = _lifespan;
            OnActivate?.Invoke();
        }

        public void Activate(float lifespan)
        {
            _lifespan = lifespan;
            Activate();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            OnDeactivate?.Invoke();
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
                Deactivate();
            }
        }

        #endregion
    }
}
