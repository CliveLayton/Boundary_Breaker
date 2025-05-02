using UnityEngine;

public class PlayerGrabbedState : PlayerBaseState
{
    public PlayerGrabbedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Anim.Play("Grabbed");
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
        if (Ctx.InHitStun)
        {
            Ctx.InGrab = false;
            SwitchState(Factory.HitStun());
        }
    }

    public override void InitializeSubState()
    {
        
    }
}
