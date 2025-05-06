using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
       
    }
    
    public override void EnterState()
    {
        Ctx.Rb.linearVelocity = Vector3.zero;
        if (Ctx.IsFacingRight() && Ctx.MoveInput.x >= 0)
        {
            Ctx.Anim.Play("DashForward");
            Ctx.Rb.AddForce(new Vector2(Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = Ctx.DashPower;
        }
        else if (Ctx.IsFacingRight() && Ctx.MoveInput.x < -0.3f)
        {
            Ctx.Anim.Play("DashBack");
            Ctx.Rb.AddForce(new Vector2(-Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = -Ctx.DashPower;
        }
        else if(!Ctx.IsFacingRight() && Ctx.MoveInput.x <= 0)
        {
            Ctx.Anim.Play("DashForward");
            Ctx.Rb.AddForce(new Vector2(-Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = -Ctx.DashPower;
        }
        else if (!Ctx.IsFacingRight() && Ctx.MoveInput.x > 0.3f)
        {
            Ctx.Anim.Play("DashBack");
            Ctx.Rb.AddForce(new Vector2(Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = Ctx.DashPower;
        }

        Ctx.IsDashing = false;
    }

    public override void UpdateState()
    {
        Ctx.Rb.linearVelocity = new Vector2(Ctx.Rb.linearVelocity.x, 0);
        if (Ctx.Anim.NormalizedTime(0) > 0.9f)
        {
            CheckSwitchStates();  
        }
    }

    public override void ExitState()
    {
        if (Ctx.IsGrounded())
        {
            Ctx.CanDash = true; 
        }
    }
    
    public override void InitializeSubState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing)
        {
            SwitchState(Factory.Walk());
        }
    }
}
