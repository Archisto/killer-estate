using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Door : InteractableObject
    {
        private enum AttachedRoom
        {
            None = 0,
            Room1 = 1,
            Room2 = 2
        }

        [Header("Door Specific")]

        [SerializeField]
        private Room _room1;

        [SerializeField]
        private Room _room2;

        [SerializeField]
        private int _unlockCost = 100;

        public bool Unlocked { get; private set; }

        public override bool Interactable
        {
            get
            {
                return _room1 != null && _room2 != null;
            }
        }

        protected override bool IsInCurrentRoom()
        {
            return GameManager.Instance.CurrentRoom == _room1
                   || GameManager.Instance.CurrentRoom == _room2;
        }

        protected override void OnClick()
        {
            if (Interactable)
            {
                if (!Unlocked)
                {
                    TryUnlock();
                }
                else
                {
                    EnterOtherRoom();
                }
            }
            else
            {
                Debug.LogError("The door cannot be used! "
                    + "One or both rooms are unset.");
            }
        }

        private AttachedRoom GetCurrentAttachedRoom()
        {
            Room currRoom = GameManager.Instance.CurrentRoom;
            if (currRoom == _room1)
            {
                return AttachedRoom.Room1;
            }
            else if (currRoom == _room2)
            {
                return AttachedRoom.Room2;
            }
            else
            {
                return AttachedRoom.None;
            }
        }

        public void EnterOtherRoom()
        {
            AttachedRoom currRoom = GetCurrentAttachedRoom();
            if (currRoom == AttachedRoom.Room1)
            {
                _room2.Enter();
            }
            else if (currRoom == AttachedRoom.Room2)
            {
                _room1.Enter();
            }
            else
            {
                Debug.LogError("The door can't be used from this room.");
            }
        }

        public void TryUnlock()
        {
            if (!Unlocked)
            {
                if (GameManager.Instance.ChangeScore(-1 * _unlockCost))
                {
                    Unlocked = true;

                    AttachedRoom currRoom = GetCurrentAttachedRoom();
                    if (currRoom == AttachedRoom.Room1)
                    {
                        _room2.Open = true;
                    }
                    else if (currRoom == AttachedRoom.Room2)
                    {
                        _room1.Open = true;
                    }
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

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (Interactable && Unlocked)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, 1f);
            }
        }
    }
}
