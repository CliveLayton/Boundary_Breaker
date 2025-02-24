using System;
using UnityEngine;

public class SawFighterAnimationEvents : MonoBehaviour
{
    #region Variables

    private SawFighter sawFighter;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        sawFighter = GetComponentInParent<SawFighter>();
    }

    #endregion

    #region SawFighter Animation Events Methods

    public void EndAttack()
    {
        sawFighter.inAttack = false;
    }
    
    #endregion
}
