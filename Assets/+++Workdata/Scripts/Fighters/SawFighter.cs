using System.Collections;
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
    public bool inAttack = false;
    public bool inBlock = false;
    
    //private Variables
    private float speed;
    private bool pressedSpecial = false;
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

    private void PlayerMovement()
    {
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    #endregion

    #region SawFighter Methods

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
