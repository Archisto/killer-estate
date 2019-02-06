using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class MouseFiring : MonoBehaviour
    {
        [SerializeField]
        private float _mouseRange = 1f;

        [SerializeField]
        private float _projectileRange = 5f;

        [SerializeField]
        private float _minMouseDist = 0.5f;

        [SerializeField]
        private float _maxMouseDistStrength = 2f;

        [SerializeField]
        private int _minDamage = 1;

        [SerializeField]
        private int _maxDamage = 5;

        [SerializeField]
        private float _baseLaunchForce = 12f;

        [SerializeField]
        private Projectile _projectilePrefab;

        [SerializeField]
        private Transform _projectileLaunchPoint;

        [SerializeField]
        private Transform _clickAreaCenter;

        [SerializeField]
        private GameObject _rotatingObj;

        [SerializeField]
        private GameObject _testObj;

        private Vector3 _mousePosition;
        private Vector3 _mousePositionOnButtonDown;
        private Vector3 _lookVector;
        private float _mouseDist;
        private float _strengthRatio;
        private bool _active;
        private bool _mouseButtonHeld;
        private Pool<Projectile> _projectiles;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _projectiles = new Pool<Projectile>(5, false, _projectilePrefab);

            if (_rotatingObj == null)
            {
                _rotatingObj = gameObject;
            }
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.GamePaused)
            {
                AimAndFire();
            }
        }

        private bool AimAndFire()
        {
            _mousePosition = Input.mousePosition;
            _mousePosition.z = 10f + 0.01f * _mousePosition.y;
            _mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
            _mousePosition.y = transform.position.y;

            bool mouseButtonDownOnWeapon = (!_mouseButtonHeld && CursorOnWeapon());

            if (Input.GetMouseButton(0))
            {
                if (!_mouseButtonHeld)
                {
                    _mouseButtonHeld = true;
                    _mousePositionOnButtonDown = _mousePosition;
                }

                //_testObj.transform.position = _mousePosition; // testing
            }
            else
            {
                _mouseButtonHeld = false;
                if (_active)
                {
                    _active = false;

                    if (_mouseDist > _minMouseDist)
                    {
                        Fire();
                        return true;
                    }
                }
            }

            if (_mouseButtonHeld && (_active || mouseButtonDownOnWeapon))
            {
                _active = true;

                _lookVector = _mousePositionOnButtonDown - _mousePosition;
                _mouseDist = _lookVector.magnitude;
                if (_lookVector != Vector3.zero)
                {
                    _strengthRatio = StrengthRatio();
                    _lookVector.Normalize();
                    Quaternion newRotation = Quaternion.LookRotation(_lookVector);
                    newRotation.x = _rotatingObj.transform.rotation.x;
                    newRotation.z = _rotatingObj.transform.rotation.z;
                    _rotatingObj.transform.rotation = newRotation;
                }
            }

            return false;
        }

        private bool CursorOnWeapon()
        {
            return (Vector3.Distance(_mousePosition, _clickAreaCenter.position) < _mouseRange);
        }

        private float StrengthRatio()
        {
            return Mathf.Clamp01(_mouseDist / _maxMouseDistStrength);
        }

        private void Fire()
        {
            Projectile projectile = _projectiles.GetPooledObject();
            if (projectile != null)
            {
                int damage = (int) (_minDamage + (_maxDamage - _minDamage) * _strengthRatio);
                projectile.transform.position = _projectileLaunchPoint.position;
                projectile.SetDamage(damage);
                projectile.SetForce(_baseLaunchForce + _baseLaunchForce * _strengthRatio);
                projectile.Init(OnHit);
                projectile.Launch(_lookVector);
            }
        }

        private void OnHit(Projectile projectile)
        {
            //Debug.Log("Hit! Dealt " + projectile.GetDamage() + " dmg");
        }

        private void OnDrawGizmos()
        {
            if (!_active)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, _mouseRange);
                //Gizmos.DrawWireSphere(_clickAreaCenter.position, _mouseRange);
            }
            else
            {
                if (_mouseDist > _minMouseDist)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.red, _strengthRatio);
                    Vector3 targetPoint = transform.position + _lookVector * _mouseDist;
                    Gizmos.DrawLine(transform.position, targetPoint);

                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawLine(transform.position,
                    _mousePosition + transform.position - _mousePositionOnButtonDown);
            }
        }
    }
}
