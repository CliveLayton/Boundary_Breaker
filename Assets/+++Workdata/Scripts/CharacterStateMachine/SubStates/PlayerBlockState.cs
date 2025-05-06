using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Anim.Play("Blocking");
        Ctx.Rb.linearVelocity = Vector3.zero;
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
        Debug.Log("check");
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            Debug.Log("Idle");
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            Debug.Log("Walk");
            SwitchState(Factory.Walk());
        }
    }

    public override void InitializeSubState()
    {
        
    }
}
