using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Enemy : LevelObject, IDamageReceiver
    {
        public int _maxHealth = 10;
        public int _health;
        public int _damage = 10;
        public float _speed = 3f;
        public int _scoreReward = 10;

        private Move _move;
        private Hardware _target;
        private bool _idle;

        public bool Dead { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            Init();

            if (!_idle)
            {
                InitVelocity();
            }
        }

        private void Init()
        {
            _health = _maxHealth;
            _move = GetComponent<Move>();
            FindTarget();
        }

        private void FindTarget()
        {
            _target = FindObjectOfType<Hardware>();
            _idle = (_target == null);
        }

        public void InitVelocity()
        {
            if (_target == null)
            {
                Init();
            }

            if (!_idle)
            {
                Vector3 velocity = _speed * (_target.transform.position - transform.position).normalized;
                velocity.y = 0f;
                _move.TopSpeed = velocity;
                _move.StartMoving(true);
            }
        }

        protected override void UpdateObject()
        {
            if (_target == null)
            {
                FindTarget();
                InitVelocity();
            }
            else if (!_idle)
            {
                if (Vector3.Distance(_target.transform.position, transform.position) < 2f)
                {
                    _target.TakeDamage(_damage);
                    DestroyObject();
                }
            }

            base.UpdateObject();
        }

        public void TakeDamage(int amount)
        {
            if (Dead)
            {
                return;
            }

            _health -= amount;
            if (_health <= 0)
            {
                Dead = true;
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
            Dead = false;
            _idle = false;
            _health = _maxHealth;
            gameObject.SetActive(false);
            base.ResetObject();
        }
    }
}
