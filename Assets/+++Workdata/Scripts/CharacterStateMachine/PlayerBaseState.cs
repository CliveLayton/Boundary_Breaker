using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine Ctx { get; private set; }
    protected PlayerStateFactory Factory { get; private set; }
    private PlayerBaseState currentSubState;
    private PlayerBaseState currentSuperState;
    protected bool IsRootState { get; set; }

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        Ctx = currentContext;
        Factory = playerStateFactory;
    }
    
    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();
        
        //new state enters state
        newState.EnterState();
        
        if (IsRootState)
        {
            //switch current state of context
            Ctx.CurrentState = newState;

            //if the new state has a substate initialized enter the substate too
            if (newState.currentSubState != null)
            {
                newState.currentSubState.EnterState();
            }
        }
        else if (currentSuperState != null)
        {
            //if this state is not a root state but want to switch to a root state
            //switch current state of context
            if (newState.IsRootState)
            {
                Ctx.CurrentState = newState;
                
                //if the new state has a substate initialized enter the substate too
                if (newState.currentSubState != null)
                {
                    newState.currentSubState.EnterState();
                }

                return;
            }
            
            //set the current super states sub state to the new state
            currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
