using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Anim.Play("Blocking");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
        
    }
}
