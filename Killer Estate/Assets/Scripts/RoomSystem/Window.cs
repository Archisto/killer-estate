using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Window : LevelObject
    {
        [SerializeField]
        private HardwareBase _nearestHardware;

        [SerializeField]
        private int _maxHealth = 5;

        private int _health;

        public bool Barricaded
        {
            get { return _health > 0; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            FullRepair();
        }

        public void TakeDamage(int damage)
        {
            if (_health > 0)
            {
                _health -= damage;
                if (_health <= 0)
                {
                    _health = 0;
                }
            }
        }

        public void Repair()
        {
            if (_health < _maxHealth)
            {
                _health++;
            }
        }

        public void FullRepair()
        {
            _health = _maxHealth;
        }

        public override void ResetObject()
        {
            FullRepair();
            base.ResetObject();
        }

        private void OnDrawGizmos()
        {
            if (_nearestHardware != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, _nearestHardware.transform.position);
            }
        }
    }
}
