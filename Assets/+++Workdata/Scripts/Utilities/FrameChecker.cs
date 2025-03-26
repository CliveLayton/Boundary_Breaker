using System;
using UnityEngine;

[Serializable]
public class FrameChecker
{
    public int hitFrameStart;
    public int hitFrameEnd;
    public int totalFrames;
    public AnimationFrameInfo animationFrameInfo;

    private IFrameCheckHandler frameCheckHandler;
    private bool checkedHitFrameStart;
    private bool checkedHitFrameEnd;
    private bool lastFrame;
    private AnimationState animationState = AnimationState.Startup;

    enum AnimationState
    {
        Startup,
        ActiveHit,
        Recovery
    }

    /// <summary>
    /// initialize the animationinfo to get all data for the check
    /// </summary>
    /// <param name="frameCheck"></param>
    /// <param name="animationInfo"></param>
    public void Initialize(IFrameCheckHandler frameCheck)
    {
        animationFrameInfo.Initialize();
        frameCheckHandler = frameCheck;
        totalFrames = animationFrameInfo.TotalFrames();
        InitCheck();
    }

    /// <summary>
    /// reset all bools for the check
    /// </summary>
    public void InitCheck()
    {
        checkedHitFrameStart = false;
        checkedHitFrameEnd = false;
        lastFrame = false;
    }

    /// <summary>
    /// check the current state of the animation to get the hit frame window
    /// </summary>
    public void CheckFrames()
    {

        if (!animationFrameInfo.isActive())
        {
            return;
        }
        
        switch (animationState)
        {
            case AnimationState.Startup:
                if (animationFrameInfo.BiggerOrEqualThanFrame(hitFrameStart))
                {
                    frameCheckHandler.OnHitFrameStart();
                    animationState = AnimationState.ActiveHit;
                }
                break;
            case AnimationState.ActiveHit:
                if (animationFrameInfo.BiggerOrEqualThanFrame(hitFrameEnd))
                {
                    frameCheckHandler.OnHitFrameEnd();
                    animationState = AnimationState.Recovery;
                }
                break;
            case AnimationState.Recovery:
                if (animationFrameInfo.ItsOnLastFrame())
                {
                    frameCheckHandler.OnLastFrameStart();
                    animationState = AnimationState.Startup;
                }
                break;
        }
        
        // if (lastFrame)
        // {
        //     lastFrame = false;
        //     frameCheckHandler.OnLastFrameEnd();
        // }
        //
        // if (!animationFrameInfo.isActive())
        // {
        //     return;
        // }
        //
        // if (!checkedHitFrameStart && animationFrameInfo.BiggerOrEqualThanFrame(hitFrameStart))
        // {
        //     frameCheckHandler.OnHitFrameStart();
        //     checkedHitFrameStart = true;
        // }
        // else if (!checkedHitFrameEnd && animationFrameInfo.BiggerOrEqualThanFrame(hitFrameEnd))
        // {
        //     frameCheckHandler.OnHitFrameEnd();
        //     checkedHitFrameEnd = true;
        // }
        //
        // if (!lastFrame && animationFrameInfo.ItsOnLastFrame())
        // {
        //     frameCheckHandler.OnLastFrameStart();
        //     lastFrame = true; // This is here so we don't skip the last frame
        // }
    }
    
    /// <summary>
    /// get the total frames of the animation
    /// </summary>
    /// <param name="animationInfo"></param>
    public void GetTotalFrames()
    {
        if (animationFrameInfo.clip != null)
        {
            animationFrameInfo.Initialize();
            totalFrames = animationFrameInfo.TotalFrames();
        }
    }
}
