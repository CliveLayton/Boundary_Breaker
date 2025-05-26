using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Canvas;
    [SerializeField] private GameObject player2Canvas;
    [SerializeField] private GameObject FirstSelectedP1;
    [SerializeField] private GameObject FirstSelectedP2;
    
    public readonly List<PlayerConfiguration> PlayerConfigs = new List<PlayerConfiguration>();
    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void HandlePlayerJoined(PlayerInput pi)
    {
        Debug.Log("Player Joined: " + pi.playerIndex);
        pi.transform.SetParent(transform);
        
        PlayerConfigs.Add(new PlayerConfiguration(pi));
        MultiplayerEventSystem multiplayerEventSystem = pi.GetComponent<MultiplayerEventSystem>();
        pi.uiInputModule = pi.gameObject.AddComponent<InputSystemUIInputModule>();

        if (pi.playerIndex == 0)
        {
            multiplayerEventSystem.firstSelectedGameObject = FirstSelectedP1;
            multiplayerEventSystem.playerRoot = player1Canvas;
        }
        else if (pi.playerIndex == 1)
        {
            multiplayerEventSystem.firstSelectedGameObject = FirstSelectedP2;
            multiplayerEventSystem.playerRoot = player2Canvas;
        }
    }

    public void CheckAllPlayerReady()
    {
        bool player1Ready = false;
        bool player2Ready = false;
        
        for (int i = 0; i < PlayerConfigs.Count; i++)
        {
            if (i == 0)
            {
                player1Ready = PlayerConfigs[i].IsReady;
            }

            if (i == 1)
            {
                player2Ready = PlayerConfigs[i].IsReady;
            }
        }

        if (player1Ready && player2Ready)
        {
            UIManager.Instance.CharacterSelected();
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    
    public PlayerInput Input { get; set; }
    
    public int PlayerIndex { get; set; }

    public bool IsReady { get; set; }
}
