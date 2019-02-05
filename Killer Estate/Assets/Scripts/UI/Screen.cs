using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KillerEstate.UI
{
    public class Screen : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _extraScreens;

        /// <summary>
        ///  Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        ///  Updates the object once per frame.
        /// </summary>
        protected virtual void Update()
        {
        }

        public virtual void Activate(bool active)
        {
            CloseExtraScreens();
            gameObject.SetActive(active);
        }

        public void CloseExtraScreens()
        {
            if (_extraScreens != null)
            {
                foreach (GameObject go in _extraScreens)
                {
                    if (go.activeSelf)
                    {
                        go.SetActive(false);
                    }
                }
            }
        }
    }
}
