using System.Collections;
using UnityEngine;

public class PlayerHitStunState : PlayerBaseState
{
    public PlayerHitStunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Anim.Play("HitReactionHeavy");
        //Apply HitStop Before Knockback
        Ctx.StartCoroutine(HitStop());
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }
    
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f; //freeze game time
        if (Ctx.IsComboPossible)
        {
            Ctx.Opponent.HandleCombo(true);
        }
        Ctx.CmImpulse.GenerateImpulse();
        Ctx.HitStunCoroutine = Ctx.StartCoroutine(HitStunCoroutine());
        yield return new WaitForSecondsRealtime(Ctx.HitStopDuration); // wait for real-world time
        if (Ctx.IsComboPossible)
        {
            Ctx.Opponent.HandleCombo(false);
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
