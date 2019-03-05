using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Window : InteractableObject
    {
        [SerializeField]
        private HardwareBase _nearestHardware;

        [SerializeField]
        private int _maxHealth = 5;

        [SerializeField]
        private float _repairRate = 1f;

        private Timer _repairTimer;
        private int _health;
        private Vector3[] hpPositions;

        public bool Barricaded
        {
            get { return _health > 0; }
        }

        public bool Repairing
        {
            get { return _repairTimer.Active; }
        }

        private bool RepairDone
        {
            get { return _repairTimer.Finished; }
        }

        private float RepairProgress
        {
            get { return _repairTimer.GetRatio(); }
        }

        public override bool Interactable
        {
            get { return _health < _maxHealth; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _repairTimer = new Timer(_repairRate, true);
            //FullRepair(); // Testing before enemies can damage windows
            hpPositions = Utils.GetPointsInRing(transform.position, 1f, _maxHealth, _maxHealth, Vector3.up, 1f);
        }

        protected override void UpdateInteraction()
        {
            if (WithinClickRange(_mouse.Position))
            {
                Repair();
            }
            else if (Repairing)
            {
                _repairTimer.Reset();
            }
        }

        public void TakeDamage(int damage)
        {
            if (_health > 0)
            {
                _health -= damage;
                if (_health <= 0)
                {
                    _health = 0;
                }
            }
        }

        public void Repair()
        {
            if (Repairing || RepairDone)
            {
                if (_health < _maxHealth)
                {
                    if (_repairTimer.Check())
                    {
                        _health++;
                        _repairTimer.Reset();
                    }
                }
                else
                {
                    _repairTimer.Reset();
                }
            }
            else 
            {
                _repairTimer.Activate();
            }
        }

        public void FullRepair()
        {
            _health = _maxHealth;
            _repairTimer.Reset();
        }

        protected override void OnClick()
        {
        }

        public override void ResetObject()
        {
            FullRepair();
            base.ResetObject();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (_nearestHardware != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, _nearestHardware.transform.position);
            }

            if (_repairTimer != null)
            {
                if (Barricaded)
                {
                    Gizmos.color = Color.green;
                    for (int i = 0; i < _health; i++)
                    {
                        Gizmos.DrawSphere(hpPositions[i], 0.1f);
                    }
                }
                else if (!Repairing)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position,
                        transform.position + Vector3.up * 3f);
                }

                if (Repairing)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(transform.position,
                        transform.position + Vector3.up * 3f * RepairProgress);
                }
            }
        }
    }
}
