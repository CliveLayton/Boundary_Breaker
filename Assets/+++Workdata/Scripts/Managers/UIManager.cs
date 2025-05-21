using System;
using System.Globalization;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI player1Percentage;
    [SerializeField] private TextMeshProUGUI player2Percentage;
    [SerializeField] private TextMeshProUGUI timerText;
    
    public float matchTime;
    public float remainingMatchTime;
    public bool countdownActive;

    public event Action<bool> onTimerExpired;

    private void Awake()
    {
        Instance = this;

        GameStateManager.Instance.onStateChanged += ActivateInGameUI;
    }

    private void Update()
    {
        if (countdownActive)
        {
            if (remainingMatchTime > 0)
            {
                remainingMatchTime -= Time.deltaTime;
            }
            else if (remainingMatchTime < 0)
            {
                remainingMatchTime = 0;
                if (onTimerExpired != null)
                {
                    onTimerExpired(true);
                }
            }
            
            int minutes = Mathf.FloorToInt(remainingMatchTime / 60);
            int seconds = Mathf.FloorToInt(remainingMatchTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnDisable()
    {
        GameStateManager.Instance.onStateChanged -= ActivateInGameUI;
    }

    private void ActivateInGameUI(GameStateManager.GameState newState)
    {
        PlayerStateMachine player1 = GameObject.Find("TestPlayer1").GetComponent<PlayerStateMachine>();
        PlayerStateMachine player2 = GameObject.Find("TestPlayer2").GetComponent<PlayerStateMachine>();
        
        switch (newState)
        {
            case GameStateManager.GameState.InGame:
                inGame.ShowCanvasGroup();
                player1.onPercentageChanged += Player1Percentage;
                player2.onPercentageChanged += Player2Percentage;
                remainingMatchTime = matchTime;
                countdownActive = true;
                break;
            case GameStateManager.GameState.InMainMenu:
                inGame.HideCanvasGroup();
                player1.onPercentageChanged -= Player1Percentage;
                player2.onPercentageChanged -= Player2Percentage;
                countdownActive = false;
                break;
        }
    }

    public void EnterMainMenu()
    {
        mainMenu.ShowCanvasGroup();
        characterSelection.HideCanvasGroup();
        inGame.HideCanvasGroup();
        pauseMenu.HideCanvasGroup();
        winningScreen.HideCanvasGroup();
        optionMenu.HideCanvasGroup();
        quitMenu.HideCanvasGroup();
    }

    public void EnterCharacterSelection()
    {
        characterSelection.ShowCanvasGroup();
        mainMenu.HideCanvasGroup();
    }

    public void EnterOptionMenu()
    {
        optionMenu.ShowCanvasGroup();
        mainMenu.HideCanvasGroup();
    }

    public void EnterQuitMenu()
    {
        quitMenu.ShowCanvasGroup();
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

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Player1Percentage(float percentage)
    {
        player1Percentage.text = percentage.ToString("0") + "%";
        if (percentage < 30)
        {
            player1Percentage.color = Color.white;
        }
        else if (percentage is > 30 and < 60)
        {
            player1Percentage.color = Color.yellow;
        }
        else
        {
            player1Percentage.color = Color.red;
        }
    }

    private void Player2Percentage(float percentage)
    {
        player2Percentage.text = percentage.ToString("0") + "%";
        if (percentage < 30)
        {
            player2Percentage.color = Color.white;
        }
        else if (percentage is > 30 and < 60)
        {
            player2Percentage.color = Color.yellow;
        }
        else
        {
            player2Percentage.color = Color.red;
        }
    }
}
