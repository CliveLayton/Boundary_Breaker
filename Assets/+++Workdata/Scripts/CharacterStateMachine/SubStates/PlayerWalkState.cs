using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    
    public override void EnterState()
    {
        if (Ctx.IsFacingRight() && Ctx.IsGrounded())
        {
            if (Ctx.Rb.linearVelocity.x > 0)
            {
                Ctx.Anim.Play("Walk_Forward");
            }
            else
            {
                Ctx.Anim.Play("Walk_Back");
            }
        }
        else if (!Ctx.IsFacingRight() && Ctx.IsGrounded())
        {
            if (Ctx.Rb.linearVelocity.x < 0)
            {
                Ctx.Anim.Play("Walk_Forward");
            }
            else
            {
                Ctx.Anim.Play("Walk_Back");
            }
        }
    }

    public override void UpdateState()
    {
        if (Ctx.IsFacingRight() && Ctx.MoveInput.x > 0)
        {
            Ctx.Speed = Ctx.ForwardSpeed;
        }
        else if (Ctx.IsFacingRight() && Ctx.MoveInput.x < 0)
        {
            Ctx.Speed = Ctx.BackwardSpeed;
        }
        
        if (!Ctx.IsFacingRight() && Ctx.MoveInput.x > 0)
        {
            Ctx.Speed = Ctx.ForwardSpeed;
        }
        else if (!Ctx.IsFacingRight() && Ctx.MoveInput.x < 0)
        {
            Ctx.Speed = Ctx.BackwardSpeed;
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
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.IsAttacking && !Ctx.InBlock)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsDashing && !Ctx.IsAttacking && !Ctx.InBlock)
        {
            SwitchState(Factory.Dash());
        }
        else if(Ctx.IsAttacking && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Attack());
        }
        else if(Ctx.InBlock && !Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SwitchState(Factory.Block());
        }
    }
}
