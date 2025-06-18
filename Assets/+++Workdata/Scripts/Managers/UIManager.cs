using System;
using System.Collections;
using System.Globalization;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Main Canvases")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup characterSelection;
    [SerializeField] private CanvasGroup inGame;
    [SerializeField] private CanvasGroup optionMenu;
    [SerializeField] private CanvasGroup quitMenu;
    
    [Header("Main EventSystem Components")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private InputSystemUIInputModule mainInputModule;

    [Header("Player Canvases")]
    [SerializeField] private CanvasGroup characterSelectionP1;
    [SerializeField] private CanvasGroup characterSelectionP2;
    [SerializeField] private CanvasGroup pauseMenuP1;
    [SerializeField] private CanvasGroup pauseMenuP2;
    [SerializeField] private CanvasGroup optionMenuP1;
    [SerializeField] private CanvasGroup optionMenuP2;
    [SerializeField] private CanvasGroup winningScreenP1;
    [SerializeField] private CanvasGroup winningScreenP2;

    [Header("GameObjects To First Select")]
    [SerializeField] private GameObject versusButton;
    [SerializeField] private GameObject startMatchButton;
    [SerializeField] private GameObject optionsSelect;
    [SerializeField] private GameObject quitSelect;
    [SerializeField] private GameObject charSelectP1;
    [SerializeField] private GameObject charSelectP2;
    [SerializeField] private GameObject pauseSelectP1;
    [SerializeField] private GameObject pauseSelectP2;
    [SerializeField] private GameObject optionSelectP1;
    [SerializeField] private GameObject optionSelectP2;
    [SerializeField] private GameObject winningSelectP1;
    [SerializeField] private GameObject winningSelectP2;

    [Header("In Game UI")]
    [SerializeField] private TextMeshProUGUI player1Percentage;
    [SerializeField] private TextMeshProUGUI player2Percentage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winScoreGame;
    [SerializeField] private TextMeshProUGUI player1WinScore;
    [SerializeField] private TextMeshProUGUI player2WinScore;
    
    [field: SerializeField] public float MatchTime { get; private set; }
    public float RemainingMatchTime { get; set; }
    
    
    public bool countdownActive;

    public event Action onTimerExpired;

    public PlayerStateMachine Player1 { get; private set; }
    public PlayerStateMachine Player2 { get; private set; }
    
    private CinemachineTargetGroup cmTargetGroup;

    private void Awake()
    {
        Instance = this;

        EventSystem.current.SetSelectedGameObject(versusButton);

        GameStateManager.Instance.onStateChanged += ActivateInGameUI;
    }

    private void Start()
    {
        cmTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
    }

    private void Update()
    {
        if (countdownActive)
        {
            if (RemainingMatchTime > 0)
            {
                RemainingMatchTime -= Time.deltaTime;
            }
            else if (RemainingMatchTime < 0)
            {
                RemainingMatchTime = 0;
                if (onTimerExpired != null)
                {
                    onTimerExpired();
                }
            }
            
            int minutes = Mathf.FloorToInt(RemainingMatchTime / 60);
            int seconds = Mathf.FloorToInt(RemainingMatchTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void OnDisable()
    {
        GameStateManager.Instance.onStateChanged -= ActivateInGameUI;
    }

    private void ActivateInGameUI(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.InGame:
                cmTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
                cmTargetGroup.Targets.Clear();
                cmTargetGroup.AddMember(Player1.CameraPoint, 1f, 1f);
                cmTargetGroup.AddMember(Player2.CameraPoint, 1f, 1f);
                Time.timeScale = 1f;
                winScoreGame.text = PlayerConfigurationManager.Instance.PlayerConfigs[0].Wins + " - " +
                               PlayerConfigurationManager.Instance.PlayerConfigs[1].Wins;
                inGame.ShowCanvasGroup();
                Player1.onPercentageChanged += Player1Percentage;
                Player2.onPercentageChanged += Player2Percentage;
                countdownActive = true;
                break;
            case GameStateManager.GameState.InMainMenu:
                cmTargetGroup.Targets.Clear();
                cmTargetGroup.AddMember(GameObject.Find("CameraPoint1").transform, 1f, 1f);
                cmTargetGroup.AddMember(GameObject.Find("CameraPoint2").transform, 1f, 1f);
                Time.timeScale = 1f;
                inGame.HideCanvasGroup();
                if (Player1 != null && Player2 != null)
                {
                    Player1.onPercentageChanged -= Player1Percentage;
                    Player2.onPercentageChanged -= Player2Percentage; 
                }
                countdownActive = false;
                RemainingMatchTime = MatchTime;
                break;
            case GameStateManager.GameState.InGameMenus:
                Time.timeScale = 0f;
                countdownActive = false;
                break;
        }
    }

    public void EnterMainMenu(bool fromGame)
    {
        mainMenu.ShowCanvasGroup();
        characterSelection.HideCanvasGroup();
        characterSelectionP1.HideCanvasGroup();
        characterSelectionP2.HideCanvasGroup();
        inGame.HideCanvasGroup();
        pauseMenuP1.HideCanvasGroup();
        pauseMenuP2.HideCanvasGroup();
        winningScreenP1.HideCanvasGroup();
        winningScreenP2.HideCanvasGroup();
        optionMenu.HideCanvasGroup();
        quitMenu.HideCanvasGroup();
        if (PlayerConfigurationManager.Instance.PlayerConfigs.Count == 2)
        {
            StartCoroutine(SwitchToSingleEventSystem(versusButton)); 
        }

        if (fromGame)
        {
            GameStateManager.Instance.GoToMainMenu(false);
        }
        else
        {
            GameStateManager.Instance.SwitchGameState(GameStateManager.GameState.InMainMenu); 
        }
    }

    public void EnterCharacterSelection(bool fromGame)
    {
        if (fromGame && PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.isActiveAndEnabled)
        {
            PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(charSelectP1);
            PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.enabled = true;
            PlayerConfigurationManager.Instance.PlayerConfigs[1].UIInputModule.enabled = true;
            PlayerConfigurationManager.Instance.PlayerConfigs[1].ReassignUIActions();
            PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(charSelectP2);
        }
        else if (fromGame && PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.isActiveAndEnabled)
        {
            PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(charSelectP2);
            PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.enabled = true;
            PlayerConfigurationManager.Instance.PlayerConfigs[0].UIInputModule.enabled = true;
            PlayerConfigurationManager.Instance.PlayerConfigs[0].ReassignUIActions();
            PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(charSelectP1);
        }
        else
        {
            eventSystem.enabled = false;
            mainInputModule.enabled = false;
            for (int i = 0; i < PlayerConfigurationManager.Instance.PlayerConfigs.Count; i++)
            {
                PlayerConfigurationManager.Instance.PlayerConfigs[i].PlayerEvent.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[i].UIInputModule.enabled = true;

                if (i == 0)
                {
                    PlayerConfigurationManager.Instance.PlayerConfigs[i].ReassignUIActions();
                    PlayerConfigurationManager.Instance.PlayerConfigs[i].PlayerEvent.SetSelectedGameObject(charSelectP1);
                }

                if (i == 1)
                {
                    PlayerConfigurationManager.Instance.PlayerConfigs[i].ReassignUIActions();
                    PlayerConfigurationManager.Instance.PlayerConfigs[i].PlayerEvent.SetSelectedGameObject(charSelectP2);
                }
            }
        }
        
        PlayerConfigurationManager.Instance.hasEnteredCharSelection = true;
        characterSelection.ShowCanvasGroup();
        characterSelectionP1.ShowCanvasGroup();
        characterSelectionP2.ShowCanvasGroup();
        mainMenu.HideCanvasGroup();
        pauseMenuP1.HideCanvasGroup();
        pauseMenuP2.HideCanvasGroup();
        winningScreenP1.HideCanvasGroup();
        winningScreenP2.HideCanvasGroup();

        if (fromGame)
        {
            GameStateManager.Instance.GoToMainMenu(false);
        }
        else
        {
            GameStateManager.Instance.SwitchGameState(GameStateManager.GameState.InMainMenu);
        }
    }
    
    public void EnterPauseMenu(int index)
    {
        GameStateManager.Instance.SwitchGameState(GameStateManager.GameState.InGameMenus);
        switch (index)
        {
            case 0:
                pauseMenuP1.ShowCanvasGroup();
                optionMenuP1.HideCanvasGroup();
                PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[0].UIInputModule.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[0].ReassignUIActions();
                PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(pauseSelectP1);
                break;
            case 1:
                pauseMenuP2.ShowCanvasGroup();
                optionMenuP2.HideCanvasGroup();
                PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[1].UIInputModule.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[1].ReassignUIActions();
                PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(pauseSelectP2);
                break;
        }
    }

    public void ResumeGame(int index)
    {
        GameStateManager.Instance.SwitchGameState(GameStateManager.GameState.InGame);
        switch (index)
        {
            case 0:
                pauseMenuP1.HideCanvasGroup();
                break;
            case 1:
                pauseMenuP2.HideCanvasGroup();
                StartCoroutine(SwitchToSingleEventSystem(null));
                break;
        }
    }

    public void EnterWinningScreen(int index)
    {
        GameStateManager.Instance.SwitchGameState(GameStateManager.GameState.InGameMenus);
        inGame.HideCanvasGroup();
        switch (index)
        {
            case 0:
                player1WinScore.text = PlayerConfigurationManager.Instance.PlayerConfigs[0].Wins + " - " +
                                       PlayerConfigurationManager.Instance.PlayerConfigs[1].Wins;
                winningScreenP1.ShowCanvasGroup();
                PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[0].UIInputModule.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[0].ReassignUIActions();
                PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(winningSelectP1);
                break;
            case 1:
                player2WinScore.text = PlayerConfigurationManager.Instance.PlayerConfigs[0].Wins + " - " +
                                       PlayerConfigurationManager.Instance.PlayerConfigs[1].Wins;
                winningScreenP2.ShowCanvasGroup();
                PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[1].UIInputModule.enabled = true;
                PlayerConfigurationManager.Instance.PlayerConfigs[1].ReassignUIActions();
                PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(winningSelectP2);
                break;
        }
    }

    public void EnterOptionMenu(int index)
    {
        switch (index)
        {
            case 0:
                optionMenuP1.ShowCanvasGroup();
                pauseMenuP1.DisableInteraction();
                PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(optionSelectP1);
                break;
            case 1:
                optionMenuP2.ShowCanvasGroup();
                pauseMenuP2.DisableInteraction();
                PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(optionSelectP2);
                break;
            default:
                optionMenu.ShowCanvasGroup();
                mainMenu.DisableInteraction();
                EventSystem.current.SetSelectedGameObject(optionsSelect);
                break;
        }
    }

    public void EnterQuitMenu()
    {
        quitMenu.ShowCanvasGroup();
        mainMenu.DisableInteraction();
        EventSystem.current.SetSelectedGameObject(quitSelect);
    }
    
    public void CheckAllPlayerReady()
    {
        bool player1Ready = false;
        bool player2Ready = false;
        
        for (int i = 0; i < PlayerConfigurationManager.Instance.PlayerConfigs.Count; i++)
        {
            if (i == 0)
            {
                player1Ready = PlayerConfigurationManager.Instance.PlayerConfigs[i].IsReady;
            }

            if (i == 1)
            {
                player2Ready = PlayerConfigurationManager.Instance.PlayerConfigs[i].IsReady;
            }
        }

        if (player1Ready && player2Ready)
        {
            CharacterSelected();
        }
    }

    public void CharacterSelected()
    {
        characterSelectionP1.HideCanvasGroup();
        characterSelectionP2.HideCanvasGroup();
        characterSelection.ShowCanvasGroup();
        startMatchButton.SetActive(true);
        //use coroutine to wait for frames to let unity intern system correctly handle the eventsystem 
        StartCoroutine(SwitchToSingleEventSystem(startMatchButton));
    }

    public void EnterGame()
    {
        var characterArray = FindObjectsByType<PlayerStateMachine>(FindObjectsSortMode.None);
        for (int i = 0; i < characterArray.Length; i++)
        {
            if (characterArray[i].PlayerIndex == 0)
            {
                Player1 = characterArray[i];
            }

            if (characterArray[i].PlayerIndex == 1)
            {
                Player2 = characterArray[i];
            }
        }

        cmTargetGroup.Targets.Clear();
        cmTargetGroup.AddMember(Player1.CameraPoint, 1f, 1f);
        cmTargetGroup.AddMember(Player2.CameraPoint, 1f, 1f);
        
        Player1.Opponent = Player2;
        Player2.Opponent = Player1;
        Player1.SetLayers(0);
        Player2.SetLayers(1);
        Player1Percentage(0);
        Player2Percentage(0);
        
        for (int i = 0; i < PlayerConfigurationManager.Instance.PlayerConfigs.Count; i++)
        {
            PlayerConfigurationManager.Instance.PlayerConfigs[i].IsReady = false;
        }
        startMatchButton.SetActive(false);
        characterSelection.HideCanvasGroup();
        RemainingMatchTime = MatchTime;
        GameStateManager.Instance.StartNewGame();
    }

    public void ReloadGameplayScene(int index)
    {
        switch (index)
        {
            case 0:
                pauseMenuP1.HideCanvasGroup();
                winningScreenP1.HideCanvasGroup();
                break;
            case 1:
                pauseMenuP2.HideCanvasGroup();
                winningScreenP2.HideCanvasGroup();
                StartCoroutine(SwitchToSingleEventSystem(null));
                break;
        }
        Time.timeScale = 1f;
        Player1Percentage(0);
        Player2Percentage(0);
        foreach (var player in PlayerConfigurationManager.Instance.PlayerConfigs)
        {
            player.Wins = 0;
        }
        GameStateManager.Instance.wallBreakCountR = 0;
        GameStateManager.Instance.wallBreakCountL = 0;
        GameStateManager.Instance.LoadGameplayScene(GameStateManager.fightingScene1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ReassignUIActions()
    {
        GameInput input = PlayerConfigurationManager.Instance.PlayerConfigs[0].GameInputMap;
        mainInputModule.actionsAsset = input.asset;
        mainInputModule.point = InputActionReference.Create(input.UI.Point);
        mainInputModule.move = InputActionReference.Create(input.UI.Navigate);
        mainInputModule.submit = InputActionReference.Create(input.UI.Submit);
        mainInputModule.cancel = InputActionReference.Create(input.UI.Cancel);
        PlayerConfigurationManager.Instance.PlayerConfigs[0].Input.uiInputModule = mainInputModule;
    }
    
    private IEnumerator SwitchToSingleEventSystem(GameObject buttonToSelect)
    {
        PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.SetSelectedGameObject(null);
        PlayerConfigurationManager.Instance.PlayerConfigs[1].UIInputModule.enabled = false;
        PlayerConfigurationManager.Instance.PlayerConfigs[1].PlayerEvent.enabled = false;

        yield return null;
        
        eventSystem.enabled = true;
        mainInputModule.enabled = true;
        ReassignUIActions();

        yield return null;

        PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.SetSelectedGameObject(null);
        PlayerConfigurationManager.Instance.PlayerConfigs[0].UIInputModule.enabled = false;
        PlayerConfigurationManager.Instance.PlayerConfigs[0].PlayerEvent.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
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
