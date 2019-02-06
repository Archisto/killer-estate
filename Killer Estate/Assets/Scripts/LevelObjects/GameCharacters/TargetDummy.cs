using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class TargetDummy : LevelObject, IDamageReceiver
    {
        public int _maxHealth = 10;
        public int _health;
        public float _speed = 3f;
        public int _scoreReward = 10;

        private Move _move;
        private MouseFiring _playerWeapon;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            Init();
            InitVelocity();
        }

        private void Init()
        {
            _health = _maxHealth;
            _move = GetComponent<Move>();
            _playerWeapon = FindObjectOfType<MouseFiring>();
        }

        public void InitVelocity()
        {
            if (_playerWeapon == null)
            {
                Init();
            }

            Vector3 velocity = _speed * (_playerWeapon.transform.position - transform.position).normalized;
            velocity.y = 0f;
            _move.TopSpeed = velocity;
            _move.StartMoving(true);
        }

        protected override void UpdateObject()
        {
            if (Vector3.Distance(_playerWeapon.transform.position, transform.position) < 1f)
            {
                DestroyObject();
            }

            base.UpdateObject();
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                Debug.Log(name + " was killed with " + amount + " damage");
                GameManager.Instance.ChangeScore(_scoreReward);
                DestroyObject();
            }
            else
            {
                Debug.Log(name + " took " + amount + " damage");
            }
        }

        public override void DestroyObject()
        {
            Destroyed = true;
            gameObject.SetActive(false);
            base.DestroyObject();
        }

        public override void ResetObject()
        {
            _health = _maxHealth;
            gameObject.SetActive(false);
            base.ResetObject();
        }
    }
}
