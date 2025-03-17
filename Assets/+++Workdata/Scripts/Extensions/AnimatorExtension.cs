using UnityEngine;

public static class AnimatorExtension
{
    /// <summary>
    /// check if the current state is the state we want to check
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="fullPathHash"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool IsPlayingOnLayer(this Animator animator, int fullPathHash, int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;
    }

    /// <summary>
    /// get the time in the animation. NOTE: you get the percentage back so a number between 0-1 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static double NormalizedTime(this Animator animator, System.Int32 layer)
    {
        double time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        return time > 1 ? 1 : time;
    }
}
