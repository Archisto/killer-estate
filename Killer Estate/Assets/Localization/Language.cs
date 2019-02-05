using System;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate.Localization
{
    /// <summary>
    /// Stores key and value pairs of a language.
    /// </summary>
    [Serializable]
    public class Language
    {
        [SerializeField]
        private List<string> keys = new List<string>();

        [SerializeField]
        private List<string> values = new List<string>();

        [SerializeField]
        private LangCode langCode;

        /// <summary>
        /// The language's code.
        /// </summary>
        public LangCode LanguageCode
        {
            get
            {
                return langCode;
            }
            set
            {
                langCode = value;
            }
        }

        /// <summary>
        /// Initializes the language.
        /// </summary>
        /// <param name="language">A language code</param>
        public Language(LangCode language)
        {
            LanguageCode = language;

            if (language != LangCode.None)
            {
                Debug.Log("Language created and initialized");
            }
            else
            {
                Debug.LogWarning("Language is invalid");
            }
        }

        /// <summary>
        /// Gets a translation value that corresponds to the given key.
        /// </summary>
        /// <param name="key">A translation key</param>
        /// <returns>A translation value</returns>
        public string GetTranslation(string key)
        {
            string result = "";

            if (!string.IsNullOrEmpty(key))
            {
                // Gets the index of the key (if it exists)
                int index = keys.IndexOf(key);

                // If the index is valid, sets the corresponding
                // value to the result
                if (index >= 0)
                {
                    result = values[index];
                }
                else
                {
                    // TODO: Search key in the default language: Lang EN

                    Debug.LogError("Key '" + key + "' doesn't exist in " + this);
                }
            }
            else
            {
                Debug.LogError("Key is null or empty");
            }

            return result;
        }

        /// <summary>
        /// Gets all of the language's key and value pairs.
        /// </summary>
        /// <returns>Language key and value pairs</returns>
        public Dictionary<string, string> GetValues()
        {
            var result = new Dictionary<string, string>();

            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Sets the language's key and value pairs.
        /// </summary>
        /// <param name="translations">Language key and value pairs</param>
        public void SetValues(Dictionary<string, string> translations)
        {
            // Clears the lists before adding new values
            Clear();

            foreach (var translation in translations)
            {
                keys.Add(translation.Key);
                values.Add(translation.Value);
            }
        }

        /// <summary>
        /// Adds missing keys to the language. Sets default values.
        /// </summary>
        /// <param name="translations">Language key and value pairs</param>
        //public void AddMissingValues(Dictionary<string, string> translations)
        //{
        //    List<string> oldKeys = keys;
        //    List<string> oldValues = values;

        //    // Clears the lists
        //    Clear();

        //    for (string key in oldKeys)
        //    {
        //        if ()
        //    }

        //    foreach (var translation in translations)
        //    {
        //        keys.Add(translation.Key);
        //        values.Add(translation.Value);
        //    }
        //}

        /// <summary>
        /// Removes all of the language's key and value pairs.
        /// </summary>
        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

#endif

        public override string ToString()
        {
            return "Lang " + langCode;
        }
    }
}
