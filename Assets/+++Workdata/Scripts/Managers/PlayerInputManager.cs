using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    #region Variables

    private GameInput gameInput;
    //private PlayerController playerController;
    private PlayerStateMachine playerStateMachine;
    private int index;

    #endregion

    #region Unity Methods
    
    private void Awake()
    {
        //var characterControlsArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var characterControlsArray = FindObjectsByType<PlayerStateMachine>(FindObjectsSortMode.None);
        var playerInput = GetComponent<PlayerInput>();
        index = playerInput.playerIndex;
        //playerController = characterControlsArray.FirstOrDefault(m => m.GetPlayerIndex() == index);
        playerStateMachine = characterControlsArray.FirstOrDefault(m => m.GetPlayerIndex() == index);
        
        //We create a new ControllerMap and assign it to the right player
        gameInput = new GameInput();

        InputDevice joinedDevice = playerInput.devices.FirstOrDefault();

        //if the joined device is a Keyboard or Mouse, assign both Keyboard & Mouse
        if (joinedDevice is Keyboard || joinedDevice is Mouse)
        {
            gameInput.devices = new InputDevice[] { Keyboard.current, Mouse.current };
            playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
        else
        {
            //Otherwise, keep the default device
            gameInput.devices = new[] { joinedDevice };
        }

        gameInput.Enable();
        GameStateManager.Instance.onStateChanged += HandleInputActivation;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        gameInput.Disable();
        GameStateManager.Instance.onStateChanged -= HandleInputActivation;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        //disable and unsubscribe all old playerstatemachines
        UnsubscribePlayerInput();
        
        if (scene.name == LoadSceneManager.instance.currentScene)
        {
            var characterControlsArray = FindObjectsByType<PlayerStateMachine>(FindObjectsSortMode.None);
            playerStateMachine = characterControlsArray.FirstOrDefault(m => m.GetPlayerIndex() == index);
            HandleInputActivation(GameStateManager.Instance.currentState);
        }
    }

    private void HandleInputActivation(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.InMainMenu:
                UnsubscribePlayerInput();
                break;
            case GameStateManager.GameState.InGame:
                SubscribePlayerInput();
                break;
        }
    }

    /// <summary>
    /// disable the Controllermap
    /// desubscribe methods to certain buttons
    /// </summary>
    private void UnsubscribePlayerInput()
    {
        gameInput.Player.Move.performed -= playerStateMachine.OnMove;
        gameInput.Player.Move.canceled -= playerStateMachine.OnMove;

        gameInput.Player.Jump.started -= playerStateMachine.OnJump;
        gameInput.Player.Jump.canceled -= playerStateMachine.OnJump;

        gameInput.Player.Dash.performed -= playerStateMachine.OnDash;
        gameInput.Player.Dash.canceled -= playerStateMachine.OnDash;

        gameInput.Player.Jab.performed -= playerStateMachine.OnLightAttack;

        gameInput.Player.HeavyAttack.performed -= playerStateMachine.OnHeavyAttack;

        gameInput.Player.SpecialAttack.performed -= playerStateMachine.OnSpecialAttack;
        
        gameInput.Player.Grab.performed -= playerStateMachine.OnGrab;
    }

    /// <summary>
    /// enable ControllerMap
    /// subscribe methods to certain Buttons
    /// </summary>
    private void SubscribePlayerInput()
    {
        gameInput.Player.Move.performed += playerStateMachine.OnMove;
        gameInput.Player.Move.canceled += playerStateMachine.OnMove;

        gameInput.Player.Jump.started += playerStateMachine.OnJump;
        gameInput.Player.Jump.canceled += playerStateMachine.OnJump;

        gameInput.Player.Dash.performed += playerStateMachine.OnDash;
        gameInput.Player.Dash.canceled += playerStateMachine.OnDash;

        gameInput.Player.Jab.performed += playerStateMachine.OnLightAttack;

        gameInput.Player.HeavyAttack.performed += playerStateMachine.OnHeavyAttack;

        gameInput.Player.SpecialAttack.performed += playerStateMachine.OnSpecialAttack;

        gameInput.Player.Grab.performed += playerStateMachine.OnGrab;
    }

    public int GetInputIndex()
    {
        return index;
    }

    #endregion
}
