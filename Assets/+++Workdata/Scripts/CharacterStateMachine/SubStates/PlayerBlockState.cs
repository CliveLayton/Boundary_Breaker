using System.Collections;
using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Shield.transform.localRotation = Ctx.IsFacingRight() ? Quaternion.Euler(0, 30, 0) : Quaternion.Euler(0, -30, 0);
        Ctx.Anim.Play("Blocking");
        Ctx.ShieldTimer = 0;
        Ctx.ShieldDurability -= Mathf.Abs(Ctx.AttackForce.x) + Mathf.Abs(Ctx.AttackForce.y);
        if (Ctx.ShieldDurability <= 0)
        {
            Ctx.ShieldMaterial.EnableKeyword("_USE_DESOLVE");
            Ctx.ShieldMaterial.SetFloat("_Desolve_Value", Ctx.MaxShieldDurability);
            Ctx.StartCoroutine(LerpDesolve());
            Ctx.InHitStun = true;
        }
        Ctx.ShieldMaterial.SetFloat("_Desolve_Value", Ctx.ShieldDurability);
        Ctx.Shield.SetActive(true);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.ShieldDurability > 0)
        {
            Ctx.Shield.SetActive(false);
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.MoveInput.x == 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.MoveInput.x != 0 && !Ctx.IsDashing && !Ctx.InBlock)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.InHitStun)
        {
            SwitchState(Factory.Stunned());
        }
    }

    public override void InitializeSubState()
    {
        
    }

    private IEnumerator LerpDesolve()
    {
        float elapsed = 0f;
        float desolveValue = Ctx.MaxShieldDurability;

        while (elapsed < Ctx.HitStopDuration + Ctx.HitStunDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (Ctx.HitStopDuration + Ctx.HitStunDuration);
            desolveValue = Mathf.Lerp(Ctx.MaxShieldDurability, 0, t);
            Ctx.ShieldMaterial.SetFloat("_Desolve_Value", desolveValue);

            yield return null;
        }

        Ctx.Shield.SetActive(false);
        Ctx.ShieldMaterial.DisableKeyword("_USE_DESOLVE");
    }
}
