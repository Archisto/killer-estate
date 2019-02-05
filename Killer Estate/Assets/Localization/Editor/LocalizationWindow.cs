using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using KillerEstate.Localization;
using L10n = KillerEstate.Localization.Localization;

namespace KillerEstate.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        private const string LocalizationKey = "Localization";
        private const int MaxEntriesPerPage = 20;

        public LangCode CurrentLanguage = LangCode.None;

        private Dictionary<string, string> _localizations =
            new Dictionary<string, string>();

        private int _page = 0;
        private int _lastPage = 0;

        /// <summary>
        /// Opens a localization editor window.
        /// </summary>
        [MenuItem("Localization/Edit Localization")]
        private static void OpenWindow()
        {
            LocalizationWindow window =
                GetWindow<LocalizationWindow>("Localization");
            window.Show();
        }

        private void OnEnable()
        {
            LangCode language = (LangCode) EditorPrefs.
                GetInt(LocalizationKey, (int) LangCode.None);
            SetLanguage(language);
        }

        private void OnGUI()
        {
            int currentIndex = 0;
            int startIndex = _page * MaxEntriesPerPage;
            _lastPage = (_localizations.Count - 1) / MaxEntriesPerPage;

            LangCode langCode = (LangCode) EditorGUILayout.
                EnumPopup(CurrentLanguage);
            SetLanguage(langCode);

            EditorGUILayout.BeginVertical();

            Dictionary<string, string> newValues =
                new Dictionary<string, string>();

            List<string> deletedKeys = new List<string>();

            foreach (var localization in _localizations)
            {
                // Separates entries to pages.
                // There's a maximum number of entries displayed
                // on each page but they are all still saved to JSON.
                if (currentIndex < startIndex ||
                    currentIndex >= startIndex + MaxEntriesPerPage)
                {
                    newValues.Add(localization.Key, localization.Value);
                    currentIndex++;
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                string key = EditorGUILayout.TextField(localization.Key);
                string value = EditorGUILayout.TextField(localization.Value);

                //bool duplicateKeys = false;
                try
                {
                    newValues.Add(key, value);
                }
                catch (ArgumentException e)
                {
                    Debug.LogError("A key with the same name already exists.");
                    //duplicateKeys = true;
                    return;
                }

                if (DeleteButtonPressed())
                {
                    deletedKeys.Add(localization.Key);
                }

                EditorGUILayout.EndHorizontal();

                currentIndex++;
            }

            _localizations = newValues;

            DeleteKeys(deletedKeys);

            AddValueButton();
            SaveButton();
            PageButtons();

            EditorGUILayout.EndVertical();
        }

        private void DeleteKeys(List<string> deletedKeys)
        {
            foreach (var deletedKey in deletedKeys)
            {
                if (_localizations.ContainsKey(deletedKey))
                {
                    _localizations.Remove(deletedKey);
                }
            }
        }

        private bool DeleteButtonPressed()
        {
            return GUILayout.Button("X");
        }

        private void AddValueButton()
        {
            if (GUILayout.Button("Add Value"))
            {
                if ( !_localizations.ContainsKey("") )
                {
                    _localizations.Add("", "");
                    _lastPage = _localizations.Count / MaxEntriesPerPage;
                    _page = _lastPage;
                }
            }
        }

        private void SaveButton()
        {
            if (GUILayout.Button("Save"))
            {
                L10n.CurrentLanguage.SetValues(_localizations);
                L10n.SaveCurrentLanguage();
            }
        }

        private void PageButtons()
        {
            EditorGUILayout.BeginHorizontal();

            // Previous page button
            if (ButtonPressed("Prev", _page > 0))
            {
                _page--;
            }

            // Centering a label
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            // Current page and last page numbers
            EditorGUILayout.LabelField(string.Format("Page: {0} / {1}",
                _page + 1, _lastPage + 1), centeredStyle);

            // Next page button
            if (ButtonPressed("Next", _page < _lastPage))
            {
                _page++;
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool ButtonPressed(string buttonLabel, bool requirement)
        {
            bool result = false;

            if (!requirement)
            {
                GUI.enabled = false;
            }

            bool pressed = GUILayout.Button(buttonLabel);
            result = requirement && pressed;

            if (!requirement)
            {
                GUI.enabled = true;
            }

            return result;
        }

        private void SetLanguage(LangCode langCode)
        {
            // Current language is already set to langCode. Just return.
            if (CurrentLanguage == langCode)
            {
                return;
            }

            CurrentLanguage = langCode;
            EditorPrefs.SetInt(LocalizationKey, (int) CurrentLanguage);
            _localizations.Clear();

            // Load localization file
            L10n.LoadLanguage(CurrentLanguage);
            _localizations = L10n.CurrentLanguage.GetValues();
        }
    }
}
