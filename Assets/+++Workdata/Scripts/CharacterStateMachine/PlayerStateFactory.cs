using System.Collections.Generic;

enum PlayerStates
{
    Idle,
    Walk,
    Dash,
    Grounded,
    InAir,
    Attack,
    Stunned,
    KnockBack
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
        states[PlayerStates.Attack] = new PlayerAttackState(context, this);
        states[PlayerStates.Stunned] = new PlayerStunnedState(context, this);
        states[PlayerStates.KnockBack] = new PlayerKnockBackState(context, this);
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
}
