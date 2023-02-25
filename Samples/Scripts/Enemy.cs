using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSpark.ObjectPooler.Demo
{
    public class Enemy : PoolableMonobehaviour
    {
        [SerializeField]
        private LayerMask _playerShotLayer;

        private ParticleSystem _onDeathParticleSystem;

        private SpriteRenderer _spriteRenderer;

        private Collider2D _collider2D;

        private void Awake()
        {
            _onDeathParticleSystem = GetComponentInChildren<ParticleSystem>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnActivate += Restore;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnActivate -= Restore;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsInLayerMask(other.gameObject, _playerShotLayer))
            {
                Die();
                StartDeactivation();
            }
        }

        private void Restore()
        {
            _spriteRenderer.enabled = true;
            _collider2D.enabled = true;
        }

        private void Die()
        {
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;
            PlayVFX();
        }

        private void PlayVFX()
        {
            _onDeathParticleSystem.Play();
        }

        private bool IsInLayerMask(GameObject obj, LayerMask mask) 
        {
            return ((mask.value & (1 << obj.layer)) > 0);
        }
    }
}
