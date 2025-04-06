using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        InitializeSubState();
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Debug.Log("Enter Grounded");
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
        if (Ctx.IsJumpedPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }
    }
}
