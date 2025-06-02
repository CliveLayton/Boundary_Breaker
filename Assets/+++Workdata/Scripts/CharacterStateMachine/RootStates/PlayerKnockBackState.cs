using System.Collections;
using UnityEngine;

public class PlayerKnockBackState : PlayerBaseState
{
    private Coroutine knockBackCoroutine;
    private bool applyKnockdown;
    
    public PlayerKnockBackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Rb.linearVelocity = Vector3.zero;
        Ctx.IsBeingKnockedBack = true;
        if (!Ctx.GetFixedKnockBack)
        {
            Ctx.Anim.Play("Knockback"); 
        }

        Vector3 directionToOpponent = (Ctx.Opponent.transform.position - Ctx.transform.position).normalized;
        if (Ctx.GetKnockBackToOpponent)
        {
            knockBackCoroutine = Ctx.StartCoroutine(KnockbackAction(directionToOpponent, Vector2.up));
        }
        else
        {
            knockBackCoroutine = Ctx.StartCoroutine(KnockbackAction(-directionToOpponent, Vector2.up));
        }
    }

    public override void UpdateState()
    {
        if (!Ctx.InKnockdown)
        {
            if (!Ctx.IsGrounded() && Ctx.Rb.linearVelocity.y <= -1f)
            {
                applyKnockdown = true;
            }
            else if (!Ctx.IsGrounded() && Ctx.Rb.linearVelocity.y > -1f)
            {
                applyKnockdown = false;
            }
        }
        
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.IsAttacking = false;
        Ctx.IsBeingKnockedBack = false;
        Ctx.StopCoroutine(knockBackCoroutine);
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded() && !Ctx.InHitStun && !Ctx.IsBeingKnockedBack && !Ctx.InKnockdown)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded() && !Ctx.InHitStun && !Ctx.IsBeingKnockedBack)
        {
            //set InKnockdown false if the knockback was strong enough to declare knockdown but stopped in air
            Ctx.InKnockdown = false;
            Ctx.LastMovementX = Ctx.Rb.linearVelocity.x;
            SwitchState(Factory.InAir());
        }
        else if(Ctx.InHitStun || (Ctx.InKnockdown && Ctx.IsGrounded()))
        {
            SwitchState(Factory.Stunned());
        }
    }

    public override void InitializeSubState()
    {
        
    }
    
    private IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection)
    {
        Vector2 hitForce;
        Vector2 constantKnockBackForce;
        Vector2 knockBackForce;

        float elapsedTime = 0f;
        while (elapsedTime < Ctx.KnockBackTime)
        {
            //iterate the timer
            elapsedTime += Time.fixedDeltaTime;

            //if the player did DI in Hitstop calculate inputforce to knockback otherwise not
            if (Ctx.DidDI)
            {
                //update hitForce (x force)
                hitForce = hitDirection * ((Ctx.AttackForce.x - Ctx.InputForce.x) *
                                           Ctx.KnockBackForceCurve.Evaluate(elapsedTime / Ctx.KnockBackTime));
            
                //update y force
                constantKnockBackForce = constantForceDirection * ((Ctx.AttackForce.y + Ctx.InputForce.y) *
                                                                   Ctx.KnockBackForceCurve.Evaluate(elapsedTime / Ctx.KnockBackTime));
            }
            else
            {
                //update hitForce (x force)
                hitForce = hitDirection * (Ctx.AttackForce.x * Ctx.KnockBackForceCurve.Evaluate(elapsedTime / Ctx.KnockBackTime));
            
                //update y force
                constantKnockBackForce = constantForceDirection * (Ctx.AttackForce.y * Ctx.KnockBackForceCurve.Evaluate(elapsedTime / Ctx.KnockBackTime));
            }

            //combine hitForce and constantForce
            knockBackForce = hitForce + constantKnockBackForce;
            
            //combine knockBackForce with Input Force
            if (Ctx.MoveInput.x != 0 && !Ctx.GetFixedKnockBack)
            {
                Ctx.CombinedForce = new Vector2(knockBackForce.x * (1 + Ctx.PercentageCount / 100),
                    knockBackForce.y * (1 + Ctx.PercentageCount / 100));
            }
            else if(!Ctx.GetFixedKnockBack)
            {
                Ctx.CombinedForce = new Vector2(knockBackForce.x * (1 + Ctx.PercentageCount/100),
                    knockBackForce.y * (1 + Ctx.PercentageCount/100));
            }
            else if (Ctx.MoveInput.x != 0 && Ctx.GetFixedKnockBack)
            {
                Ctx.CombinedForce = knockBackForce;
            }
            else
            {
                Ctx.CombinedForce = knockBackForce;
            }
            
            //apply knockBack
            Ctx.Rb.linearVelocity = Ctx.CombinedForce;

            yield return new WaitForFixedUpdate();
        }

        //if the player is not already declared as InKnockdown through the move, change value 
        if (!Ctx.InKnockdown)
        {
            Ctx.InKnockdown = applyKnockdown;
        }
        Ctx.IsBeingKnockedBack = false;
    }
}
