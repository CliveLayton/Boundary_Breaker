using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup characterSelection;
    [SerializeField] private CanvasGroup inGame;
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private CanvasGroup winningScreen;
    [SerializeField] private CanvasGroup optionMenu;
    [SerializeField] private CanvasGroup quitMenu;

    [SerializeField] private GameObject startMatchButton;

    private void Awake()
    {
        Instance = this;
    }

    public void EnterCharacterSelection()
    {
        characterSelection.ShowCanvasGroup();
        mainMenu.HideCanvasGroup();
    }

    public void CharacterSelected()
    {
        startMatchButton.SetActive(true);
    }

    public void EnterGame()
    {
        startMatchButton.SetActive(false);
        characterSelection.HideCanvasGroup();
        GameStateManager.Instance.StartNewGame();
    }
}
