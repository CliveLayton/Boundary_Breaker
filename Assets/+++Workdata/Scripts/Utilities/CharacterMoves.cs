using System;
using UnityEngine;

[Serializable]
public class CharacterMoves
{
    public string name;
    [Min(0)] public float damage;
    [Min(0)] public float stunDuration;
    [Min(0)] public float hitStopDuration;
    public Vector2 attackForce;
    [Min(0)] public float knockBackTime;
    public bool hasFixedKnockBack;
    public bool isComboPossible;
    public bool applyKnockDown;
    public bool getKnockBackToOpponent;
    public Hitbox[] hitbox;
    public FrameChecker frameChecker;
}
