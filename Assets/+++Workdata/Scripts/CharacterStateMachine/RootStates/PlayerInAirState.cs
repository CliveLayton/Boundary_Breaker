using UnityEngine;

public class PlayerInAirState : PlayerBaseState
{
    public PlayerInAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        InitializeSubState();
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
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SetSubState(Factory.Walk());
        }
        else if(Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SetSubState(Factory.Dash());
        }
        else if (Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SetSubState(Factory.Attack());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded() && Ctx.Rb.linearVelocity.y < 0.05f && !Ctx.InHitStun)
        {
            SwitchState(Factory.Grounded());
        }
        else if(Ctx.InHitStun)
        {
            SwitchState(Factory.Stunned());
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
