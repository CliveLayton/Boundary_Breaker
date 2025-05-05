using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerCharacterSelection : MonoBehaviour
{
    public GameObject player1SelectionPrefab;
    public GameObject player2SelectionPrefab;

    public PlayerInput playerInput;

    private void Awake()
    {
        CheckPlayerIndex( 0, player1SelectionPrefab);

        CheckPlayerIndex(1, player2SelectionPrefab, true);
    }

    private void CheckPlayerIndex(int index, GameObject playerSelection, bool showStartButton = false)
    {
        var rootMenu = GameObject.Find("Character Selection");
        if (rootMenu != null)
        {
            if (playerInput.playerIndex == index)
            {
                var menu = Instantiate(playerSelection, rootMenu.transform);

                if (showStartButton)
                {
                   UIManager.Instance.CharacterSelected();
                }
            }
        }
    }
}
