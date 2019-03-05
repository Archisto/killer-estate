using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class EnemySpawner : MonoBehaviour
    {
        public bool active;

        public int maxConcurrentEnemies;
        public float spawnInterval;
        public Vector3 spawnAreaLowerLeftCorner;
        public Vector3 spawnAreaUpperRightCorner;
        public Enemy enemyPrefab;

        private EnemySpawnPoint[] _spawnPoints;
        private List<List<EnemySpawnPoint>> _validRoomSpawnPoints;
        private Pool<Enemy> _enemyPool;
        private Timer spawnTimer;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _spawnPoints = FindObjectsOfType<EnemySpawnPoint>();
            _validRoomSpawnPoints = new List<List<EnemySpawnPoint>>();
            _enemyPool = new Pool<Enemy>(maxConcurrentEnemies, false, enemyPrefab);
            spawnTimer = new Timer(spawnInterval, true);
            spawnTimer.Activate();

            if (_spawnPoints.Length == 0)
            {
                Debug.LogError(Utils.GetObjectMissingString("EnemySpawnPoint"));
            }
            else
            {
                AddRoomValidSpawnPoints(GameManager.Instance.CurrentRoom);
                GameManager.Instance.RoomOpened += AddRoomValidSpawnPoints;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (active && spawnTimer.Check())
            {
                SpawnEnemy();
                spawnTimer.Activate();
            }
        }

        private void AddRoomValidSpawnPoints(Room room)
        {
            List<EnemySpawnPoint> roomSpawnPoints = new List<EnemySpawnPoint>();
            foreach (EnemySpawnPoint spawn in _spawnPoints)
            {
                if (spawn.Room == room)
                {
                    roomSpawnPoints.Add(spawn);
                }
            }
            _validRoomSpawnPoints.Add(roomSpawnPoints);
            Debug.Log("Enemy spawnpoints added: " + roomSpawnPoints.Count);
        }

        private void SpawnEnemy()
        {
            Enemy enemy = _enemyPool.GetPooledObject();
            if (enemy != null)
            {
                EnemySpawnPoint spawn = GetRandomSpawnPoint();
                if (spawn != null)
                {
                    spawn.SpawnEnemy(enemy);
                }
            }
        }

        private Vector3 GetRandomPositionWithinBounds()
        {
            Vector3 position = spawnAreaLowerLeftCorner;
            position.x = Random.Range(spawnAreaLowerLeftCorner.x, spawnAreaUpperRightCorner.x);
            position.z = Random.Range(spawnAreaLowerLeftCorner.z, spawnAreaUpperRightCorner.z);
            return position;
        }

        private EnemySpawnPoint GetRandomSpawnPoint()
        {
            if (_validRoomSpawnPoints.Count > 0)
            {
                int roomIndex = Random.Range(0, _validRoomSpawnPoints.Count);
                int spawnIndex = Random.Range(0, _validRoomSpawnPoints[roomIndex].Count);
                //Debug.Log("Spawn room: " + roomIndex + "; Spawn point: " + spawnIndex);
                return _validRoomSpawnPoints[roomIndex][spawnIndex];
            }
            else
            {
                return null;
            }
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.gray;
        //    Utils.DrawBoxGizmo(spawnAreaLowerLeftCorner, spawnAreaUpperRightCorner);
        //}
    }
}
