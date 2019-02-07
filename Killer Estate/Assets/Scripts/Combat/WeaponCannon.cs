using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    /// <summary>
    /// The weapon's power is determined by its charge, and charge is
    /// filled by clicking on the weapon before dragging and releasing to fire.
    /// </summary>
    public class WeaponCannon : WeaponMouse
    {
        [Header("Cannon Specific")]

        [SerializeField]
        private float _minChargeToFire = 0.5f;

        [SerializeField]
        private float _chargeDepletionRate = 0.3f;

        private Timer _chargeDepletionTimer;

        protected override void Start()
        {
            base.Start();
            _chargeDepletionTimer = new Timer(_chargeDepletionRate, true);
        }

        protected override void UpdateObject()
        {
            base.UpdateObject();
            UpdateChargeDepletion();
        }

        private void UpdateChargeDepletion()
        {
            if (Charge > 0f && _chargeDepletionTimer.Check())
            {
                DepleteCharge(false);
                if (Charge > 0f)
                {
                    _chargeDepletionTimer.Activate();
                }
            }
        }

        protected override void OnLeftMouseButtonDown()
        {
            base.OnLeftMouseButtonDown();
            _powerRatio = PowerRatio();
        }

        protected override bool ReleaseMouse(bool tryToFire)
        {
            _mouseButtonHeld = false;
            if (_active)
            {
                _active = false;

                if (!_targetAcquired)
                {
                    FillCharge(false);
                    if (!_chargeDepletionTimer.Active)
                    {
                        _chargeDepletionTimer.Activate();
                    }
                }
                else if (tryToFire && MouseReachesMinDragRange())
                {
                    return TryToFire();
                }

                _targetAcquired = false;
            }

            return false;
        }

        protected override void UpdateTargetPosition()
        {
            float range = Mathf.Lerp(_minRange, _maxRange, _powerRatio);
            _targetPosition = _projectileLaunchPoint.position + _lookVector * range;
            _targetAcquired = true;
        }

        protected override bool CanAim()
        {
            return MouseReachesMinDragRange();
        }

        protected override bool CanFire()
        {
            return Charge > _minChargeToFire;
        }

        protected override float PowerRatio()
        {
            return Charge;
        }

        protected override void Fire()
        {
            Projectile projectile = _projectiles.GetPooledObject();
            if (projectile != null)
            {
                projectile.transform.position = _projectileLaunchPoint.position;
                projectile.SetDamage(GetDamage());
                projectile.SetForce(12f);
                projectile.Init(OnHit);
                projectile.Launch(_lookVector);
            }

            DepleteCharge(true);
        }

        public override void CancelOperation()
        {
            base.CancelOperation();
            DepleteCharge(true);
        }

        protected override void DrawActiveGizmos()
        {
            base.DrawActiveGizmos();

            if (_powerRatio > 0.95f)
            {
                Gizmos.color = Color.green;
            }
            else if (!CanFire())
            {
                Gizmos.color = Color.black;
            }
            else
            {
                Gizmos.color = Color.Lerp(Color.white, Color.red, _powerRatio);
            }
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3f);
        }
    }
}
