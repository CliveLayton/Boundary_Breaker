using UnityEngine;

public interface IDamageable
{
    public void Damage(float damageAmount, float stunDuration, float hitStopDuration, Vector2 attackForce, 
        float knockBackTime, bool hasFixedKnockBack, bool isComboPossible, bool getKnockBackToOpponent, bool isPlayerAttack);
}
