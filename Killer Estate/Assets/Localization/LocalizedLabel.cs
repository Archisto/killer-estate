using UnityEngine;
using UnityEngine.UI;
using L10n = KillerEstate.Localization.Localization;

namespace KillerEstate.Localization
{
    public class LocalizedLabel : MonoBehaviour
    {
        [SerializeField]
        private string key;

        private string localizedString;

        public Text Text { get; set; }

        private void Awake()
        {
            Text = GetComponent<Text>();

            L10n.LanguageLoaded += OnLanguageLoaded;
            OnLanguageLoaded();
        }

        private void OnLanguageLoaded()
        {
            localizedString = L10n.CurrentLanguage.GetTranslation(key);
            if (Text != null)
            {
                Text.text = localizedString;
            }
        }

        public void FormatString(params object[] insertedObjects)
        {
            if (Text != null)
            {
                Text.text = string.Format(localizedString, insertedObjects);
            }
        }
    }
}
