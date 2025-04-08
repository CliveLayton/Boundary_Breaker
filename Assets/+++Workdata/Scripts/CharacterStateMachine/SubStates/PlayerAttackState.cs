using UnityEngine;

public class PlayerAttackState : PlayerBaseState, IHitboxResponder, IFrameCheckHandler
{
    private CharacterMoves currentMove;
    
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        if (currentMove != null)
        {
            OnHitFrameEnd();
        }
        
        Debug.Log("Enter Attack");
        
        Ctx.Anim.Play(Ctx.CurrentMove.ToString());
        currentMove = Ctx.Moves[(int)Ctx.CurrentMove];
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].SetResponder(this);
        }
        currentMove.frameChecker.Initialize(this);
        
        Ctx.Rb.linearVelocity = new Vector2(0, Ctx.Rb.linearVelocity.y);
        Ctx.LastMovementX = 0;
    }

    public override void UpdateState()
    {
        currentMove.frameChecker.CheckFrames();
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].HitBoxUpdate();
        }
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsDashing && !Ctx.IsAttacking)
        {
            SwitchState(Factory.Dash());
        }
    }

    public override void InitializeSubState()
    {
        
    }

    #region FrameCheck Methods

    public void OnHitFrameStart()
    {
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].StartCheckingCollision();
        }
    }

    public void OnHitFrameEnd()
    {
        for (int i = 0; i < currentMove.hitbox.Length; i++)
        {
            currentMove.hitbox[i].StopCheckingCollision();
        }
    }

    public void OnLastFrameStart()
    {
        Debug.Log("Last Frame Exit");
        Ctx.IsAttacking = false;
        CheckSwitchStates();
    }

    public void OnLastFrameEnd()
    {
        Debug.Log("After Animation Exit");
        Ctx.IsAttacking = false;
        CheckSwitchStates();
    }

    #endregion

    #region Hitbox Methods

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

    #endregion

    
}
