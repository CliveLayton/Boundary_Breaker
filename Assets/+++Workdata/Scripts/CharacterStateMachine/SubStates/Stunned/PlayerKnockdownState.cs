using UnityEngine;

public class PlayerKnockdownState : PlayerBaseState
{
    private bool gettingUp;
    
    public PlayerKnockdownState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.HandleHurtboxes(false);
        Ctx.Anim.Play("Knockdown");
    }

    public override void UpdateState()
    {
        switch (gettingUp)
        {
            case false :
                if (Ctx.Anim.NormalizedTime(0) > 0.9f)
                {
                    Ctx.Anim.Play("GetUp");
                    gettingUp = true;
                }
                break;
            case true :
                if (Ctx.Anim.NormalizedTime(0) > 0.9f)
                {
                    gettingUp = false;
                    CheckSwitchStates();
                }
                break;
        }
    }

    public override void ExitState()
    {
        Ctx.HandleHurtboxes(true);
        Ctx.InKnockdown = false;
    }

    public override void CheckSwitchStates()
    {
        SwitchState(Factory.Grounded());
    }

    public override void InitializeSubState()
    {
        
    }
}
