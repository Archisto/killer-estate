using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KillerEstate.UI
{
    public class VitalWeaponInfo : MonoBehaviour
    {
        [SerializeField]
        private Color _vitalPortraitColor = Color.yellow;

        [SerializeField]
        private Color _normalPortraitColor = Color.gray;

        [SerializeField]
        private Color _healthGoodColor = Color.green;

        [SerializeField]
        private Color _healthBadColor = Color.red;

        [SerializeField]
        private Color _selectionIconColor = Color.blue;

        [SerializeField, Range(0.05f, 0.95f)]
        private float _badHealthPercentageThreshold = 0.5f;

        [SerializeField]
        private Text _healthText;

        [SerializeField]
        private Text _roomText;

        [SerializeField]
        private Image _weaponIcon;

        [SerializeField]
        private Image _sideIcon;

        [SerializeField]
        private Sprite _selectionIcon;

        [SerializeField]
        private Sprite _dangerIcon;

        public WeaponMouse Weapon { get; private set; }

        private bool _showSelectionIcon;
        private bool _showDangerIcon;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        //private void Start()
        //{
        //}

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        //private void Update()
        //{
        //    if (!GameManager.Instance.GamePaused)
        //    {
        //        if (_weapon != null)
        //        {
        //            UpdateWeaponInfo();
        //        }
        //    }
        //}

        public void UpdateHealth(int health)
        {
            if (Weapon != null)
            {
                _healthText.text = string.Format("{0} / {1}",
                    health, Weapon.MaxHealth);

                float healthRatio = (float) health / Weapon.MaxHealth;
                if (healthRatio < _badHealthPercentageThreshold)
                {
                    _healthText.color = _healthBadColor;
                }
                else
                {
                    _healthText.color = _healthGoodColor;
                }
            }
        }

        public void SetWeapon(WeaponMouse weapon)
        {
            Weapon = weapon;
            if (weapon != null)
            {
                UpdateHealth(Weapon.Health);
                UpdateWeaponIcon();
                _roomText.text = Weapon.Room.name;
                UpdatePortraitColor(true);
            }
        }

        private void UpdateWeaponIcon()
        {
            // TODO
            //_weaponIcon.sprite = _weapon.sprite;
        }

        public void UpdatePortraitColor(bool vitalWeapon)
        {
            _weaponIcon.color = (vitalWeapon ?
                _vitalPortraitColor : _normalPortraitColor);
        }

        public void ShowSelectionIcon()
        {
            _sideIcon.color = _selectionIconColor;
            ShowSideIcon(_selectionIcon);
        }

        public void ShowDangerIcon()
        {
            _sideIcon.color = Color.white;
            ShowSideIcon(_dangerIcon);
        }

        private void ShowSideIcon(Sprite icon)
        {
            if (icon != null)
            {
                _sideIcon.sprite = icon;
                _sideIcon.gameObject.SetActive(true);
            }
            else
            {
                _sideIcon.gameObject.SetActive(false);
            }
        }

        public void HideSideIcon()
        {
            _sideIcon.gameObject.SetActive(false);
        }
    } 
}
