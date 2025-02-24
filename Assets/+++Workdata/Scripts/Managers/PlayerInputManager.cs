using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Variables

    private GameInput gameInput;
    private SawFighter sawFighter;

    #endregion

    #region Unity Methods

    //We create a new ControllerMap
    private void Awake()
    {
        gameInput = new GameInput();
        sawFighter = GetComponent<SawFighter>();
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

        gameInput.Player.Jump.performed += sawFighter.OnJump;

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

        gameInput.Player.Jump.performed -= sawFighter.OnJump;

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
