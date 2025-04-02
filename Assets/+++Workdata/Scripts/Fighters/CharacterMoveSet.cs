using UnityEngine;

public class CharacterMoveSet : MonoBehaviour, IHitboxResponder, IFrameCheckHandler
{
    public CharacterMoves[] moves;

    private CharacterMoves currentMove;

    private PlayerController playerController;

    private bool attackIsActive;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (currentMove == null)
        {
            return;
        }

        if (attackIsActive)
        {
            currentMove.frameChecker.CheckFrames();
            for (int i = 0; i < currentMove.hitbox.Length; i++)
            {
                currentMove.hitbox[i].HitBoxUpdate();
            }
        }
    }

    public void CollisionedWith(Collider collider)
    {
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].StopCheckingCollision();
        }
        IDamageable iDamageable = collider.gameObject.GetComponentInParent<IDamageable>();
        iDamageable?.Damage(currentMove.damage, currentMove.stunDuration, currentMove.hitStopDuration, 
            currentMove.attackForce, currentMove.knockBackTime, currentMove.hasFixedKnockBack, currentMove.isComboPossible, currentMove.getKnockBackToOpponent);
    }

    public void SetMove(int moveNumber)
    {
        currentMove = moves[moveNumber];
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].SetResponder(this);
        }
        currentMove.frameChecker.Initialize(this);
        attackIsActive = true;
    }

    public void OnHitFrameStart()
    {
        Debug.Log("start hitbox");
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].StartCheckingCollision();
        }
    }

    public void OnHitFrameEnd()
    {
        if (currentMove == null)
        {
            return;
        }
        
        Debug.Log("end hitbox");
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].StopCheckingCollision();
        }
    }

    public void OnLastFrameStart()
    {
        Debug.Log("start last frame");
        attackIsActive = false;
        playerController.inAttack = false;
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
