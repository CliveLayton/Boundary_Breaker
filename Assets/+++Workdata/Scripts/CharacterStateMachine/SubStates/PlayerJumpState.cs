using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Speed = Ctx.ForwardSpeed;
        Ctx.Anim.Play("Jump");
        Ctx.Rb.linearVelocity = new Vector2(Ctx.Rb.linearVelocity.x, Ctx.JumpPower);
    }

    public override void UpdateState()
    {
        Ctx.PlayerMovement();
        HandleJump();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if(Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Dash());
        }
        else if (Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SwitchState(Factory.Attack());
        }
    }

    public override void InitializeSubState()
    {
        
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
