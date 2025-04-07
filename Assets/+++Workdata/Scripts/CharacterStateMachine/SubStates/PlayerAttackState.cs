using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Move.OnHitFrameEnd();
        Ctx.Move.SetMove((int)Ctx.CurrentMove);
        Ctx.Rb.linearVelocity = new Vector2(0, Ctx.Rb.linearVelocity.y);
        Ctx.LastMovementX = 0;
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
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Dash());
        }
    }

    public override void InitializeSubState()
    {
        
    }
}
