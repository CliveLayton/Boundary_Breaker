using System;
using UnityEngine;

public class AttackSawFighter : MonoBehaviour, IHitboxResponder, IFrameCheckHandler
{
    public MovesSawFighter[] moves;

    private MovesSawFighter currentAttack;

    private SawFighter sawFighter;

    private void Awake()
    {
        sawFighter = GetComponentInParent<SawFighter>();
    }

    private void Update()
    {
        if (currentAttack == null)
        {
            return;
        }
        
        if (sawFighter.inAttack)
        {
            currentAttack.hitbox.StartCheckingCollision();
            currentAttack.hitbox.HitBoxUpdate();
        }
        else
        {
            currentAttack.hitbox.StopCheckingCollision();
            currentAttack = null;
        }
    }

    public void CollisionedWith(Collider collider)
    {
        IDamageable iDamageable = collider.gameObject.GetComponentInParent<IDamageable>();
        iDamageable?.Damage(currentAttack.damage, currentAttack.stunDuration, currentAttack.hitStopDuration);
    }

    public void SetMove(int moveNumber)
    {
        currentAttack = moves[moveNumber];
        currentAttack.hitbox.SetResponder(this);
    }

    public void OnHitFrameStart()
    {
        
    }

    public void OnHitFrameEnd()
    {
        
    }

    public void OnLastFrameStart()
    {
        
    }

    public void OnLastFrameEnd()
    {
        
    }

    /// <summary>
    /// get the frames of each animation assigned for the frame checkers
    /// </summary>
    [ContextMenu("Get Frames From Animations")]
    private void GetAnimationFrames()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            moves[i].frameChecker.GetTotalFrames(moves[i].frameChecker.animationFrameInfo);
        }
    }
}
