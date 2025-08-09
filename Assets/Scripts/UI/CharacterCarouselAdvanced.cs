using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FlyingNumbers.UI
{
    public class CharacterCarousel : MonoBehaviour
    {
        [Header("Data")]
        public CharacterCatalog catalog;

        [Header("UI")]
        public Image previewImage;
        public Text nameText;
        public Button prevButton;
        public Button nextButton;
        public Button playButton;

        private int index;

        private void Awake()
        {
            if (prevButton) prevButton.onClick.AddListener(Prev);
            if (nextButton) nextButton.onClick.AddListener(Next);
            if (playButton) playButton.onClick.AddListener(Play);
        }

        private void Start()
        {
            if (catalog == null || catalog.entries.Count == 0) return;

            var saved = CharacterSelectionService.GetOrDefault(catalog.FirstNameOrNull());
            index = Mathf.Max(0, catalog.entries.FindIndex(e => e.displayName == saved));
            Apply();
        }

        private void Apply()
        {
            if (catalog == null || catalog.entries.Count == 0) return;

            index = (index % catalog.entries.Count + catalog.entries.Count) % catalog.entries.Count;
            var e = catalog.entries[index];

            if (nameText) nameText.text = e.displayName;

            if (previewImage)
            {
                Sprite bodySprite = null;
                if (e.prefab != null)
                {
                    var body = e.prefab.transform.Find("Body");
                    if (body)
                    {
                        var sr = body.GetComponent<SpriteRenderer>();
                        if (sr) bodySprite = sr.sprite;
                    }
                    if (bodySprite == null)
                    {
                        var anySr = e.prefab.GetComponentsInChildren<SpriteRenderer>(true).FirstOrDefault();
                        if (anySr) bodySprite = anySr.sprite;
                    }
                }
                previewImage.sprite = bodySprite;
                previewImage.preserveAspect = true;
                previewImage.enabled = bodySprite != null;
            }

            CharacterSelectionService.Set(e.displayName);
        }

        public void Next() { index++; Apply(); }
        public void Prev() { index--; Apply(); }

        private void Play()
        {
            var sceneLoaderType = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == "SceneLoader");
            if (sceneLoaderType != null)
            {
                var loaderInstance = FindObjectOfType(sceneLoaderType);
                if (loaderInstance != null)
                {
                    var m = sceneLoaderType.GetMethod("LoadGame");
                    if (m != null) m.Invoke(loaderInstance, null);
                }
            }
        }
    }
}
