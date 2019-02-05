using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void LateUpdate()
        {
            if (GameManager.Instance.PlayReady)
            {
                // TODO
            }
        }

        private void UpdatePositionBetweenObjects(List<LevelObject> levelObjects)
        {
            Vector3 newPosition = Vector3.zero;
            int intactObjects = 0;

            for (int i = 0; i < levelObjects.Count; i++)
            {
                if (!levelObjects[i].Destroyed)
                {
                    newPosition += levelObjects[i].transform.position;
                    intactObjects++;
                }
            }

            if (intactObjects > 0)
            {
                newPosition = newPosition / intactObjects;
                newPosition.y = _startPosition.y;
                transform.position = newPosition;
            }
        }

        public Vector3 GetCameraViewPosition(Vector3 camPosOffset)
        {
            Vector3 worldOffset = transform.right * camPosOffset.x +
                                  transform.up * camPosOffset.y +
                                  transform.forward * camPosOffset.z;
            return transform.position + worldOffset;
        }

        public Quaternion GetRotationTowardsCamera()
        {
            // TODO: Quaternion.Inverse?

            return Quaternion.Euler(
                transform.rotation.eulerAngles.x * -1,
                transform.rotation.eulerAngles.y + 180,
                transform.rotation.eulerAngles.z * -1);
        }
    }
}
