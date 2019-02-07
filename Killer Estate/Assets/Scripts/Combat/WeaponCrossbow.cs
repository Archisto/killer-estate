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
            Projectile projectile = _projectiles.GetPooledObject();
            if (projectile != null)
            {
                projectile.transform.position = _projectileLaunchPoint.position;
                projectile.SetDamage(GetDamage());
                projectile.SetForce(12f);
                projectile.Init(OnHit);
                projectile.Launch(_lookVector);
            }
        }
    }
}
