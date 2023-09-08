using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VermillionVanguard.ObjectPooler.Demo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Shot : PoolableMonobehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields
        #endregion

        #region Non-serialized fields

        private float _damage = 1f;

        private float _speed = 500f;

        private Rigidbody2D _rb2d;
        
        #endregion

        #region Constant fields
        #endregion

        #region Unity events

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Public methods

        public void Setup(float damage, float speed)
        {
            _damage = damage;
            _speed = speed;
            SetInitialVelocity();
        }

        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        private void SetInitialVelocity()
        {
            _rb2d.velocity = Time.deltaTime * _speed * Vector2.up;
        }

        #endregion  
    }
}
