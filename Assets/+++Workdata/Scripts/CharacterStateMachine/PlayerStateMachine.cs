using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    #region Variables

    //Inspector Variables
    [Header("Player Index")] 
    [SerializeField] private int playerIndex;

    [Header("SawFighter Behavior Variables")] 
    [SerializeField] private float percentageCount = 0;
    [SerializeField] private float rotationSpeed = 60f;
    //[SerializeField] private float knockbackPower = 10f;
    [SerializeField] private CharacterMoveSet move;
    [SerializeField] private GameObject opponent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private CinemachineImpulseSource cmImpulse;

    [Header("KnockBack Variables")]
    [SerializeField] private float inputForce = 7.5f;
    [SerializeField] private AnimationCurve knockBackForceCurve;

    //public Variables
    public bool inAttack = false;
    public bool inBlock = false;

    //private Variables
    private float knockBackTime;
    private Vector2 attackForce;
    public Vector2 combinedForce;
    private bool isJumping = false;
    private bool inHitStun = false;
    private float hitStunDuration = 0f;
    private bool canGetDamage = true;
    private bool isBeingKnockedBack = false;
    private bool canDash = true;
    private bool getFixedKnockBack;
    private Quaternion targetRotation;
    private Animator anim;
    private PlayerStateFactory states;
    
    //getters and setters
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float SpeedChangeRate { get; private set; }
    [field: SerializeField] public float DashPower { get; private set; }
    [field: SerializeField] public float JumpPower { get; private set; }
    [field: SerializeField] public float FallMultiplier { get; private set; }

    public PlayerBaseState CurrentState { get; set; }
    public Rigidbody Rb { get; private set; }
    public  Vector2 MoveInput { get; private set; }
    public bool IsJumpedPressed { get; private set; }
    public bool RequireNewJumpPress { get; set; }
    public bool IsDashing { get; set; }
    public float LastMovementX { get; set; }
    

    #endregion
    
    enum CurrentMove
    {
        Attack1,
        AttackLw1,
        AttackS1,
        AttackAir1,
        Attack2,
        AttackLw2,
        AttackS2,
        AttackAir2,
        SpecialN,
        SpecialLw,
        SpecialS,
        SpecialAir
    }

    #region UnityMethods

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        
        //setup states
        states = new PlayerStateFactory(this);
        CurrentState = states.Grounded();
        CurrentState.EnterState();
    }

    private void Update()
    {
        CurrentState.UpdateStates();
    }

    private void FixedUpdate()
    {
        if (!inAttack)
        {
            RotateToOpponent();
        }
    }

    #endregion
    
    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        IsJumpedPressed = context.ReadValueAsButton();
        RequireNewJumpPress = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && context.performed)
        {
            canDash = false;
            IsDashing = true;
            StartCoroutine(DashCooldown());
        }
    }
    

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock) //isgrounded
        {
            anim.SetTrigger("Block");
            anim.SetBool("isBlocking", true);
            inBlock = true;
        }

        if (context.canceled  && !inAttack) //isgrounded
        {
            anim.SetBool("isBlocking", false);
            inBlock = false;
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            move.OnHitFrameEnd();
            move.SetMove((int)CurrentMove.Attack1);
            anim.SetTrigger("Jab");
            inAttack = true;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            move.OnHitFrameEnd();
            move.SetMove((int)CurrentMove.Attack2);
            anim.SetTrigger("HeavyAttack");
            inAttack = true;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            move.OnHitFrameEnd();
            move.SetMove((int)CurrentMove.SpecialN);
            anim.SetTrigger("Special");
            inAttack = true;
        }
    }

    #endregion
    
    #region Player Movement
    
    private void RotateToOpponent()
    {
        Vector3 direction = (opponent.transform.position - transform.position).normalized;

        //ensure the rotation only happens on the Y-Axis
        direction.y = 0;
        
        targetRotation = Quaternion.LookRotation(direction);

        //rotate the visual of the player to opponent
        anim.transform.rotation =
            Quaternion.Slerp(anim.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void PlayerMovement()
    {
        float currentSpeed = LastMovementX;
        
        if (!inAttack && !inBlock)
        {
            float targetSpeed = MoveInput.x == 0 ? 0 : Speed * MoveInput.x;

            if (Mathf.Abs(currentSpeed - targetSpeed) > 0.05f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, SpeedChangeRate * Time.deltaTime);
            }
            else
            {
                currentSpeed = targetSpeed;
            }

            Rb.linearVelocity = new Vector2(currentSpeed, Rb.linearVelocity.y);
        }
        else
        {
            float targetSpeed = 0;

            if (Mathf.Abs(currentSpeed - targetSpeed) > 0.05f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, SpeedChangeRate * Time.deltaTime);
            }
            else
            {
                currentSpeed = targetSpeed;
            }

            Rb.linearVelocity = new Vector2(currentSpeed, Rb.linearVelocity.y);
        }

        LastMovementX = currentSpeed;
    }

    #endregion

    #region PlayerStateMachine Methods

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(1f);
        canDash = true;
    }
    
    /// <summary>
    /// check if player is on the ground
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        bool hitGround = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
        
        return hitGround;
    }
    
    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    #endregion
}
