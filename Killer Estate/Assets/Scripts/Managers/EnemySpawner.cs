using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class EnemySpawner : MonoBehaviour
    {
        public int maxConcurrentEnemies;
        public float spawnInterval;
        public GameObject spawnPointParent;
        public Vector3 spawnAreaLowerLeftCorner;
        public Vector3 spawnAreaUpperRightCorner;
        public TargetDummy enemyPrefab;

        private Pool<TargetDummy> _enemyPool;
        private List<Transform> _spawnPoints;
        private Timer spawnTimer;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _enemyPool = new Pool<TargetDummy>(maxConcurrentEnemies, false, enemyPrefab);
            InitSpawnPoints();
            spawnTimer = new Timer(spawnInterval, true);
            spawnTimer.Activate();
        }

        private void InitSpawnPoints()
        {
            _spawnPoints = new List<Transform>();
            Transform[] transforms = spawnPointParent.GetComponentsInChildren<Transform>();

            // Gets rid of the parent's transform
            foreach (Transform t in transforms)
            {
                if (t != spawnPointParent.transform)
                {
                    _spawnPoints.Add(t);
                }
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (spawnTimer.Check())
            {
                SpawnEnemy();
                spawnTimer.Activate();
            }
        }

        private void SpawnEnemy()
        {
            TargetDummy enemy = _enemyPool.GetPooledObject();
            if (enemy != null)
            {
                enemy.transform.position = GetRandomSpawnPoint();
                enemy.InitVelocity();
            }
        }

        private Vector3 GetRandomPositionWithinBounds()
        {
            Vector3 position = spawnAreaLowerLeftCorner;
            position.x = Random.Range(spawnAreaLowerLeftCorner.x, spawnAreaUpperRightCorner.x);
            position.z = Random.Range(spawnAreaLowerLeftCorner.z, spawnAreaUpperRightCorner.z);
            return position;
        }

        private Vector3 GetRandomSpawnPoint()
        {
            int randomIndex = Random.Range(0, _spawnPoints.Count);
            return _spawnPoints[randomIndex].position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            Utils.DrawBoxGizmo(spawnAreaLowerLeftCorner, spawnAreaUpperRightCorner);
        }
    }
}
