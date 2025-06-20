using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerStateMachine : MonoBehaviour, IDamageable, IGrabable
{
    #region Variables

    //Inspector Variables
    [Header("SawFighter Behavior Variables")]
    [SerializeField] private Quaternion lookDirectionToLeft;
    [SerializeField] private Quaternion lookDirectionToRight;
    [SerializeField] private float rotationSpeed = 60f;
    //[SerializeField] private float knockbackPower = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask pushboxLayer;
    [SerializeField] private LayerMask p1HurtboxLayer;
    [SerializeField] private LayerMask p2HurtboxLayer;
    [SerializeField] private int p1HitboxLayerNumber;
    [SerializeField] private int p2HitboxLayerNumber;
    [SerializeField] private int p1HurtboxLayerNumber;
    [SerializeField] private int p2HurtboxLayerNumber;

    [Header("Hurtboxes")] 
    [SerializeField] private BoxCollider[] hurtboxes;

    [Header("Hitboxes")] 
    [SerializeField] private Hitbox[] hitboxes;

    //public Variables
    public event Action<float> onPercentageChanged; 

    //private Variables
    // private float jumpCancelBufferTimer = -1f;
    // private float jumpCancelBufferDuration = 0.5f;
    private Quaternion targetRotation;
    private PlayerStateFactory states;
    
    //getters and setters
    [field: SerializeField] public float PercentageCount { get; set; }
    [field: SerializeField] public float ForwardSpeed { get; private set; }
    [field: SerializeField] public float BackwardSpeed { get; private set; }
    [field: SerializeField] public float SpeedChangeRate { get; private set; }
    [field: SerializeField] public float DashPower { get; private set; }
    [field: SerializeField] public float JumpPower { get; private set; }
    [field: SerializeField] public float JumpBrake { get; private set; }
    [field: SerializeField] public float FallMultiplier { get; private set; }
    [field: SerializeField] public Vector2 InputForce { get; set; }
    [field: SerializeField] public AnimationCurve KnockBackForceCurve { get; private set; }
    [field: SerializeField] public CapsuleCollider Pushbox { get; set; }
    [field: SerializeField] public Transform GrabPosition { get; private set; }
    [field: SerializeField] public Transform CameraPoint { get; private set; }

    public PlayerBaseState CurrentState { get; set; }
    public int PlayerIndex { get; set; }
    public Rigidbody Rb { get; private set; }
    public  Animator Anim { get; private set; }
    public PlayerStateMachine Opponent { get; set; }
    public CinemachineImpulseSource CmImpulse { get; private set; }
    public ECurrentMove CurrentMove { get; set; }
    public  Vector2 MoveInput { get; private set; }
    public float Speed { get; set; }
    public float LastMovementX { get; set; }
    public bool IsJumpedPressed { get; private set; }
    public bool RequireNewJumpPress { get; set; }
    public bool HasJumpCanceled { get; private set; }
    //public bool BufferedJumpCancel => Time.unscaledTime <= jumpCancelBufferTimer;
    public bool IsDashing { get; set; }
    public bool InBlock { get; set; }
    [field: SerializeField] public bool IsAttacking { get; set; }
    [field: SerializeField] public bool IsGrabbing { get; set; }
    public bool InGrab { get; set; }
    public bool IsThrowing { get; set; }
    public bool CanCombo { get; set; }
    public  bool InHitStun { get; set; }
    public bool InComboHit { get; set; }
    public float KnockBackTime { get; private set; }
    public Vector2 AttackForce { get; private set; }
    public Vector2 CombinedForce { get; set; }
    public bool GetFixedKnockBack { get; private set; }
    public float HitStunDuration { get; private set; }
    public float HitStopDuration { get; private set; }
    public bool IsComboPossible { get; private set; }
    public bool GetKnockBackToOpponent { get; private set; }
    public bool IsBeingKnockedBack { get; set; }
    public bool InKnockdown { get; set; }
    public bool CanDash { get; set; } = true;
    public bool DidDI { get; set; }
    public Vector2 DefaultInputForce { get; private set; }
    [field: SerializeField] public CharacterMoves[] Moves { get; private set; }


    #endregion
    
    public enum ECurrentMove
    {
        Attack1,
        Attack1Air,
        Attack2,
        Attack2Air,
        SpecialN,
        SpecialAir,
        Grab,
        Throw
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
        Anim = GetComponent<Animator>();
        Speed = ForwardSpeed;
        DefaultInputForce = InputForce;
        
        //setup states
        states = new PlayerStateFactory(this);
        CurrentState = states.Grounded();
        CurrentState.EnterState();
        GameStateManager.Instance.onStateChanged += GetCinemachineListener;
    }

    private void Start()
    {
        CmImpulse = FindAnyObjectByType<CinemachineImpulseSource>();
    }

    private void Update()
    {
        CurrentState.UpdateStates();
    }

    private void FixedUpdate()
    {
        if (!IsAttacking && GameStateManager.Instance.currentState == GameStateManager.GameState.InGame)
        {
            RotateToOpponent();
        }
    }

    private void LateUpdate()
    {
        PlayerAnimations();
        //just for backup if a player falls through the stage
        if (transform.position.y <= -1)
        {
            GameReferee gameReferee = FindAnyObjectByType<GameReferee>();
            StartCoroutine(gameReferee.RestartGame(-1, 0.2f));
        }
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.onStateChanged -= GetCinemachineListener;
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
        if (CanDash && context.performed && !InBlock)
        {
            CanDash = false;
            IsDashing = true;
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock
            && !InHitStun && !IsBeingKnockedBack && IsGrounded() && !IsJumpedPressed)
        {
            if (Mathf.Abs(MoveInput.y) <= 0.3f && Mathf.Abs(MoveInput.x) <= 0.3f)
            {
                CurrentMove = ECurrentMove.Attack1;
                IsAttacking = true;
            }
            // else if (MoveInput.y <= -0.5f && Mathf.Abs(MoveInput.x) < 0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack1Lw;
            //     IsAttacking = true;
            // }
            // else if (IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x > 0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack1S;
            //     IsAttacking = true;
            // }
            // else if (!IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x < -0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack1S;
            //     IsAttacking = true;
            // }
        } else if (context.performed && !IsAttacking && !InBlock
                   && !InHitStun && !IsBeingKnockedBack && !IsGrounded())
        {
            CurrentMove = ECurrentMove.Attack1Air;
            IsAttacking = true;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock 
            && !InHitStun  && !IsBeingKnockedBack && IsGrounded())
        {
            if (Mathf.Abs(MoveInput.y) <= 0.3f && Mathf.Abs(MoveInput.x) <= 0.3f)
            {
                CurrentMove = ECurrentMove.Attack2;
                IsAttacking = true;
            }
            // else if (MoveInput.y <= -0.5f && Mathf.Abs(MoveInput.x) < 0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack2Lw;
            //     IsAttacking = true;
            // }
            // else if (IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x > 0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack2S;
            //     IsAttacking = true;
            // }
            // else if (!IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x < -0.5f)
            // {
            //     CurrentMove = ECurrentMove.Attack2S;
            //     IsAttacking = true;
            // }
        } else if (context.performed && !IsAttacking && !InBlock
                  && !InHitStun && !IsBeingKnockedBack && !IsGrounded())
        {
            CurrentMove = ECurrentMove.Attack2Air;
            IsAttacking = true;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock 
            && !InHitStun  && !IsBeingKnockedBack && IsGrounded() && !IsJumpedPressed)
        {
            if (Mathf.Abs(MoveInput.y) <= 0.3f && Mathf.Abs(MoveInput.x) <= 0.3f)
            {
                CurrentMove = ECurrentMove.SpecialN;
                IsAttacking = true;
            }
            // else if (MoveInput.y <= -0.5f && Mathf.Abs(MoveInput.x) < 0.5f)
            // {
            //     CurrentMove = ECurrentMove.SpecialLw;
            //     IsAttacking = true;
            // }
            // else if (IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x > 0.5f)
            // {
            //     CurrentMove = ECurrentMove.SpecialS;
            //     IsAttacking = true;
            // }
            // else if (!IsFacingRight() && Mathf.Abs(MoveInput.y) <= 0.3f && MoveInput.x < -0.5f)
            // {
            //     CurrentMove = ECurrentMove.SpecialS;
            //     IsAttacking = true;
            // }
        } else if (context.performed && !IsAttacking && !InBlock
                   && !InHitStun && !IsBeingKnockedBack && !IsGrounded())
        {
            CurrentMove = ECurrentMove.SpecialAir;
            IsAttacking = true;
        }
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.performed && !IsAttacking && !InBlock
            && !InHitStun && !IsBeingKnockedBack && IsGrounded() && !CanCombo)
        {
            CurrentMove = ECurrentMove.Grab;
            IsAttacking = true;
            IsGrabbing = true;
        }
    }

    public void OnPauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIManager.Instance.EnterPauseMenu(PlayerIndex);
        }
    }

    #endregion
    
    #region Player Movement
    
    private void RotateToOpponent()
    {
        if (Opponent.transform.position.x > transform.position.x)
        {
            Anim.transform.rotation = Quaternion.Slerp(Anim.transform.rotation, lookDirectionToRight,
                rotationSpeed * Time.deltaTime);
        }
        else
        {
            Anim.transform.rotation = Quaternion.Slerp(Anim.transform.rotation, lookDirectionToLeft,
                rotationSpeed * Time.deltaTime);
        }
        
        // Vector3 direction = (Opponent.transform.position - transform.position).normalized;
        //
        // //ensure the rotation only happens on the Y-Axis
        // direction.y = 0;
        //
        // targetRotation = Quaternion.LookRotation(direction);
        //
        // //rotate the visual of the player to opponent
        // Anim.transform.rotation =
        //     Quaternion.Slerp(Anim.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
    
    public void Damage(float damageAmount, float stunDuration, float hitStopDuration, Vector2 attackForce, float knockBackTime, 
        bool hasFixedKnockBack, bool isComboPossible, bool getKnockBackToOpponent, bool isPlayerAttack, bool applyKnockDown)
    {
        if (IsFacingRight() && MoveInput.x < 0 && !InGrab && CanDash && isPlayerAttack)
        {
            MusicManager.Instance.PlayInGameSFX(MusicManager.Instance.onBlockedSounds[Random.Range(0, MusicManager.Instance.onBlockedSounds.Length)]);
            InBlock = true;
            return;
        }

        if (!IsFacingRight() && MoveInput.x > 0 && !InGrab && CanDash && isPlayerAttack)
        {
            MusicManager.Instance.PlayInGameSFX(MusicManager.Instance.onBlockedSounds[Random.Range(0, MusicManager.Instance.onBlockedSounds.Length)]);
            InBlock = true;
            return;
        }

        if (!isPlayerAttack && IsFacingRight())
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, Vector3.left, out hit, 10f, groundLayer);
            var holoWall = hit.transform.GetComponent<HoloWall>();
            if (holoWall != null)
            {
                holoWall.GetImpact(hit);
            }
        }
        else if (!isPlayerAttack && !IsFacingRight())
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, Vector3.right, out hit, 10f, groundLayer);
            var holoWall = hit.transform.GetComponent<HoloWall>();
            if (holoWall != null)
            {
                holoWall.GetImpact(hit);
            }
        }

        if (!InHitStun)
        {
            InHitStun = true;
        }
        else if (InHitStun && isPlayerAttack && !InComboHit)
        {
            InComboHit = true;
        }
        
        PercentageCount += damageAmount;
        if (onPercentageChanged != null)
        {
            onPercentageChanged(PercentageCount);
        }
        HitStunDuration = stunDuration;
        HitStopDuration = hitStopDuration;
        IsComboPossible = isComboPossible;
        GetKnockBackToOpponent = getKnockBackToOpponent;
        KnockBackTime = knockBackTime;
        AttackForce = attackForce;
        GetFixedKnockBack = hasFixedKnockBack;
        InKnockdown = applyKnockDown;
        MusicManager.Instance.PlayInGameSFX(MusicManager.Instance.onHitSounds[Random.Range(0, MusicManager.Instance.onHitSounds.Length)]);
    }
    
    /// <summary>
    /// Set the player in grab state if possible and set his position to the new position
    /// </summary>
    /// <param name="newPosition"></param>
    public void Grabbed(Vector2 newPosition)
    {
        InGrab = true;
        transform.position = newPosition;
    }

    /// <summary>
    /// Handle if the character can do a combo or not
    /// </summary>
    /// <param name="isComboTime"></param>
    /// <param name="isJumpCancel">Check if the opponent is jumping out the combo</param>
    public void HandleCombo(bool isComboTime, bool isJumpCancel)
    {
        if (isJumpCancel)
        {
            HasJumpCanceled = true;
            StartCoroutine(JumpCancelCooldown());
            IsAttacking = isComboTime;
        }
        else
        {
            IsAttacking = !isComboTime;
        }
        CanCombo = isComboTime;
    }

    /// <summary>
    /// Handle if the hurtboxes of the player should be active or not
    /// </summary>
    /// <param name="active"></param>
    public void HandleHurtboxes(bool active)
    {
        for (int i = 0; i < hurtboxes.Length; i++)
        {
            hurtboxes[i].enabled = active;
        }
    }

    /// <summary>
    /// handle if the rigidbody should be freezed completely or switch to default
    /// </summary>
    /// <param name="freeze"></param>
    public void HandleRbFreeze(bool freeze)
    {
        if (freeze)
        {
            Rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            Rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }

    private IEnumerator JumpCancelCooldown()
    {
        yield return new WaitForSeconds(0.05f);
        HasJumpCanceled = false;
    }

    /// <summary>
    /// check if player is on the ground
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        bool hitGround = Physics.Raycast(transform.position + new Vector3(0,0.1f,0), Vector3.down, 0.2f, groundLayer);
        
        return hitGround;
    }

    /// <summary>
    /// check if player is above other player
    /// </summary>
    /// <returns></returns>
    public bool IsAbovePlayer()
    {
        RaycastHit hit;
        bool hitPlayer = Physics.SphereCast(transform.position + new Vector3(0,0.5f, 0), 0.3f,Vector3.down, out hit, 0.6f, pushboxLayer);
        
        return hitPlayer;
    }

    // private void OnDrawGizmos()
    // {
    //     Vector3 startPos = transform.position + new Vector3(0, 0.5f, 0);
    //     Gizmos.DrawWireSphere(startPos, 0.3f);
    //     Gizmos.DrawLine(startPos, startPos + 0.6f * Vector3.down);
    //     Gizmos.DrawWireSphere(startPos + (0.6f * Vector3.down), 0.3f);
    // }

    /// <summary>
    /// check if player is facing right
    /// </summary>
    /// <returns></returns>
    public bool IsFacingRight()
    {
        return Anim.transform.forward.x > 0;
    }

    public void SetLayers(int index)
    {
        switch (index)
        {
            case 0:
                foreach (var hurtbox in hurtboxes)
                {
                    hurtbox.gameObject.layer = p1HurtboxLayerNumber;
                }

                foreach (var hitbox in hitboxes)
                {
                    hitbox.layerToCheck = p2HurtboxLayer;
                    hitbox.gameObject.layer = p1HitboxLayerNumber;
                }
                break;
            case 1:
                foreach (var hurtbox in hurtboxes)
                {
                    hurtbox.gameObject.layer = p2HurtboxLayerNumber;
                }

                foreach (var hitbox in hitboxes)
                {
                    hitbox.layerToCheck = p1HurtboxLayer;
                    hitbox.gameObject.layer = p2HitboxLayerNumber;
                }
                break;
        }
    }

    public void ResetCharacter()
    {
        LastMovementX = 0;
        MoveInput = Vector2.zero;
        Rb.linearVelocity = Vector3.zero;
        PercentageCount = 0f;
        if (onPercentageChanged != null)
        {
            onPercentageChanged(PercentageCount);
        }
        if (PlayerIndex == 0)
        {
            transform.SetParent(CharacterPool.Instance.Player1PoolParent);
        }
        else
        {
            transform.SetParent(CharacterPool.Instance.Player2PoolParent);
        }
    }

    private void GetCinemachineListener(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.InGame)
        {
            CmImpulse = FindAnyObjectByType<CinemachineImpulseSource>();
        }
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
