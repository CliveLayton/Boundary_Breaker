using System.Collections.Generic;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    #region Variables

    public static CharacterPool Instance;

    [SerializeField] private GameObject[] charsToPool;
    [field: SerializeField] public Transform Player1PoolParent { get; private set; }
    [field: SerializeField] public Transform Player2PoolParent { get; private set; }

    private GameObject[] player1Pool;
    private GameObject[] player2Pool;

    #endregion
    
    #region Unity Methods

    void Awake()
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

    void Start()
    {
        player1Pool = new GameObject[charsToPool.Length];
        player2Pool = new GameObject[charsToPool.Length];

        for (int i = 0; i < charsToPool.Length; i++)
        {
            player1Pool[i] = Instantiate(charsToPool[i], Player1PoolParent);
            player1Pool[i].SetActive(false);
            player2Pool[i] = Instantiate(charsToPool[i], Player2PoolParent);
            player2Pool[i].SetActive(false);
        }
    }

    #endregion

    #region ObjectPooling Methods

    /// <summary>
    /// get a object out of the player1 Pool if not already active
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetP1PooledObject(int index)
    {
        if (!player1Pool[index].activeInHierarchy)
        {
            return player1Pool[index];
        }

        return null;
    }

    /// <summary>
    /// get a object out of the player2 Pool if nor already active
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetP2PooledObject(int index)
    {
        if (!player2Pool[index].activeInHierarchy)
        {
            return player2Pool[index];
        }

        return null;
    }

    #endregion
}
