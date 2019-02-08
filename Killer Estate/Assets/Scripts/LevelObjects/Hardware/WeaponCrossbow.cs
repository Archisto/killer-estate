using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class WeaponCrossbow : WeaponMouse
    {
        protected override void Aim()
        {
            _powerRatio = PowerRatio();
            UpdateRotation();
            UpdateTargetPosition();
        }

        protected override void UpdateTargetPosition()
        {
            float range = Mathf.Lerp(_minRange, _maxRange, _powerRatio);
            _targetPosition = _projectileLaunchPoint.position + _lookVector * range;
            _targetAcquired = true;
        }

        protected override bool CanFire()
        {
            return MouseReachesMinDragRange();
        }

        protected override void Fire()
        {
            HitScanProjectile projectile = _base.HardwareManager.GetProjectile();
            if (projectile != null)
            {
                projectile.Launch(_projectileLaunchPoint.position,
                                  _targetPosition,
                                  OnHit,
                                  0.4f);
            }
        }
    }
}
