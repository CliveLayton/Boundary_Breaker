using System.Collections;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float hitStopDuration = 0.3f;
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private Vector2 attackForce = new Vector2(1.5f,-2f);
    [SerializeField] private bool applyKnockDown;
    
    private PlayerStateMachine player1;
    private PlayerStateMachine player2;
    
    private BoxCollider col;
    private bool isOnWall;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        UIManager.Instance.onTimerExpired += RestartMatch;
        GameStateManager.Instance.onStateChanged += GetPlayers;
    }

    private void OnDisable()
    {
        UIManager.Instance.onTimerExpired -= RestartMatch;
        GameStateManager.Instance.onStateChanged -= GetPlayers;
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

    private void GetPlayers(GameStateManager.GameState newState)
    {
        if (newState == GameStateManager.GameState.InGame)
        {
            var characterArray = FindObjectsByType<PlayerStateMachine>(FindObjectsSortMode.None);
            for (int i = 0; i < characterArray.Length; i++)
            {
                if (characterArray[i].PlayerIndex == 0)
                {
                    player1 = characterArray[i];
                }

                if (characterArray[i].PlayerIndex == 1)
                {
                    player2 = characterArray[i];
                }
            }
        }
    }

    private IEnumerator RestartGame()
    {
        Time.timeScale = 0.5f;
        UIManager.Instance.countdownActive = false;
        yield return new WaitForSeconds(1.5f);
        player1.ResetPercentage();
        player2.ResetPercentage();
        GameStateManager.Instance.LoadGameplayScene(GameStateManager.fightingScene1);
    }
}
