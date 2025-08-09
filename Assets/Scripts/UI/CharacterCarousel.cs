using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple character carousel for the Start scene; cycles unlocked characters using UI buttons.
/// </summary>
public class CharacterCarousel : MonoBehaviour
{
    public Text nameText;
    public Image previewImage;
    public Button prevButton;
    public Button nextButton;

    private int index;

    private void Start()
    {
        // Start with currently selected
        index = CharacterManager.Instance != null ? CharacterManager.Instance.GetSelectedCharacterIndex() : 0;
        Refresh();

        if (prevButton != null) prevButton.onClick.AddListener(Prev);
        if (nextButton != null) nextButton.onClick.AddListener(Next);
    }

    private void Prev()
    {
        index = Mathf.Max(0, index - 1);
        Select();
    }

    private void Next()
    {
        if (CharacterManager.Instance == null)
        {
            index++;
        }
        else
        {
            index = Mathf.Min(CharacterManager.Instance.GetAllCharacters().Count - 1, index + 1);
        }
        Select();
    }

    private void Select()
    {
        if (CharacterManager.Instance != null)
        {
            if (CharacterManager.Instance.IsCharacterUnlocked(index))
            {
                CharacterManager.Instance.SelectCharacter(index);
            }
        }
        Refresh();
    }

    private void Refresh()
    {
        if (CharacterManager.Instance == null)
        {
            if (nameText != null) nameText.text = $"Character {index}";
            return;
        }

        var c = CharacterManager.Instance.GetCharacter(index);
        if (c != null)
        {
            if (nameText != null) nameText.text = c.name + (CharacterManager.Instance.IsCharacterUnlocked(index) ? "" : " (Locked)");
            if (previewImage != null)
            {
                previewImage.color = c.characterColor;
            }
        }
    }
}
