using System.Collections;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField] private GameReferee referee;
    [SerializeField] private float damage;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float hitStopDuration = 0.3f;
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private Vector2 attackForce = new Vector2(1.5f,-2f);
    [SerializeField] private bool applyKnockDown;

    [SerializeField] private Vector3 position1;
    [SerializeField] private Vector3 position2;
    [SerializeField] private Vector3 position3;
    [SerializeField] private bool isRightWall;

    private PlayerStateMachine player1;
    private PlayerStateMachine player2;
    
    private BoxCollider col;
    private bool isOnWall1, isOnWall2;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        GameStateManager.Instance.onStateChanged += GetPlayers;
        SetupWall();
    }

    private void OnDisable()
    {
        GameStateManager.Instance.onStateChanged -= GetPlayers;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player1.gameObject == other.gameObject)
            {
                Player1OnWall();
            }
            else if (player2.gameObject == other.gameObject)
            {
                Player2OnWall();
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (player1.gameObject == other.gameObject && !player1.InHitStun)
            {
                isOnWall1 = false;
            }
            else if (player2.gameObject == other.gameObject & !player2.InHitStun)
            {
                isOnWall2 = false;
            }
        }
    }

    private void Player1OnWall()
    {
        if (player1.InHitStun)
        {
            return;
        }

        if (player1.CombinedForce.magnitude > 10f && !isOnWall1)
        {
            col.enabled = false;
            isOnWall1 = true;
            player1.Damage(damage, 0.5f, 0.5f, 
                new Vector2(3,0.8f), 0.4f, false, false, false, false, false);
            WallBreak(1);
        }
        else if(!isOnWall1)
        {
            isOnWall1 = true;
            player1.Damage(damage, stunDuration, hitStopDuration, 
                attackForce, knockBackTime, false, false, true, false, applyKnockDown);
        }
    }

    private void Player2OnWall()
    {
        if (player2.InHitStun)
        {
            return;
        }

        if (player2.CombinedForce.magnitude > 10f && !isOnWall2)
        {
            col.enabled = false;
            isOnWall2 = true;
            player2.Damage(damage, 0.5f, 0.5f, 
                new Vector2(3,0.8f), 0.4f, false, false, false, false, false);
            WallBreak(0);
        }
        else if(!isOnWall2)
        {
            isOnWall2 = true;
            player2.Damage(damage, stunDuration, hitStopDuration, 
                attackForce, knockBackTime, false, false, true, false, applyKnockDown);
        }
    }

    private void SetupWall()
    {
        if (isRightWall)
        {
            switch (GameStateManager.Instance.wallBreakCountR)
            {
                case 0:
                    transform.position = position1;
                    break;
                case 1:
                    transform.position = position2;
                    break;
                case 2:
                    transform.position = position3;
                    break;
            }
        }
        else
        {
            switch (GameStateManager.Instance.wallBreakCountL)
            {
                case 0:
                    transform.position = position1;
                    break;
                case 1:
                    transform.position = position2;
                    break;
                case 2:
                    transform.position = position3;
                    break;
            }
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

    private void WallBreak(int index)
    {
        if (isRightWall)
        {
            GameStateManager.Instance.wallBreakCountR += 1;
        }
        else
        {
            GameStateManager.Instance.wallBreakCountL += 1;
        }

        StartCoroutine(referee.RestartGame(index));
    }
}
