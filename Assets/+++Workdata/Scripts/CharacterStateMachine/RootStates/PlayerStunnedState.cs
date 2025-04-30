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
        Ctx.Rb.linearVelocity = Vector3.zero; //Reset movement
        InitializeSubState();
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
        if (!Ctx.InHitStun)
        {
            SwitchState(Factory.KnockBack());
        }
        else if (Ctx.InHitStun && Ctx.InComboHit)
        {
            Ctx.StopCoroutine(Ctx.HitStunCoroutine);
            Ctx.InComboHit = false;
            SwitchState(Factory.Stunned());
        }
    }

    public override void InitializeSubState()
    {
        SetSubState(Factory.HitStun());
    }
}
