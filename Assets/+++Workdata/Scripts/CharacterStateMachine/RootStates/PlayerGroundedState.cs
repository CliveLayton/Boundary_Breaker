using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Enter Grounded");
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
        Debug.Log("Enter InitSub Ground");
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
        if (Ctx.IsJumpedPressed && !Ctx.RequireNewJumpPress && !Ctx.InHitStun && !Ctx.IsAttacking)
        {
            SwitchState(Factory.InAir());
        }
        else if(Ctx.InHitStun)
        {
            SwitchState(Factory.Stunned());
        }
    }
}
