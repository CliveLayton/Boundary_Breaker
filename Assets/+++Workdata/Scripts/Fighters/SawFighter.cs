using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SawFighter : MonoBehaviour
{
    #region Variables

    //Inspector Variables
    [Header("SawFighter Behavior Variables")] 
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private LayerMask groundLayer;
    
    //public Variables
    
    //private Variables
    private float speed;
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
        PlayerMovement();
    }

    private void LateUpdate()
    {
        PlayerAnimations();
    }

    #endregion

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
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
        if (context.performed && IsGrounded())
        {
            anim.SetTrigger("Block");
            anim.SetBool("isBlocking", true);
        }

        if (context.canceled && IsGrounded())
        {
            anim.SetBool("isBlocking", false);
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            anim.SetTrigger("Jab");
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            anim.SetTrigger("HeavyAttack");
        }
    }

    #endregion

    #region Player Movement

    private void PlayerMovement()
    {
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    #endregion

    #region SawFighter Methods

    /// <summary>
    /// check if player is on the ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        bool hitGround = Physics.Raycast(transform.position, Vector3.down, 0.7f, groundLayer);
        
        return hitGround;
    }

    #endregion

    #region Animation/Sound Methods

    private void PlayerAnimations()
    {
        anim.SetBool("isGrounded", IsGrounded());
        anim.SetFloat("speed", rb.linearVelocity.x);
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
