using System.Collections;
using UnityEngine;

public class PlayerStunnedState : PlayerBaseState
{
    public PlayerStunnedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Rb.linearVelocity = Vector3.zero; //reset velocity
        InitializeSubState();
    }

    public override void UpdateState()
    {
        //keep velocity on zero while stunned
        Ctx.Rb.linearVelocity = Vector3.zero;
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void InitializeSubState()
    {
        if (Ctx.InHitStun && !Ctx.InGrab)
        {
            SetSubState(Factory.HitStun());
        }
        else if (Ctx.InGrab && !Ctx.InHitStun && !Ctx.InKnockdown)
        {
            SetSubState(Factory.Grabbed());
        }
        else if (Ctx.InKnockdown && !Ctx.InHitStun && !Ctx.InGrab)
        {
            SetSubState(Factory.Knockdown());
        }
    }
}
