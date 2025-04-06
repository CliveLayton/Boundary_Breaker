using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        Debug.Log("Enter Idle");
    }

    public override void UpdateState()
    {
       Ctx.PlayerMovement();
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
        if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsDashing)
        {
            SwitchState(Factory.Dash());
        }
    }
}
