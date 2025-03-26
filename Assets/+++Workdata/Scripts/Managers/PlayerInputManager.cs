using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Variables

    private GameInput gameInput;
    private SawFighter sawFighter;

    #endregion

    #region Unity Methods
    
    private void Awake()
    {
        var characterControlsArray = FindObjectsByType<SawFighter>(FindObjectsSortMode.None);
        var playerInput = GetComponent<PlayerInput>();
        var index = playerInput.playerIndex;
        sawFighter = characterControlsArray.FirstOrDefault(m => m.GetPlayerIndex() == index);
        
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
    }

    /// <summary>
    /// Enable the ControllerMap
    /// Subscribe methods to certain buttons
    /// </summary>
    private void OnEnable()
    {
        gameInput.Enable();

        gameInput.Player.Move.performed += sawFighter.OnMove;
        gameInput.Player.Move.canceled += sawFighter.OnMove;

        gameInput.Player.Jump.started += sawFighter.OnJump;
        gameInput.Player.Jump.canceled += sawFighter.OnJump;

        gameInput.Player.Sprint.performed += sawFighter.OnSprint;
        gameInput.Player.Sprint.canceled += sawFighter.OnSprint;

        gameInput.Player.Block.performed += sawFighter.OnBlock;
        gameInput.Player.Block.canceled += sawFighter.OnBlock;
        
        gameInput.Player.Jab.performed += sawFighter.OnLightAttack;

        gameInput.Player.HeavyAttack.performed += sawFighter.OnHeavyAttack;

        gameInput.Player.SpecialAttack.performed += sawFighter.OnSpecialAttack;
    }

    /// <summary>
    /// Disable the ControllerMap
    /// Desubscribe methods to certain buttons
    /// </summary>
    private void OnDisable()
    {
        gameInput.Disable();
        
        gameInput.Player.Move.performed -= sawFighter.OnMove;
        gameInput.Player.Move.canceled -= sawFighter.OnMove;
        
        gameInput.Player.Jump.started -= sawFighter.OnJump;
        gameInput.Player.Jump.canceled -= sawFighter.OnJump;

        gameInput.Player.Sprint.performed -= sawFighter.OnSprint;
        gameInput.Player.Sprint.canceled -= sawFighter.OnSprint;
        
        gameInput.Player.Block.performed -= sawFighter.OnBlock;
        gameInput.Player.Block.canceled -= sawFighter.OnBlock;
        
        gameInput.Player.Jab.performed -= sawFighter.OnLightAttack;

        gameInput.Player.HeavyAttack.performed -= sawFighter.OnHeavyAttack;
        
        gameInput.Player.SpecialAttack.performed -= sawFighter.OnSpecialAttack;
    }

    #endregion
}
