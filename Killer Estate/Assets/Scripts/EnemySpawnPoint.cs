using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Window _targetWindow;

        public Room Room { get; private set; }

        private void Start()
        {
            if (_targetWindow != null)
            {
                Room = _targetWindow.Room;
            }
            else
            {
                Debug.LogError(Utils.GetFieldNullString("Target window"));
            }
        }

        /// <summary>
        /// Spawns an enemy character.
        /// </summary>
        /// <param name="enemy">The enemy to spawn</param>
        public void SpawnEnemy(Enemy enemy)
        {
            //Debug.Log("Spawning enemy in " + name + ", " + Room.name);
            enemy.transform.position =
                new Vector3(transform.position.x, 0f, transform.position.z);
            enemy.InitVelocity();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.3f);

            if (_targetWindow != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, _targetWindow.transform.position);
            }
        }
    }
}
