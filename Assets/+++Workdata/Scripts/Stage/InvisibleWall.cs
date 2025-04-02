using System;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    private BoxCollider col;
    private bool isOnWall;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController fighter = other.gameObject.GetComponent<PlayerController>();
            IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();

            if (fighter.combinedForce.magnitude > 20f && !isOnWall)
            {
                col.enabled = false;
                isOnWall = true;
                iDamageable?.Damage(0, 0.5f, 0.5f, 
                    new Vector2(3,0.8f), 0.4f, false, false, false);
            }
            else if(!isOnWall)
            {
                isOnWall = true;
                iDamageable?.Damage(0, 0.5f, 0.3f, 
                    new Vector2(1.5f,-2f), 0.2f, false, false, true);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isOnWall = false;
        }
    }
}
