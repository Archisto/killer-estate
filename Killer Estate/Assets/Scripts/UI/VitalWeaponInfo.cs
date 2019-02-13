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

        [SerializeField, Range(1f, 1.5f)]
        private float _dangerIconMaxScale = 1.1f;

        [SerializeField, Range(0.1f, 1.5f)]
        private float _dangerIconScaleChangeDuration = 1f;

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
        private Timer _dangerIconPulsatingTimer;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _dangerIconPulsatingTimer = new Timer(_dangerIconScaleChangeDuration, false);
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (_dangerIconPulsatingTimer.Active)
            {
                UpdateWarningIconScale();

                if (_dangerIconPulsatingTimer.Check())
                {
                    _dangerIconPulsatingTimer.Activate();
                }
            }
        }

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

        private void UpdateWarningIconScale()
        {
            float ratio = _dangerIconPulsatingTimer.GetRatio();
            float scale;
            if (ratio < 0.5f)
            {
                scale = Mathf.Lerp(1f, _dangerIconMaxScale, ratio * 2);
            }
            else
            {
                scale = Mathf.Lerp(_dangerIconMaxScale, 1f, ratio * 2 - 1f);
            }

            Vector3 newScale = new Vector3(scale, scale, 1f);
            _sideIcon.transform.localScale = newScale;
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
            ResetDangerIconPulsating();
            _sideIcon.color = _selectionIconColor;
            ShowSideIcon(_selectionIcon);
        }

        public void ShowDangerIcon()
        {
            StartDangerIconPulsating();
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
            ResetDangerIconPulsating();
            _sideIcon.gameObject.SetActive(false);
        }

        private void StartDangerIconPulsating()
        {
            if (!_dangerIconPulsatingTimer.Active)
            {
                _dangerIconPulsatingTimer.Activate();
            }
        }

        private void ResetDangerIconPulsating()
        {
            if (_dangerIconPulsatingTimer != null
                && (_dangerIconPulsatingTimer.Active ||
                    _dangerIconPulsatingTimer.Finished))
            {
                _dangerIconPulsatingTimer.Reset();
                _sideIcon.transform.localScale = Vector3.one;
            }
        }
    } 
}
