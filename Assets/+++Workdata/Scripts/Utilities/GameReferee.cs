using System.Collections;
using UnityEngine;

public class GameReferee : MonoBehaviour
{
    private PlayerStateMachine player1;
    private PlayerStateMachine player2;
    
    private void Awake()
    {
        UIManager.Instance.onTimerExpired += RestartMatch;
        GameStateManager.Instance.onStateChanged += GetPlayers;
    }

    private void OnDisable()
    {
        UIManager.Instance.onTimerExpired -= RestartMatch;
        GameStateManager.Instance.onStateChanged -= GetPlayers;
    }
    
    private void RestartMatch()
    {
        if (Mathf.Approximately(player1.PercentageCount, player2.PercentageCount))
        {
            StartCoroutine(RestartGame(-1, 0.5f));
        }
        else if (player1.PercentageCount > player2.PercentageCount)
        {
            StartCoroutine(RestartGame(1, 0.5f)); 
        }
        else if (player2.PercentageCount > player1.PercentageCount)
        {
            StartCoroutine(RestartGame(0, 0.5f));
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
    
    public IEnumerator RestartGame(int playerIndex, float restartDelay = 1.5f)
    {
        if (playerIndex != -1 && PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].Wins < 2)
        {
            PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].Wins += 1; 
        }

        Time.timeScale = 0.5f;
        UIManager.Instance.countdownActive = false;
        yield return new WaitForSeconds(restartDelay);
        player1.ResetCharacter();
        player2.ResetCharacter();
        
        if (playerIndex != -1 && PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].Wins == 2)
        {
            UIManager.Instance.EnterWinningScreen(playerIndex);
        }
        else
        {
            GameStateManager.Instance.LoadGameplayScene(GameStateManager.fightingScene1);
        }
    }
}
