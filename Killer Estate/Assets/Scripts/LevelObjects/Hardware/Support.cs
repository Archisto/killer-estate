using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public abstract class Support : Hardware
    {
        [Header("Support Hardware Stats")]

        [SerializeField]
        protected int _maxAmmo = 10;

        [SerializeField]
        protected float _effectRate = 0.3f;

        [SerializeField]
        protected float _minRange = 0.5f;

        [SerializeField]
        protected float _maxRange = 5f;

        protected Timer _effectTimer;
        protected List<Transform> _targets;

        protected override void Start()
        {
            base.Start();
            _effectTimer = new Timer(_effectRate, true);
            _targets = new List<Transform>();
            _needsAmmo = _maxAmmo > 0;

            if (_rotatingObj == null)
            {
                _rotatingObj = gameObject;
            }

            MakeReady();
        }

        public virtual void MakeReady()
        {
            _active = true;
            _effectTimer.Reset();

            // TODO: Check how much ammo is left.
            _ammo = _maxAmmo;
        }

        protected override void UpdateObject()
        {
            UpdateSupport();
            base.UpdateObject();
        }

        protected virtual void UpdateSupport()
        {
            if (CanUseEffect())
            {
                TryToUseEffect();
            }   
        }

        protected abstract bool CanUseEffect();

        protected virtual bool TryToUseEffect()
        {
            bool canUseEffect = !_needsAmmo;
            if (_needsAmmo && _ammo > 0)
            {
                canUseEffect = true;
                _ammo--;
                if (_ammo <= 0)
                {
                    _ammo = 0;
                }
            }

            if (canUseEffect)
            {
                UseEffect();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract void UseEffect();

        public virtual void CancelOperation()
        {
            ReleaseMouse(false);
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
                // TODO
                Gizmos.color = Color.white;
            }
        }

        protected virtual void DrawActiveGizmos()
        {
            if (_clickAreaCenter == null)
            {
                return;
            }

            // TODO
            DrawAimingGizmos();
        }

        protected virtual void DrawAimingGizmos()
        {
            if (_targets.Count > 0)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform target in _targets)
                {
                    Gizmos.DrawLine(_clickAreaCenter.position, target.position);
                }
            }
        }
    }
}
