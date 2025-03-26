using System;
using UnityEngine;

[Serializable]
public class MovesSawFighter
{
    public string name;
    [Min(0)] public float damage;
    [Min(0)] public float stunDuration;
    [Min(0)] public float hitStopDuration;
    [Min(0)] public Vector2 attackForce;
    [Min(0)] public float knockBackTime;
    public bool hasFixedKnockBack;
    public Hitbox hitbox;
    public FrameChecker frameChecker;
}
