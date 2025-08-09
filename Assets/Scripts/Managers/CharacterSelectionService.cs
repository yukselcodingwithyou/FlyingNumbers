using UnityEngine;

namespace FlyingNumbers
{
    public static class CharacterSelectionService
    {
        private const string Key = "FN_SelectedCharacterName";

        public static void Set(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            PlayerPrefs.SetString(Key, name);
            PlayerPrefs.Save();
        }

        public static string GetOrDefault(string fallbackName)
        {
            var name = PlayerPrefs.GetString(Key, null);
            return string.IsNullOrEmpty(name) ? fallbackName : name;
        }
    }
}
