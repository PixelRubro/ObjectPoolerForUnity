using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSparkStudio.ObjectPooler.Demo
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class ShipController : MonoBehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields

        [SerializeField]
        private float _movingSpeed = 370f;

        [SerializeField]
        private Transform _shotOrigin;

        #endregion

        #region Non-serialized fields

        private Vector2 _inputVector;

        private Rigidbody2D _rb2d;

        private ObjectPooler _objectPooler;

        #endregion

        #region Constant fields
        #endregion

        #region Unity events

        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _objectPooler = GetComponent<ObjectPooler>();
        }

        private void Update()
        {
            ReadPlayerInput();
            Move();
            Fire();
        }

        #endregion

        #region Public methods
        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        private void ReadPlayerInput()
        {
            _inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void Move()
        {
            var movementVector = _inputVector.normalized;
            _rb2d.velocity = Time.deltaTime * _movingSpeed * movementVector;
        }

        private void Fire()
        {
            if(!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            var shot = _objectPooler.Pop<Shot>();
            shot.transform.position = _shotOrigin.position;
            shot.Setup(1f, 800f);
        }

        #endregion  
    }
}

