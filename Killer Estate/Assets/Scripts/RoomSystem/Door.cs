using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Door : LevelObject
    {
        [SerializeField]
        private Room _room1;

        [SerializeField]
        private Room _room2;

        [SerializeField]
        private int _unlockCost = 100;

        [SerializeField]
        protected float _mouseClickRange = 1f;

        private MouseController _mouse;

        public bool Unlocked { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _mouse = GameManager.Instance.Mouse;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Clicked())
            {
                // TODO: Unlocking
                EnterOtherRoom();
            }

            base.UpdateObject();
        }

        protected virtual bool Clicked()
        {
            return (_mouse.LeftButtonReleased
                    && MouseWithinClickRange());
        }

        protected bool MouseWithinClickRange()
        {
            return (Vector3.Distance(_mouse.Position, transform.position) <= _mouseClickRange);
        }

        public void EnterOtherRoom()
        {
            if (_room1 != null && _room2 != null)
            {
                Room currRoom = GameManager.Instance.CurrentRoom;
                if (currRoom == _room1)
                {
                    _room2.Enter();
                }
                else if (currRoom == _room2)
                {
                    _room1.Enter();
                }
                else
                {
                    Debug.LogError("The door can't be used from this room.");
                }
            }
            else
            {
                Debug.LogError("One or both rooms are null.");
            }
        }

        public void TryUnlock()
        {
            if (!Unlocked)
            {
                if (GameManager.Instance.ChangeScore(-1 * _unlockCost))
                {
                    Unlocked = true;
                }
                else
                {
                    Debug.LogWarning("Not enough score to unlock.");
                }
            }
            else
            {
                Debug.LogWarning("Already unlocked.");
            }
        }

        public override void ResetObject()
        {
            Unlocked = false;
            base.ResetObject();
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}
