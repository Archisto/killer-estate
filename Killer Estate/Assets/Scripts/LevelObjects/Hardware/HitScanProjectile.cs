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
        [SerializeField, Range(0.1f, 1f)]
        private float _projectileWidth = 0.2f;

        [SerializeField, Range(0.1f, 1f)]
        private float _defaultLifeTime = 0.5f;

        private float _lifeTime;
        private Timer _lifeTimer;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private Action<Vector3> _hitCallBack;
        private bool _initialized;
        protected LineRenderer _projectileLine;

        public int Damage { get; private set; }
        private bool Active { get { return _lifeTimer.Active; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            _projectileLine = GetComponent<LineRenderer>();
            _initialized = true;
        }

        public void Launch(int damage,
                           Vector3 startPosition,
                           Vector3 targetPosition,
                           Action<Vector3> hitCallBack,
                           float lifeTime = 0f)
        {
            _lifeTime = (lifeTime > 0f ? lifeTime : _defaultLifeTime);
            _lifeTimer = new Timer(_lifeTime, true);

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
            //RaycastHit[] hits = Physics.SphereCastAll
            //    (_startPosition, _projectileWidth, flightVector.normalized, flightVector.magnitude);
            CheckHits(hits);
            _projectileLine.SetPosition(0, _startPosition);
            _projectileLine.SetPosition(1, _targetPosition);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Active)
            {
                UpdateTrail();
            }

            base.UpdateObject();
        }

        private void UpdateTrail()
        {
            UpdateTrailAlpha();

            if (_lifeTimer.Check())
            {
                _hitCallBack(_targetPosition);
                _lifeTimer.Reset();
                DestroyObject();
            }
        }

        private void UpdateTrailAlpha()
        {
            GradientAlphaKey[] alpha = _projectileLine.colorGradient.alphaKeys;
            for (int i = 0; i < alpha.Length; i++)
            {
                alpha[i].alpha = 1f - _lifeTimer.GetRatio();
            }
            Gradient newGradient = new Gradient();
            newGradient.SetKeys(
                _projectileLine.colorGradient.colorKeys,
                alpha
            );
            _projectileLine.colorGradient = newGradient;
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
