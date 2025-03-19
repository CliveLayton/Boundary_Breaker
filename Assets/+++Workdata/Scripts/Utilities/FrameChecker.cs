using System;

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

    public void Initialize(IFrameCheckHandler frameCheck, AnimationFrameInfo animationInfo)
    {
        animationInfo.Initialize();
        frameCheckHandler = frameCheck;
        animationFrameInfo = animationInfo;
        totalFrames = animationInfo.TotalFrames();
        InitCheck();
    }

    public void InitCheck()
    {
        checkedHitFrameStart = false;
        checkedHitFrameEnd = false;
        lastFrame = false;
    }

    public void CheckFrames()
    {
        if (lastFrame)
        {
            lastFrame = false;
            frameCheckHandler.OnLastFrameEnd();
        }

        if (!animationFrameInfo.isActive())
        {
            return;
        }

        if (!checkedHitFrameStart && animationFrameInfo.BiggerOrEqualThanFrame(hitFrameStart))
        {
            frameCheckHandler.OnHitFrameStart();
            checkedHitFrameStart = true;
        }
        else if (!checkedHitFrameEnd && animationFrameInfo.BiggerOrEqualThanFrame(hitFrameEnd))
        {
            frameCheckHandler.OnHitFrameEnd();
            checkedHitFrameEnd = true;
        }

        if (!lastFrame && animationFrameInfo.ItsOnLastFrame())
        {
            frameCheckHandler.OnLastFrameStart();
            lastFrame = true; // This is here so we don't skip the last frame
        }
    }
    
    public void GetTotalFrames(AnimationFrameInfo animationInfo)
    {
        if (animationInfo.clip != null)
        {
            animationInfo.Initialize();
            totalFrames = animationInfo.TotalFrames();
        }
    }
}
