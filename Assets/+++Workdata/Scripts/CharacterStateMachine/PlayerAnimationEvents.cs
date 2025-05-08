using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerStateMachine player;

    private void Start()
    {
        player = GetComponentInParent<PlayerStateMachine>();
    }

    public void SetPlayerHurtbox(int indicator)
    {
        if (indicator == 0)
        {
            Debug.Log("SetOff");
            player.HandleHurtboxes(false); 
        }
        else
        {
            Debug.Log("SetOn");
            player.HandleHurtboxes(true);
        }
    }
}
