using UnityEngine;

public class AttackSawFighter : MonoBehaviour, IHitboxResponder, IFrameCheckHandler
{
    public MovesSawFighter[] moves;

    private MovesSawFighter currentAttack;

    private SawFighter sawFighter;

    private bool attackIsActive;

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

        if (attackIsActive)
        {
            currentAttack.frameChecker.CheckFrames();
            currentAttack.hitbox.HitBoxUpdate();
        }
    }

    public void CollisionedWith(Collider collider)
    {
        IDamageable iDamageable = collider.gameObject.GetComponentInParent<IDamageable>();
        iDamageable?.Damage(currentAttack.damage, currentAttack.stunDuration, currentAttack.hitStopDuration, 
            currentAttack.attackForce, currentAttack.knockBackTime, currentAttack.hasFixedKnockBack);
    }

    public void SetMove(int moveNumber)
    {
        currentAttack = moves[moveNumber];
        currentAttack.hitbox.SetResponder(this);
        currentAttack.frameChecker.Initialize(this);
        attackIsActive = true;
    }

    public void OnHitFrameStart()
    {
        Debug.Log("start hitbox");
        currentAttack.hitbox.StartCheckingCollision();
    }

    public void OnHitFrameEnd()
    {
        Debug.Log("end hitbox");
        currentAttack.hitbox.StopCheckingCollision();
    }

    public void OnLastFrameStart()
    {
        Debug.Log("start last frame");
        attackIsActive = false;
        sawFighter.inAttack = false;
    }

    public void OnLastFrameEnd()
    {
        Debug.Log("end last frame");
       
    }

    /// <summary>
    /// get the frames of each animation assigned for the frame checkers
    /// </summary>
    [ContextMenu("Get Frames From Animations")]
    private void GetAnimationFrames()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            moves[i].frameChecker.GetTotalFrames();
        }
    }
}
