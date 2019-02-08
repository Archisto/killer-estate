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
        protected const string EnvironmentKey = "Environment";
        protected const string EnemyKey = "Enemy";

        [SerializeField, Range(0.1f, 1f)]
        private float _projectileWidth = 0.2f; // TODO: Use this

        [SerializeField, Range(0.1f, 1f)]
        private float _defaultLifeTime = 0.5f;

        protected float _lifeTime;
        protected Timer _lifeTimer;
        protected Vector3 _startPosition;
        protected Vector3 _targetPosition;
        protected Action<GameObject, Vector3> _hitCallBack;
        protected bool _initialized;
        protected bool _hit;
        protected LineRenderer _projectileLine;

        private bool Active { get { return _lifeTimer.Active; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Init()
        {
            _projectileLine = GetComponent<LineRenderer>();
            _initialized = true;
        }

        public virtual void Launch(Vector3 startPosition,
                                   Vector3 targetPosition,
                                   Action<GameObject, Vector3> hitCallBack,
                                   float lifeTime = 0f)
        {
            _lifeTime = (lifeTime > 0f ? lifeTime : _defaultLifeTime);
            _lifeTimer = new Timer(_lifeTime, true);

            if (!_initialized)
            {
                Init();
            }

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

        protected virtual void UpdateTrail()
        {
            UpdateTrailAlpha();

            if (_lifeTimer.Check())
            {
                if (!_hit)
                {
                    _hitCallBack(null, _targetPosition);
                }
                _hit = false;
                _lifeTimer.Reset();
                DestroyObject();
            }
        }

        protected virtual void UpdateTrailAlpha()
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

        protected virtual bool CheckHits(RaycastHit[] hits)
        {
            if (hits.Length > 0)
            {
                bool hitSomething = false;
                RaycastHit firstValidHit = hits[0];
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject.activeSelf)
                    {
                        // Checks if the projectile hit a primary target
                        // and if so, sends info about it to the weapon
                        // and ends the loop
                        if (PrimaryTarget(hit.transform))
                        {
                            Hit(hit);
                            return true;
                        }

                        // Checks if the projectile hit a non-primary target;
                        // only the first hit is acknowledged
                        else if (!hitSomething
                                 && ValidTarget(hit.transform))
                        {
                            firstValidHit = hit;
                            hitSomething = true;
                        }
                    }
                }

                if (hitSomething)
                {
                    Hit(firstValidHit);
                    return true;
                }
            }

            return false;
        }

        protected virtual bool PrimaryTarget(Transform target)
        {
            return Utils.IsOnLayer(target.gameObject, EnemyKey);
        }

        protected virtual bool ValidTarget(Transform target)
        {
            return Utils.IsOnLayer(target.gameObject, EnvironmentKey);
        }

        /// <summary>
        /// Called when the projectile collides with something.
        /// </summary>
        /// <param name="hit">Hit info</param>
        protected virtual void Hit(RaycastHit hit)
        {
            _hit = true;
            _targetPosition = hit.point;
            _hitCallBack(hit.transform.gameObject, _targetPosition);
        }

        public override void DestroyObject()
        {
            _hit = false;
            Destroyed = true;
            _lifeTimer.Reset();
            gameObject.SetActive(false);
            base.DestroyObject();
        }

        public override void ResetObject()
        {
            _hit = false;
            Destroyed = false;
            if (_lifeTimer != null)
            {
                _lifeTimer.Reset();
            }
            gameObject.SetActive(false);
            base.ResetObject();
        }
    } 
}
