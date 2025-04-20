using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        if (Ctx.IsFacingRight())
        {
            Ctx.Rb.AddForce(new Vector2(Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = Ctx.DashPower;
        }
        else
        {
            Ctx.Rb.AddForce(new Vector2(-Ctx.DashPower, 0), ForceMode.Impulse);
            Ctx.LastMovementX = -Ctx.DashPower;
        }
        
        Ctx.IsDashing = false;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
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
