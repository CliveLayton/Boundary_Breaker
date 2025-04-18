using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour, IDamageable
{
    #region Variables

    //Inspector Variables
    [Header("Player Index")] 
    [SerializeField] private int playerIndex;

    [Header("SawFighter Behavior Variables")]
    [SerializeField] private float rotationSpeed = 60f;
    //[SerializeField] private float knockbackPower = 10f;
    [SerializeField] private LayerMask groundLayer;

    [Header("KnockBack Variables")]

    //public Variables

    //private Variables
    private bool canDash = true;
    private Quaternion targetRotation;
    private PlayerStateFactory states;
    
    //getters and setters
    [field: SerializeField] public float PercentageCount { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float SpeedChangeRate { get; private set; }
    [field: SerializeField] public float DashPower { get; private set; }
    [field: SerializeField] public float JumpPower { get; private set; }
    [field: SerializeField] public float FallMultiplier { get; private set; }
    [field: SerializeField] public float InputForce { get; private set; }
    [field: SerializeField] public AnimationCurve KnockBackForceCurve { get; private set; }
    [field: SerializeField] public GameObject Opponent { get; private set; }
    [field: SerializeField] public CinemachineImpulseSource CmImpulse { get; private set; }

    public PlayerBaseState CurrentState { get; set; }
    public Rigidbody Rb { get; private set; }
    public  Animator Anim { get; private set; }
    public ECurrentMove CurrentMove { get; private set; }
    public  Vector2 MoveInput { get; private set; }
    public float LastMovementX { get; set; }
    public bool IsJumpedPressed { get; private set; }
    public bool RequireNewJumpPress { get; set; }
    public bool IsDashing { get; set; }
    public bool InBlock { get; set; }
    [field: SerializeField] public bool IsAttacking { get; set; }
    public bool CanCombo { get; set; }
    public  bool InHitStun { get; set; }
    public bool InComboHit { get; set; }
    public bool CanGetDamage { get; set; } = true;
    public float KnockBackTime { get; private set; }
    public Vector2 AttackForce { get; private set; }
    public Vector2 CombinedForce { get; set; }
    public bool GetFixedKnockBack { get; private set; }
    public float HitStunDuration { get; private set; }
    public float HitStopDuration { get; private set; }
    public bool IsComboPossible { get; private set; }
    public bool GetKnockBackToOpponent { get; private set; }
    public bool IsBeingKnockedBack { get; set; }
    [field: SerializeField] public CharacterMoves[] Moves { get; private set; }


    #endregion
    
    public enum ECurrentMove
    {
        Attack1,
        Attack1Lw,
        Attack1S,
        Attack1Air,
        Attack2,
        Attack2Lw,
        Attack2S,
        Attack2Air,
        SpecialN,
        SpecialLw,
        SpecialS,
        SpecialAir
    }

    #region ContextMenu Methods

    /// <summary>
    /// get the frames of each animation assigned for the frame checkers
    /// </summary>
    [ContextMenu("Get Frames From Animations")]
    private void GetAnimationFrames()
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            Moves[i].frameChecker.GetTotalFrames();
        }
    }

    #endregion

    #region UnityMethods

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();
        
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
        if (!IsAttacking)
        {
            RotateToOpponent();
        }
    }

    private void LateUpdate()
    {
        PlayerAnimations();
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
        if (context.performed && !IsAttacking && !InBlock) //isgrounded
        {
            //Anim.SetTrigger("Block");
            //Anim.SetBool("isBlocking", true);
            //InBlock = true;
        }

        if (context.canceled  && !IsAttacking) //isgrounded
        {
            //Anim.SetBool("isBlocking", false);
            //InBlock = false;
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock && !Anim.IsInTransition(0)
            && !InHitStun && !IsBeingKnockedBack)
        {
            CurrentMove = ECurrentMove.Attack1;
            IsAttacking = true;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock && !Anim.IsInTransition(0) 
            && !InHitStun  && !IsBeingKnockedBack && MoveInput == Vector2.zero)
        {
            CurrentMove = ECurrentMove.Attack2;
            IsAttacking = true;
        }
        
        if (context.performed && !IsAttacking && !InBlock && !Anim.IsInTransition(0) 
            && !InHitStun  && !IsBeingKnockedBack && MoveInput.y < 0)
        {
            CurrentMove = ECurrentMove.Attack2Lw;
            IsAttacking = true;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock && !Anim.IsInTransition(0) 
            && !InHitStun  && !IsBeingKnockedBack)
        {
            CurrentMove = ECurrentMove.SpecialN;
            IsAttacking = true;
        }
    }

    #endregion
    
    #region Player Movement
    
    private void RotateToOpponent()
    {
        Vector3 direction = (Opponent.transform.position - transform.position).normalized;

        //ensure the rotation only happens on the Y-Axis
        direction.y = 0;
        
        targetRotation = Quaternion.LookRotation(direction);

        //rotate the visual of the player to opponent
        Anim.transform.rotation =
            Quaternion.Slerp(Anim.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void PlayerMovement()
    {
        float currentSpeed = LastMovementX;
        
        if (!InBlock)
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

        LastMovementX = currentSpeed;
    }

    #endregion

    #region PlayerStateMachine Methods
    
    public void Damage(float damageAmount, float stunDuration, float hitStopDuration, Vector2 attackForce, 
        float knockBackTime, bool hasFixedKnockBack, bool isComboPossible, bool getKnockBackToOpponent, bool isPlayerAttack)
    {
        if (IsFacingRight() && MoveInput.x < 0)
        {
            InBlock = true;
            return;
        }

        if (!IsFacingRight() && MoveInput.x > 0)
        {
            InBlock = true;
            return;
        }

        if (!InHitStun)
        {
            //CanGetDamage = false;
            InHitStun = true;
            PercentageCount += damageAmount;
            HitStunDuration = stunDuration;
            HitStopDuration = hitStopDuration;
            IsComboPossible = isComboPossible;
            GetKnockBackToOpponent = getKnockBackToOpponent;
            KnockBackTime = knockBackTime;
            AttackForce = attackForce;
            GetFixedKnockBack = hasFixedKnockBack;
            
            
            //StartCoroutine(WaitDamage());
        }
        else if (InHitStun && isPlayerAttack && !InComboHit)
        {
            InComboHit = true;
            PercentageCount += damageAmount;
            HitStunDuration = stunDuration;
            HitStopDuration = hitStopDuration;
            IsComboPossible = isComboPossible;
            GetKnockBackToOpponent = getKnockBackToOpponent;
            KnockBackTime = knockBackTime;
            AttackForce = attackForce;
            GetFixedKnockBack = hasFixedKnockBack;
        }
    }

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
    
    /// <summary>
    /// check if player is facing right
    /// </summary>
    /// <returns></returns>
    public bool IsFacingRight()
    {
        return Anim.transform.forward.x > 0;
    }
    
    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    #endregion
    
    #region Animation/Sound Methods

    private void PlayerAnimations()
    {
        //Anim.SetBool("isGrounded", IsGrounded());
        
        if (IsFacingRight())
        {
            Anim.SetFloat("speed", Rb.linearVelocity.x);
        }
        else if(!IsFacingRight())
        {
            Anim.SetFloat("speed", -Rb.linearVelocity.x);
        }
        
        // Anim.SetBool("isJumping", false);
        //
        // if (Rb.linearVelocity.y < 0)
        // {
        //     //anim.SetBool("isFalling", true);
        //     Anim.SetBool("isJumping", false);
        // }
        // else
        // {
        //     //anim.SetBool("isFalling", false);
        // }
    }

    #endregion
}
