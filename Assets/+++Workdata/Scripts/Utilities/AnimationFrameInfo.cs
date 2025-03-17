using System;
using UnityEngine;

[Serializable]
public class AnimationFrameInfo
{
    public Animator animator;
    public AnimationClip clip;
    public string animatorStateName;
    public int layerNumber;

    private int totalFrames = 0;
    private int animationFullNameHash;

    public void Initialize()
    {
        totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);

        if (animator.isActiveAndEnabled)
        {
            string name = animator.GetLayerName(layerNumber) + "." + animatorStateName; //get the hash of the animation clip state 
            animationFullNameHash = Animator.StringToHash(name); //set the hash correctly
        }
    }

    public bool isActive()
    {
        return animator.IsPlayingOnLayer(animationFullNameHash, 0);
    }

    /// <summary>
    /// get true if the frame you ask for is equal or higher than the current frame in the animation
    /// </summary>
    /// <param name="frameNumber"></param>
    /// <returns></returns>
    public bool BiggerOrEqualThanFrame(int frameNumber)
    {
        double percentage = animator.NormalizedTime(layerNumber);
        return percentage >= PercentageOnFrame(frameNumber);
    }

    /// <summary>
    /// check if the animation is on the last frame
    /// </summary>
    /// <returns></returns>
    public bool ItsOnLastFrame()
    {
        double percentage = animator.NormalizedTime(layerNumber);
        return percentage > PercentageOnFrame(totalFrames - 1); //-1 to avoid transition problems because sometimes animations dont reach fully 100% of the animation
    }

    public int TotalFrames()
    {
        return totalFrames;
    }

    /// <summary>
    /// get the percentage of the frame we ask for in the animation clip
    /// </summary>
    /// <param name="frameNumber"></param>
    /// <returns></returns>
    private double PercentageOnFrame(int frameNumber)
    {
        return (double)frameNumber / (double)totalFrames;
    }
}
