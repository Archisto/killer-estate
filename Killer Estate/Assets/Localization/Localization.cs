using System;
using System.Text;
using System.IO;
using UnityEngine;

namespace KillerEstate.Localization
{
    /// <summary>
    /// Language code.
    /// </summary>
    public enum LangCode
    {
        /// <summary>
        /// Language code not available
        /// </summary>
        None = 0,
        EN = 1,
        FI = 2
    }

    public static class Localization
    {
        public const string LocalizationFolderName = "Localization";
        public const string FileExtension = ".json";

        public static event Action LanguageLoaded;

        public static string LocalizationPath
        {
            get
            {
                return Path.Combine(Application.streamingAssetsPath,
                                    LocalizationFolderName);
            }
        }

        /// <summary>
        /// Currently loaded language object.
        /// </summary>
        public static Language CurrentLanguage { get; private set; }

        public static string GetLocalizationFilePath(LangCode langCode)
        {
            string fileName = langCode.ToString();
            return Path.Combine(LocalizationPath, fileName) + FileExtension;
        }

        public static void SaveCurrentLanguage()
        {
            // A language isn't loaded; just return
            if (CurrentLanguage == null ||
                CurrentLanguage.LanguageCode == LangCode.None)
            { 
                return;
            }

            if ( !Directory.Exists(LocalizationPath) )
            {
                // Localization directory does not exist. Let's create one.
                Directory.CreateDirectory(LocalizationPath);
            }

            // Serialize the language file and write
            // the serialized content to the file
            string path = GetLocalizationFilePath(CurrentLanguage.LanguageCode);
            string serializedLanguage = JsonUtility.ToJson(CurrentLanguage);
            File.WriteAllText(path, serializedLanguage, Encoding.UTF8);
        }

        public static void LoadLanguage(LangCode langCode)
        {
            var path = GetLocalizationFilePath(langCode);

            // The language exists
            if (File.Exists(path))
            {
                string jsonLanguage = File.ReadAllText(path);
                CurrentLanguage = JsonUtility.FromJson<Language>(jsonLanguage);

                if (LanguageLoaded != null)
                {
                    LanguageLoaded();
                }

                Debug.Log("Language loaded");
            }
            // The language we are trying to load does not exist
            else
            {
                CreateLanguage(langCode);

                //throw new LocalizationNotFoundException(langCode);
            }
        }

        public static void CreateLanguage(LangCode langCode)
        {
            CurrentLanguage = new Language(langCode);
        }
    }
}
