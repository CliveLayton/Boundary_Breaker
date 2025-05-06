using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        Ctx.Anim.Play("Idle");
    }

    public override void UpdateState()
    {
        if (!Ctx.IsGrounded())
        {
            if (Ctx.Rb.linearVelocity.y < 0)
            {
                Ctx.Rb.linearVelocity += Vector3.up * (Ctx.FallMultiplier * Physics.gravity.y * Time.deltaTime);
            }
        }
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
        if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.IsAttacking && !Ctx.InBlock)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsDashing && !Ctx.IsAttacking && !Ctx.InBlock)
        {
            SwitchState(Factory.Dash());
        }
        else if (Ctx.IsAttacking && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Attack());
        }
        else if(Ctx.InBlock && !Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SwitchState(Factory.Block());
        }
    }
}
