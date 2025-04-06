using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        InitializeSubState();
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Enter Jump");
        Ctx.Rb.linearVelocity = new Vector2(Ctx.Rb.linearVelocity.x, Ctx.JumpPower);
    }

    public override void UpdateState()
    {
        HandleJump();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpedPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }
    
    public override void InitializeSubState()
    {
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Dash());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded() && Ctx.Rb.linearVelocity.y < 0.05f)
        {
            SwitchState(Factory.Grounded());
        }
    }
    
    private void HandleJump()
    {

        if (!Ctx.IsJumpedPressed && Ctx.Rb.linearVelocity.y > 0)
        {
            Ctx.Rb.linearVelocity = new Vector2(Ctx.Rb.linearVelocity.x, Ctx.Rb.linearVelocity.y * 0.5f);
        }

        if (Ctx.Rb.linearVelocity.y < 0)
        {
            Ctx.Rb.linearVelocity += Vector3.up * (Ctx.FallMultiplier * Physics.gravity.y * Time.deltaTime);
        }
        
    }
}
