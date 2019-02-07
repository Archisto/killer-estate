using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    /// <summary>
    /// A projectile which deals damage to enemies.
    /// </summary>
    public class HitScanProjectile : LevelObject
    {
        [SerializeField]
        private float _flyTime = 1f;

        private Timer _lifeTimer;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Action<Vector3> _hitCallBack;
        private bool _initialized;
        private LayerMask _mask;

        public int Damage { get; private set; }
        private bool Flying { get { return _lifeTimer.Active; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            //_mask =
            _lifeTimer = new Timer(_flyTime, true);
            _initialized = true;
        }

        public void Launch(int damage,
                           Vector3 startPosition,
                           Vector3 targetPosition,
                           Action<Vector3> hitCallBack)
        {
            if (!_initialized)
            {
                Init();
            }

            Damage = damage;
            _hitCallBack = hitCallBack;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            gameObject.SetActive(true);
            _lifeTimer.Activate();

            Vector3 flightVector = _targetPosition - _startPosition;
            //Ray ray = new Ray(_startPosition, flightVector.normalized);
            RaycastHit[] hits = Physics.RaycastAll
                (_startPosition, flightVector.normalized, flightVector.magnitude);
            CheckHits(hits);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Flying)
            {
                UpdateFlight();
            }

            base.UpdateObject();
        }

        private void UpdateFlight()
        {
            Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, _lifeTimer.GetRatio());
            transform.position = newPosition;

            if (_lifeTimer.Check())
            {
                _hitCallBack(_targetPosition);
                _lifeTimer.Reset();
                DestroyObject();
            }
        }

        private void CheckHits(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.activeSelf)
                {
                    Hit(hit);
                }
                return;
            }
        }

        /// <summary>
        /// Called when the projectile collides with something.
        /// </summary>
        /// <param name="hit">Hit info</param>
        private void Hit(RaycastHit hit)
        {
            _targetPosition = hit.point;
            IDamageReceiver dmgRec = hit.transform.GetComponent<IDamageReceiver>();
            if (dmgRec != null)
            {
                dmgRec.TakeDamage(Damage);
            }
            _hitCallBack(_targetPosition);
        }

        public override void DestroyObject()
        {
            Destroyed = true;
            gameObject.SetActive(false);
            base.DestroyObject();
        }

        public override void ResetObject()
        {
            Destroyed = false;
            gameObject.SetActive(false);
            base.ResetObject();
        }
    } 
}
