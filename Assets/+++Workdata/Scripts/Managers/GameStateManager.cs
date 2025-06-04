using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// The GameStateManager lies at the heart of our code.
/// Most importantly for this demonstration, it contains the GameData
/// and manages the loading and saving of our save files.
/// Additionally, it manages the loading and unloading of levels, as well as going back to the main menu.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region Variables

    public static GameStateManager Instance;

    public const string mainMenuSceneName = "Main Menus";
    public const string fightingScene1 = "Gameplay";

    public enum GameState
    {
        InMainMenu = 0,
        InGame = 1,
        InGameMenus = 2
    }

    //this event notifies any objects that need to know about the changing of the game state.
    public event Action<GameState> onStateChanged;
    
    //the current state
    public GameState currentState { get; private set; } = GameState.InMainMenu;

    public int wallBreakCountR;
    public int wallBreakCountL;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
        //use "Screen.currentResolution.refreshRate" to check for the max refreshrate the monitor of the player has
        Application.targetFrameRate = 60;

#if UNITY_EDITOR
    
        if (EditorPrefs.GetString("activeScene") != null && EditorPrefs.GetString("activeScene") != "ManagerScene")
        {
            SceneManager.LoadScene(EditorPrefs.GetString("activeScene"), LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        }

#endif
    }

    private void Start()
    {
        //when we start the game, we first want to enter the main menu
        currentState = GameState.InMainMenu;
        LoadSceneManager.instance.SwitchScene(fightingScene1,false);
        MusicManager.Instance.PlayMusic(MusicManager.Instance.mainMenuMusic, 0.1f);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.sceneLoaded += ReloadFightScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ReloadFightScene;
    }

    #endregion

    #region GameState Manager Methods

    /// <summary>
    /// called to enter the main menu. Also changes the game state
    /// </summary>
    /// <param name="showLoadingScreen">with or without loading screen</param>
    public void GoToMainMenu(bool showLoadingScreen = true)
    {
        foreach (var player in PlayerConfigurationManager.Instance.PlayerConfigs)
        {
            player.Wins = 0;
        }
        wallBreakCountR = 0;
        wallBreakCountL = 0;
        UIManager.Instance.Player1.transform.SetParent(CharacterPool.Instance.Player1PoolParent);
        UIManager.Instance.Player2.transform.SetParent(CharacterPool.Instance.Player2PoolParent);
        currentState = GameState.InMainMenu;
        if (onStateChanged != null)
        {
            onStateChanged(currentState);
        }
        LoadSceneManager.instance.SwitchScene(fightingScene1,showLoadingScreen);
        MusicManager.Instance.PlayMusic(MusicManager.Instance.mainMenuMusic, 0.1f);
        Cursor.lockState = CursorLockMode.None;
    }

    //called to start a new game. Also changes the game state.
    public void StartNewGame()
    {
        foreach (var player in PlayerConfigurationManager.Instance.PlayerConfigs)
        {
            player.Wins = 0;
        }
        
        currentState = GameState.InGame;
        if (onStateChanged != null)
        {
            onStateChanged(currentState);
        }

        //LoadSceneManager.instance.SwitchScene(fightingScene1, false);
        MusicManager.Instance.PlayMusic(MusicManager.Instance.fightingMusic[Random.Range(0,MusicManager.Instance.fightingMusic.Length)], 0.1f);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void SwitchGameState(GameState state)
    {
        currentState = state;
        if (onStateChanged != null)
        {
            onStateChanged(currentState);
        }
    }

    private void ReloadFightScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (currentState != GameState.InMainMenu)
        {
            if (scene.name == fightingScene1)
            {
                GameObject parentP1 = GameObject.Find("Player1");
                GameObject parentP2 = GameObject.Find("Player2");
                Transform player1 = UIManager.Instance.Player1.transform;
                Transform player2 = UIManager.Instance.Player2.transform;
                player1.SetParent(parentP1.transform);
                player1.localPosition = Vector3.zero;
                player2.SetParent(parentP2.transform);
                player2.localPosition = Vector3.zero;
            
                UIManager.Instance.RemainingMatchTime = UIManager.Instance.MatchTime;
                currentState = GameState.InGame;
                if (onStateChanged != null)
                {
                    onStateChanged(currentState);
                }
            } 
        }
    }

    public void LoadGameplayScene(string sceneName)
    {
        if (currentState == GameState.InMainMenu)
        {
            return;
        }
        
        UIManager.Instance.Player1.transform.SetParent(CharacterPool.Instance.Player1PoolParent);
        UIManager.Instance.Player2.transform.SetParent(CharacterPool.Instance.Player2PoolParent);
        LoadSceneManager.instance.SwitchScene(sceneName, false);
    }

    #endregion
}
