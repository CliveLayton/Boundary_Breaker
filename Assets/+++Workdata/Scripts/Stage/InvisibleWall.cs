using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float hitStopDuration = 0.3f;
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private Vector2 attackForce = new Vector2(1.5f,-2f);
    [SerializeField] private bool applyKnockDown;
    [SerializeField] private PlayerStateMachine Player1;
    [SerializeField] private PlayerStateMachine Player2;
    
    private BoxCollider col;
    private bool isOnWall;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        UIManager.Instance.onTimerExpired += RestartMatch;
    }

    private void OnDisable()
    {
        UIManager.Instance.onTimerExpired -= RestartMatch;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //PlayerController fighter = other.gameObject.GetComponent<PlayerController>();
            PlayerStateMachine fighter = other.gameObject.GetComponent<PlayerStateMachine>();
            IDamageable iDamageable = other.gameObject.GetComponent<IDamageable>();

            if (fighter.CombinedForce.magnitude > 6f && !isOnWall) //fighter.combinedForce.magnitude
            {
                col.enabled = false;
                isOnWall = true;
                iDamageable?.Damage(damage, 0.5f, 0.5f, 
                    new Vector2(3,0.8f), 0.4f, false, false, false, false, false);
                StartCoroutine(RestartGame());
            }
            else if(!isOnWall)
            {
                isOnWall = true;
                iDamageable?.Damage(damage, stunDuration, hitStopDuration, 
                    attackForce, knockBackTime, false, false, true, false, applyKnockDown);
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

    private void RestartMatch(bool isExpired)
    {
        if (isExpired)
        {
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        Time.timeScale = 0.5f;
        UIManager.Instance.countdownActive = false;
        yield return new WaitForSeconds(1.5f);
        Player1.ResetPercentage();
        Player2.ResetPercentage();
        LoadSceneManager.instance.SwitchScene(GameStateManager.fightingScene1, false);
    }
}
