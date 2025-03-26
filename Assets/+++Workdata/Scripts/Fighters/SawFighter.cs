using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SawFighter : MonoBehaviour, IDamageable
{
    #region Variables

    //Inspector Variables
    [Header("Player Index")] 
    [SerializeField] private int playerIndex;

    [Header("SawFighter Behavior Variables")] 
    [SerializeField] private float percentageCount = 0;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpPower = 4f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 60f;
    //[SerializeField] private float knockbackPower = 10f;
    [SerializeField] private AttackSawFighter attack;
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
    private float speed;
    private float knockBackTime;
    private Vector2 attackForce;
    private bool isJumping = false;
    private bool isJumpedPressed = false;
    private bool inHitStun = false;
    private float hitStunDuration = 0f;
    private bool canGetDamage = true;
    private bool isBeingKnockedBack = false;
    private bool getFixedKnockBack;
    private Quaternion targetRotation;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator anim;

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
    

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        speed = normalSpeed;
    }

    private void FixedUpdate()
    {
        if (!inAttack && !inBlock && !isBeingKnockedBack)
        {
            PlayerMovement(); 
            HandleJump();
        }

        if (!inAttack)
        {
            RotateToOpponent();
        }
        
    }

    private void LateUpdate()
    {
        PlayerAnimations();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if ((whoToDamage.value & (1 << other.gameObject.layer)) > 0)
    //     {
    //         IDamageable iDamageable = other.gameObject.GetComponentInParent<IDamageable>();
    //         if (iDamageable != null)
    //         {
    //             iDamageable.Damage(2, 0.2f, 0.05f);
    //         }
    //     }
    // }

    #endregion

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpedPressed = context.ReadValueAsButton();
        // if (context.performed && IsGrounded() && !inAttack && !inBlock)
        // {
        //     //rb.AddForce(new Vector2(rb.linearVelocity.x, jumpPower), ForceMode.Impulse);
        //     anim.SetBool("isJumping", true);
        // }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speed = sprintSpeed;
        }

        if (context.canceled)
        {
            speed = normalSpeed;
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !inAttack && !inBlock)
        {
            anim.SetTrigger("Block");
            anim.SetBool("isBlocking", true);
            inBlock = true;
        }

        if (context.canceled && IsGrounded() && !inAttack)
        {
            anim.SetBool("isBlocking", false);
            inBlock = false;
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            attack.SetMove((int)CurrentMove.Attack1);
            anim.SetTrigger("Jab");
            inAttack = true;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            attack.SetMove((int)CurrentMove.Attack2);
            anim.SetTrigger("HeavyAttack");
            inAttack = true;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            attack.SetMove((int)CurrentMove.SpecialN);
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

    private void PlayerMovement()
    {
        if (moveInput.x != 0)
        {
            rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, rb.linearVelocity.z); 
        }
        
    }

    private void HandleJump()
    {
        if (!isJumping && IsGrounded() && isJumpedPressed)
        {
            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
        else if (isJumping && IsGrounded() && !isJumpedPressed)
        {
            isJumping = false;
        }

        if (isJumping && !isJumpedPressed && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * (fallMultiplier * Physics.gravity.y * Time.deltaTime);
        }
        
    }

    #endregion

    #region SawFighter Methods

    public void Damage(float damageAmount, float stunDuration, float hitStopDuration, Vector2 attackForce, float knockBackTime, bool hasFixedKnockBack)
    {
        if (canGetDamage && !inHitStun)
        {
            anim.SetTrigger("HeavyHit");
            canGetDamage = false;
            inHitStun = true;
            hitStunDuration = stunDuration;
            percentageCount += damageAmount;
            this.knockBackTime = knockBackTime;
            this.attackForce = attackForce;
            getFixedKnockBack = hasFixedKnockBack;

            rb.linearVelocity = Vector3.zero; //Reset movement

            //Apply HitStop Before Knockback
            StartCoroutine(HitStop(hitStopDuration));

            StartCoroutine(HitStunCoroutine());
            StartCoroutine(WaitDamage());
        }
    }

    private IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f; //freeze game time
        cmImpulse.GenerateImpulse();
        yield return new WaitForSecondsRealtime(duration); // wait for real-world time
        Time.timeScale = 1f; //resume game time
    }

    private IEnumerator HitStunCoroutine()
    {
        float timer = 0f;
        
        //Disable movement
        SawFighter playerController = GetComponent<SawFighter>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Vector3 originalPosition = transform.localPosition;

        while (timer < hitStunDuration)
        {
            //shake effect
            float shakeAmount = 0.05f;
            transform.localPosition = originalPosition + new Vector3(Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount), 0);
            
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition; // reset position
        
        //Enable movement after hit stun ends
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        inHitStun = false;
        
        //knockback
        Vector3 directionToOpponent = (opponent.transform.position - this.transform.position).normalized;
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(KnockbackAction(-directionToOpponent, Vector2.up, moveInput.x));
        //rb.AddForce(new Vector2(knockbackPower * -directionToOpponent.x, knockbackPower * 0.3f), ForceMode.Impulse);
        //StartCoroutine(KnockbackDecay());
    }
    
    private IEnumerator KnockbackDecay()
    {
        float decayTime = 0.3f; // Time to stop knockback
        float elapsedTime = 0f;

        while (elapsedTime < decayTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isBeingKnockedBack = false;
        inAttack = false;
    }

    private IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        isBeingKnockedBack = true;

        Vector2 hitForce;
        Vector2 constantKnockBackForce;
        Vector2 knockBackForce;
        Vector2 combinedForce;
        
        constantKnockBackForce = constantForceDirection * attackForce.y;

        float elapsedTime = 0f;
        while (elapsedTime < knockBackTime)
        {
            //iterate the timer
            elapsedTime += Time.fixedDeltaTime;
            
            //update hitForce
            hitForce = hitDirection * (attackForce.x * knockBackForceCurve.Evaluate(elapsedTime));
            
            //combine hitForce and constantForce
            knockBackForce = hitForce + constantKnockBackForce;
            
            //combine knockBackForce with Input Force
            if (inputDirection != 0 && !getFixedKnockBack)
            {
                combinedForce = new Vector2(knockBackForce.x * (percentageCount * 0.35f),
                    knockBackForce.y * (percentageCount * 0.15f)) + new Vector2(inputDirection * inputForce, 0f);
            }
            else if(!getFixedKnockBack)
            {
                combinedForce = new Vector2(knockBackForce.x * (percentageCount * 0.35f),
                    knockBackForce.y * (percentageCount * 0.15f));
            }
            else if (inputDirection != 0 && getFixedKnockBack)
            {
                combinedForce = knockBackForce + new Vector2(inputDirection * inputForce, 0);
            }
            else
            {
                combinedForce = knockBackForce;
            }
            
            //apply knockBack
            rb.linearVelocity = combinedForce;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(KnockbackDecay());
    }

    private IEnumerator WaitDamage()
    {
        yield return null;
        canGetDamage = true;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    /// <summary>
    /// check if player is on the ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        bool hitGround = Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayer);
        
        return hitGround;
    }

    /// <summary>
    /// check if player is facing right
    /// </summary>
    /// <returns></returns>
    private bool isFacingRight()
    {
        return anim.transform.forward.x > 0;
    }

    #endregion

    #region Animation/Sound Methods

    private void PlayerAnimations()
    {
        anim.SetBool("isGrounded", IsGrounded());
        if (isFacingRight())
        {
            anim.SetFloat("speed", rb.linearVelocity.x);
        }
        else if(!isFacingRight())
        {
            anim.SetFloat("speed", -rb.linearVelocity.x);
        }
        anim.SetBool("isJumping", false);

        if (rb.linearVelocity.y < 0)
        {
            //anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
        }
        else
        {
            //anim.SetBool("isFalling", false);
        }
    }

    #endregion
}
