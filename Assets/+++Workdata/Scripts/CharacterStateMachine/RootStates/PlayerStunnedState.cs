using System.Collections;
using UnityEngine;

public class PlayerStunnedState : PlayerBaseState
{
    private Coroutine hitStunCoroutine;
    
    public PlayerStunnedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Rb.linearVelocity = Vector3.zero; //Reset movement
        Ctx.Anim.Play("HitReactionHeavy");
        //Apply HitStop Before Knockback
        Ctx.StartCoroutine(HitStop());
    }

    public override void UpdateState()
    {
        Debug.Log("Stunned");
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.InHitStun)
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
        PlayerStateMachine opponentScript = Ctx.Opponent.GetComponent<PlayerStateMachine>();
        Time.timeScale = 0f; //freeze game time
        if (Ctx.IsComboPossible)
        {
            opponentScript.IsAttacking = false;
            opponentScript.CanCombo = true;
        }
        Ctx.CmImpulse.GenerateImpulse();
        hitStunCoroutine = Ctx.StartCoroutine(HitStunCoroutine());
        yield return new WaitForSecondsRealtime(Ctx.HitStopDuration); // wait for real-world time
        if (Ctx.IsComboPossible)
        {
            opponentScript.CanCombo = false;
            opponentScript.IsAttacking = true; 
        }
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
}
