using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject modelPivot;

    [Header("Rotations for Animations")] 
    [SerializeField] private Vector3 rotationHalfToOpponent;
    [SerializeField] private Vector3 rotationToCamera;
    [SerializeField] private int xScaling;

    private PlayerStateMachine player;

    private void Start()
    {
        player = GetComponent<PlayerStateMachine>();
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

    public void ResetRotation()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(0,0,0);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }

    public void RotateHalfToOpponent()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(rotationHalfToOpponent);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }

    public void RotateToCamera()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(rotationToCamera);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }
}
