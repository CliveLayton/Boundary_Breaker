using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        Debug.Log("Enter Walk");
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
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsDashing)
        {
            SwitchState(Factory.Dash());
        }
    }
}
