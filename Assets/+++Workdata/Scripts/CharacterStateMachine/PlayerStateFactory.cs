using System.Collections.Generic;

enum PlayerStates
{
    Idle,
    Walk,
    Dash,
    Grounded,
    InAir,
    Jump,
    Attack,
    Stunned,
    KnockBack,
    Block,
    HitStun,
    Grabbed,
    Knockdown
}

public class PlayerStateFactory
{
    private PlayerStateMachine context;
    private Dictionary<PlayerStates, PlayerBaseState> states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
        states[PlayerStates.Idle] = new PlayerIdleState(context, this);
        states[PlayerStates.Walk] = new PlayerWalkState(context, this);
        states[PlayerStates.Dash] = new PlayerDashState(context, this);
        states[PlayerStates.Grounded] = new PlayerGroundedState(context, this);
        states[PlayerStates.InAir] = new PlayerInAirState(context, this);
        states[PlayerStates.Jump] = new PlayerJumpState(context, this);
        states[PlayerStates.Attack] = new PlayerAttackState(context, this);
        states[PlayerStates.Stunned] = new PlayerStunnedState(context, this);
        states[PlayerStates.KnockBack] = new PlayerKnockBackState(context, this);
        states[PlayerStates.Block] = new PlayerBlockState(context, this);
        states[PlayerStates.HitStun] = new PlayerHitStunState(context, this);
        states[PlayerStates.Grabbed] = new PlayerGrabbedState(context, this);
        states[PlayerStates.Knockdown] = new PlayerKnockdownState(context, this);
    }

    public PlayerBaseState Idle()
    {
        return states[PlayerStates.Idle];
    }

    public PlayerBaseState Walk()
    {
        return states[PlayerStates.Walk];
    }

    public PlayerBaseState Dash()
    {
        return states[PlayerStates.Dash];
    }

    public PlayerBaseState InAir()
    {
        return states[PlayerStates.InAir];
    }

    public PlayerBaseState Jump()
    {
        return states[PlayerStates.Jump];
    }

    public PlayerBaseState Grounded()
    {
        return states[PlayerStates.Grounded];
    }

    public PlayerBaseState Attack()
    {
        return states[PlayerStates.Attack];
    }

    public PlayerBaseState Stunned()
    {
        return states[PlayerStates.Stunned];
    }

    public PlayerBaseState KnockBack()
    {
        return states[PlayerStates.KnockBack];
    }

    public PlayerBaseState Block()
    {
        return states[PlayerStates.Block];
    }

    public PlayerBaseState HitStun()
    {
        return states[PlayerStates.HitStun];
    }

    public PlayerBaseState Grabbed()
    {
        return states[PlayerStates.Grabbed];
    }

    public PlayerBaseState Knockdown()
    {
        return states[PlayerStates.Knockdown];
    }
}
