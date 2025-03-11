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
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    //[SerializeField] private float jumpPower = 5f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float knockbackPower = 10f;
    [SerializeField] private LayerMask whoToDamage;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject opponent;
    [SerializeField] private CinemachineImpulseSource cmImpulse;
    
    //public Variables
    public bool inAttack = false;
    public bool inBlock = false;
    
    //private Variables
    private float speed;
    private bool pressedSpecial = false;
    private bool inHitStun = false;
    private float hitStunDuration = 0f;
    private bool canGetDamage = true;
    private Quaternion targetRotation;
    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator anim;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        speed = normalSpeed;
    }

    private void FixedUpdate()
    {
        if (!inAttack && !inBlock)
        {
            PlayerMovement();  
        }

        if (!inAttack)
        {
            RotateToOpponent();
        }

        if (pressedSpecial && !inAttack)
        {
            anim.SetTrigger("Special");
            inAttack = true;
        }
    }

    private void LateUpdate()
    {
        PlayerAnimations();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((whoToDamage.value & (1 << other.gameObject.layer)) > 0)
        {
            IDamageable iDamageable = other.gameObject.GetComponentInParent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(2, 0.2f, 0.05f);
            }
        }
    }

    #endregion

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && !inAttack && !inBlock)
        {
            //rb.AddForce(new Vector2(rb.linearVelocity.x, jumpPower), ForceMode.Impulse);
            anim.SetBool("isJumping", true);
        }
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
            anim.SetTrigger("Jab");
            inAttack = true;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !inAttack && !inBlock)
        {
            anim.SetTrigger("HeavyAttack");
            inAttack = true;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !pressedSpecial)
        {
            pressedSpecial = true;
            StartCoroutine(ButtonBuffer());
        }
        
        if (context.performed && !inAttack && !inBlock)
        {
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
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    #endregion

    #region SawFighter Methods
    
    public void Damage(int damageAmount, float stunDuration, float hitStopDuration)
    {
        if (canGetDamage && !inHitStun)
        {
            anim.SetTrigger("HeavyHit");
            canGetDamage = false;
            inHitStun = true;
            hitStunDuration = stunDuration;

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
        rb.AddForce(new Vector2(knockbackPower * -directionToOpponent.x, knockbackPower * 0.06f), ForceMode.Impulse);
        StartCoroutine(KnockbackDecay());
    }
    
    IEnumerator KnockbackDecay()
    {
        float decayTime = 0.3f; // Time to stop knockback
        float elapsedTime = 0f;

        while (elapsedTime < decayTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        inAttack = false;
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
    
    private IEnumerator ButtonBuffer()
    {
        yield return new WaitForSeconds(0.3f);
        pressedSpecial = false;
    }
    
    /// <summary>
    /// check if player is on the ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        bool hitGround = Physics.Raycast(transform.position, Vector3.down, 0.7f, groundLayer);
        
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
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }
    }

    #endregion
}
