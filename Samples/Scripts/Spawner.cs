using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBoiledGames.ObjectPooler.Demo
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private float _spawnInterval = 2f;

        private ObjectPooler _objectPooler;

        private void Awake()
        {
            _objectPooler = GetComponent<ObjectPooler>();
        }

        private void Start()
        {
            Invoke(nameof(Spawn), _spawnInterval);
        }

        private void Spawn()
        {
            var enemy = _objectPooler.Pop<Enemy>();

            if (enemy != null)
            {
                enemy.transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-1f, 3f));
            }

            Invoke(nameof(Spawn), _spawnInterval);
        }
    }   
}
