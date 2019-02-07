using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public abstract class WeaponMouse : Hardware
    {
        [Header("Weapon Stats")]

        [SerializeField]
        protected int _maxAmmo = 10;

        [SerializeField]
        protected int _minDamage = 1;

        [SerializeField]
        protected int _maxDamage = 5;

        [SerializeField]
        protected float _fireRate = 0.3f;

        [SerializeField]
        protected float _reloadTime = 2f;

        [SerializeField]
        protected float _minRange = 0.5f;

        [SerializeField]
        protected float _maxRange = 5f;

        [Header("Projectiles")]

        [SerializeField]
        protected int _maxProjectileCount = 5;

        [SerializeField]
        protected Projectile _projectilePrefab;

        [SerializeField]
        protected Transform _projectileLaunchPoint;

        protected bool _reloading;
        protected int _ammo;
        protected Pool<Projectile> _projectiles;
        protected Timer _reloadTimer;
        protected Vector3 _lookVector;
        protected float _powerRatio;
        protected Vector3 _targetPosition;
        protected bool _targetAcquired;
        protected bool _needsAmmo;

        protected override void Start()
        {
            base.Start();
            _projectiles = new Pool<Projectile>
                (_maxProjectileCount, false, _projectilePrefab);
            _reloadTimer = new Timer(_reloadTime, true);
            _needsAmmo = _maxAmmo > 0;

            if (_rotatingObj == null)
            {
                _rotatingObj = gameObject;
            }

            MakeReady();
        }

        public virtual void MakeReady()
        {
            _reloading = false;
            _reloadTimer.Reset();

            // TODO: Check how much ammo is left.
            _ammo = _maxAmmo;
        }

        protected override void UpdateObject()
        {
            if (_reloading)
            {
                UpdateReload();
            }
            else
            {
                UpdateWeapon();
            }

            base.UpdateObject();
        }

        protected virtual void UpdateWeapon()
        {
            bool mouseHoveredOnWeapon = UpdateMouse();
            if (_mouseButtonHeld && (_active || mouseHoveredOnWeapon))
            {
                UpdateAim();
            }
        }

        protected override void CheckIfMouseLeftButtonDown()
        {
            if (Input.GetMouseButton(0))
            {
                OnLeftMouseButtonDown();
            }
            else
            {
                ReleaseMouse(CanFire());
            }
        }

        protected override bool ReleaseMouse(bool tryToFire)
        {
            _mouseButtonHeld = false;
            if (_active)
            {
                _active = false;

                if (tryToFire)
                {
                    return TryToFire();
                }
            }

            _targetAcquired = false;
            return false;
        }

        protected virtual void UpdateAim()
        {
            _active = true;

            _lookVector = _mousePositionOnButtonDown - _mousePosition;
            _mouseDist = _lookVector.magnitude;
            if (CanAim())
            {
                Aim();
            }
        }

        protected virtual void Aim()
        {
            UpdateRotation();
            UpdateTargetPosition();
        }

        protected abstract void UpdateTargetPosition();

        protected virtual void UpdateRotation()
        {
            _lookVector.Normalize();
            Quaternion newRotation = Quaternion.LookRotation(_lookVector);
            newRotation.x = _rotatingObj.transform.rotation.x;
            newRotation.z = _rotatingObj.transform.rotation.z;
            _rotatingObj.transform.rotation = newRotation;
        }

        protected virtual void UpdateReload()
        {
            if (_reloadTimer.Check())
            {
                MakeReady();
            }
        }

        protected virtual bool CanAim()
        {
            return _lookVector != Vector3.zero;
        }

        protected virtual bool CanFire()
        {
            return _targetAcquired;
        }

        protected virtual bool TryToFire()
        {
            bool canFire = !_needsAmmo;
            if (_needsAmmo && _ammo > 0)
            {
                canFire = true;
                _ammo--;
                if (_ammo <= 0)
                {
                    _ammo = 0;
                }
            }

            if (canFire)
            {
                Fire();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual float PowerRatio()
        {
            return Mathf.Clamp01((_mouseDist - _minMouseDragDist)
                / (_maxMouseDragDist - _minMouseDragDist));
        }

        protected virtual int GetDamage()
        {
            return (int) (_minDamage + (_maxDamage - _minDamage) * _powerRatio);
        }

        protected abstract void Fire();

        protected virtual void OnHit(Projectile projectile)
        {
        }

        protected virtual void StartReloading()
        {
            CancelOperation();
            if (_reloadTime > 0f)
            {
                _reloading = true;
                _reloadTimer.Activate();
            }
        }

        public virtual void CancelOperation()
        {
            ReleaseMouse(false);
            _targetAcquired = false;
        }

        protected virtual void OnDrawGizmos()
        {
            if (!_active)
            {
                DrawInactiveGizmos();
            }
            else
            {
                DrawActiveGizmos();
            }
        }

        protected virtual void DrawInactiveGizmos()
        {
            if (_clickAreaCenter != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(_clickAreaCenter.position, _mouseClickRange);
                //Gizmos.DrawWireSphere(_clickAreaCenter.position, _mouseClickRange);
            }
        }

        protected virtual void DrawActiveGizmos()
        {
            if (_clickAreaCenter == null)
            {
                return;
            }

            // Line from weapon to mouse (offset by the mouse cursor's initial inaccuracy)
            Vector3 offsetMousePosition =
            _mousePosition + _clickAreaCenter.position - _mousePositionOnButtonDown;

            if (!MouseReachesMinDragRange())
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.Lerp(Color.white, Color.red, _powerRatio);
                Gizmos.DrawLine(_clickAreaCenter.position, _targetPosition);

                if (MouseOutsideDragRange())
                {
                    // Maximum drag distance position
                    offsetMousePosition = _clickAreaCenter.position - (_lookVector * _maxMouseDragDist);
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
            }

            Gizmos.DrawLine(_clickAreaCenter.position, offsetMousePosition);

            // The target position
            //if (_targetAcquired)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawLine(_targetPosition, _targetPosition + Vector3.up * 5);
            //}
        }
    }
}
