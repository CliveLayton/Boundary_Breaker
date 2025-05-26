using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    #region Variables

    //0 is ChainsawGirl, 1 is TigerBoy
    [SerializeField] private Sprite[] charSprites;
    [SerializeField] private ButtonEvents[] charButtons;
    [SerializeField] private Image characterIcon;
    [SerializeField] private int playerIndex;
    
    private int currentSelectedCharIndex;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        currentSelectedCharIndex = 0;
        characterIcon.sprite = charSprites[0];
        for (int i = 0; i < charButtons.Length; i++)
        {
            int index = i;
            charButtons[i].onSelect += () => ChangePlayer(index);
            charButtons[i].onSubmit += OnConfirmSelection;
            charButtons[i].onMouseEnter += () => ChangePlayer(index);
            charButtons[i].onMouseClick += OnConfirmSelection;
        }
    }

    #endregion

    #region CharacterSelect Methods

    private void ChangePlayer(int index)
    {
        characterIcon.sprite = charSprites[index];
        currentSelectedCharIndex = index;
    }

    public void OnConfirmSelection()
    {
        PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].IsReady = true;
        PlayerConfigurationManager.Instance.CheckAllPlayerReady();
    }

    #endregion
}
