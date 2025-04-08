using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        if (Ctx.IsGrounded())
        {
            Debug.Log("Enter Ground Idle");
            Ctx.Anim.Play("Idle");
        }
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
        if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Dash());
        }
        else if (Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SwitchState(Factory.Attack());
        }
    }
}
