using System;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject modelPivot;

    [Header("Rotations for Animations")] 
    [SerializeField] private Vector3 rotationHalfToOpponent;
    [SerializeField] private Vector3 rotationToCamera;
    [SerializeField] private int xScaling;

    private PlayerStateMachine player;

    #endregion

    #region Unity Methods

    private void Start()
    {
        player = GetComponent<PlayerStateMachine>();
    }

    #endregion

    #region PlayerAnimation Methods

    /// <summary>
    /// Sets the Hurtboxes of the player on or off
    /// </summary>
    /// <param name="indicator">0 = off, 1 = on</param>
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

    /// <summary>
    /// Reset rotation of the parent object of the model and mirror it by scaling it negative if needed,
    /// also adjusts the hurtboxes and hitboxes to work with negative scale
    /// This is only a quick solution for a problem we want to solve later with animations
    /// </summary>
    public void ResetRotation()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(0,0,0);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }

    /// <summary>
    /// rotates the parent object of the model half to the camera and mirror it by scaling negative if needed,
    /// also adjusts the hurtboxes and hitboxes to work with negative scale
    /// This is only a quick solution for a problem we want to solve later with animations
    /// </summary>
    public void RotateHalfToOpponent()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(rotationHalfToOpponent);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }

    /// <summary>
    /// rotates the parent object of the model to the camera and mirror it by scaling negative if needed,
    /// also adjusts the hurtboxes and hitboxes to work with negative scale
    /// This is only a quick solution for a problem we want to solve later with animations
    /// </summary>
    public void RotateToCamera()
    {
        modelPivot.transform.localRotation = Quaternion.Euler(rotationToCamera);
        player.AdjustCollisionBoxes(player.IsFacingRight() ? 1 : xScaling);
        player.transform.localScale = new Vector3(player.IsFacingRight() ? 1 : xScaling, 1, 1);
    }

    #endregion
}
