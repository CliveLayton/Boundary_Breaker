using System.Collections;
using UnityEngine;

public class PlayerInAirState : PlayerBaseState
{
    private Coroutine pushBoxCoroutine;
    
    public PlayerInAirState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        if (pushBoxCoroutine == null && Ctx.IsAbovePlayer())
        {
            pushBoxCoroutine = Ctx.StartCoroutine(HandlePushbox());
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpedPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }

        Ctx.CanDash = true;
        Ctx.IsAttacking = false;
        Ctx.Opponent.InBlock = false;

        //prevent sliding if you enter ground 
        Ctx.Rb.linearVelocity = new Vector2(0, Ctx.Rb.linearVelocity.y);
        Ctx.LastMovementX = 0; 
    }
    
    public override void InitializeSubState()
    {
        if (Ctx.IsJumpedPressed && !Ctx.IsDashing)
        {
            SetSubState(Factory.Jump());
        }
        else if(Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SetSubState(Factory.Dash());
        }
        else if (Ctx.IsAttacking && !Ctx.IsDashing)
        {
            SetSubState(Factory.Attack());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded() && Ctx.Rb.linearVelocity.y < 0.5f && !Ctx.InHitStun && !Ctx.InGrab && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Grounded());
        }
        else if(Ctx.InHitStun || Ctx.InGrab)
        {
            SwitchState(Factory.Stunned());
        }
    }

    private IEnumerator HandlePushbox()
    {
        Ctx.Pushbox.enabled = false;
        yield return new WaitForSeconds(0.2f);
        Ctx.Pushbox.enabled = true;
        pushBoxCoroutine = null;
    }
}
