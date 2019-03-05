using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class LookAtMovingDir : MonoBehaviour
    {
        [SerializeField]
        private bool _lockX;

        [SerializeField]
        private bool _lockY;

        [SerializeField]
        private bool _lockZ;

        private Vector3 _oldPosition;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _oldPosition = transform.position;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.GamePaused)
            {
                UpdateRotation();
            }
        }

        private void UpdateRotation()
        {
            if (transform.position != _oldPosition)
            {
                Vector3 newRotation = (transform.position - _oldPosition).normalized;

                if (_lockX)
                {
                    newRotation.x = transform.rotation.eulerAngles.x;
                }
                if (_lockY)
                {
                    newRotation.y = transform.rotation.eulerAngles.y;
                }
                if (_lockZ)
                {
                    newRotation.z = transform.rotation.eulerAngles.z;
                }

                transform.rotation = Quaternion.LookRotation(newRotation, Vector3.up);
            }

            _oldPosition = transform.position;
        }
    }
}
