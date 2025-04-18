using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState, IHitboxResponder, IFrameCheckHandler
{
    private CharacterMoves currentMove;
    private PlayerStateMachine opponentScript;

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        opponentScript = Ctx.Opponent.GetComponent<PlayerStateMachine>();
    }

    public override void EnterState()
    {
        //check if the current move is already in his animation so its already active in the moment
        if (currentMove == Ctx.Moves[(int)Ctx.CurrentMove] && Ctx.Moves[(int)Ctx.CurrentMove].frameChecker.animationFrameInfo.isActive())
        {
            return;
        }
        
        if (currentMove != null)
        {
            OnHitFrameEnd();
        }

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
        
        if (Ctx.CanCombo && Ctx.IsAttacking)
        {
            SwitchState(Factory.Attack());
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
        opponentScript.InBlock = false;
        Ctx.IsAttacking = false;
        CheckSwitchStates();
    }

    public void OnLastFrameEnd()
    {
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
            currentMove.attackForce, currentMove.knockBackTime, currentMove.hasFixedKnockBack, 
            currentMove.isComboPossible, currentMove.getKnockBackToOpponent, true);
    }

    #endregion

    
}
