using System.Collections;
using UnityEngine;

public class PlayerHitStunState : PlayerBaseState
{
    private Coroutine hitStopCoroutine;
    private Coroutine hitStunCoroutine;
    private bool hasJumpCanceled = false;
    private bool inHitStop;

    public PlayerHitStunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.DidDI = false;
        inHitStop = false;
        Ctx.InputForce = Ctx.DefaultInputForce;
        Ctx.Anim.Play("HitReactionHeavy");
        //Apply HitStop Before Knockback
        hitStopCoroutine = Ctx.StartCoroutine(HitStop());
    }

    public override void UpdateState()
    {
        if (inHitStop && Ctx.IsFacingRight() && Ctx.MoveInput.x > 0)
        {
            Ctx.DidDI = true;
            Ctx.InputForce = new Vector2(Ctx.DefaultInputForce.x * Ctx.MoveInput.x,
                Ctx.DefaultInputForce.y * Ctx.MoveInput.x);
        }
        else if (inHitStop && !Ctx.IsFacingRight() && Ctx.MoveInput.x < 0)
        {
            Ctx.DidDI = true;
            Ctx.InputForce = new Vector2(Ctx.DefaultInputForce.x * Mathf.Abs(Ctx.MoveInput.x),
                Ctx.DefaultInputForce.x * Mathf.Abs(Ctx.MoveInput.x));
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        //Debug.Log("Check States");
        //Debug.Log(Ctx.InHitStun + " ; " + Ctx.InGrab + " ; " + Ctx.InComboHit);
        if (!Ctx.InHitStun && !Ctx.InGrab)
        {
            SwitchState(Factory.KnockBack());
        }
        else if (Ctx.InHitStun && Ctx.InComboHit)
        {
            Ctx.StopCoroutine(hitStunCoroutine);
            Ctx.InComboHit = false;
            SwitchState(Factory.Stunned());
        }
    }

    public override void InitializeSubState()
    {
        
    }
    
    private IEnumerator HitStop()
    {
        inHitStop = true;
        Time.timeScale = 0f; //freeze game time
        if (Ctx.IsComboPossible)
        {
            Ctx.Opponent.HandleCombo(true, false);
            Ctx.StartCoroutine(CheckForJumpCancel());
        }
        Ctx.CmImpulse.GenerateImpulse();
        hitStunCoroutine = Ctx.StartCoroutine(HitStunCoroutine());
        yield return new WaitForSecondsRealtime(Ctx.HitStopDuration); // wait for real-world time
        if (Ctx.IsComboPossible)
        {
            Ctx.Opponent.HandleCombo(false, hasJumpCanceled);
        }
        inHitStop = false;
        Time.timeScale = 1f; //resume game time
    }
    
    private IEnumerator HitStunCoroutine()
    {
        float timer = 0f;
        
        Vector3 originalPosition = Ctx.transform.localPosition;

        while (timer < Ctx.HitStunDuration)
        {
            //shake effect
            float shakeAmount = 0.05f;
            Ctx.transform.localPosition = originalPosition + new Vector3(Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount), 0);
            
            timer += Time.deltaTime;
            yield return null;
        }

        Ctx.transform.localPosition = originalPosition; // reset position
        Ctx.InHitStun = false;
    }

    private IEnumerator CheckForJumpCancel()
    {
        float timer = 0f;
        hasJumpCanceled = false;

        while (timer < Ctx.HitStopDuration)
        {
            hasJumpCanceled = Ctx.Opponent.IsJumpedPressed;

            if (hasJumpCanceled)
            {
                Time.timeScale = 1f;
                if (hitStopCoroutine != null)
                {
                    Ctx.StopCoroutine(hitStopCoroutine); 
                }
                
                if (hitStunCoroutine != null)
                {
                    Ctx.StopCoroutine(hitStunCoroutine); 
                }
                Ctx.Opponent.HandleCombo(false,true);
                Ctx.InHitStun = false;
                break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
