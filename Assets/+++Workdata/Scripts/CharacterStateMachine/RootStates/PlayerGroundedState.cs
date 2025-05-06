using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        InitializeSubState();
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
        if (Ctx.IsJumpedPressed && !Ctx.RequireNewJumpPress && !Ctx.InHitStun && !Ctx.InGrab && !Ctx.IsAttacking)
        {
            SwitchState(Factory.InAir());
        }
        else if(Ctx.InHitStun || Ctx.InGrab)
        {
            //reset dash if you got hit while dashing
            Ctx.CanDash = true;
            SwitchState(Factory.Stunned());
        }
    }
}
