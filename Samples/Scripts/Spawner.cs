using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRouge.ObjectPooler.Demo
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private float _spawnInterval = 2f;

        private QuickObjectPooler _objectPooler;

        private void Awake()
        {
            _objectPooler = GetComponent<QuickObjectPooler>();
        }

        private void Start()
        {
            Invoke(nameof(Spawn), _spawnInterval);
        }

        private void Spawn()
        {
            var position = new Vector2(Random.Range(-5f, 5f), Random.Range(-1f, 3f));
            var enemy = _objectPooler.Pop<Enemy>(position);
            Invoke(nameof(Spawn), _spawnInterval);
        }
    }   
}
