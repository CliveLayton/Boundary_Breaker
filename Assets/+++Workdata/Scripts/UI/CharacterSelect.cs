using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    #region Variables

    //0 is ChainsawGirl, 1 is TigerBoy
    [SerializeField] private Sprite[] charSprites;
    [SerializeField] private ButtonEvents[] charButtons;
    [SerializeField] private Image characterIcon;
    [SerializeField] private int playerIndex;

    private GameObject chosedPlayer;
    private int currentSelectedCharIndex;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        currentSelectedCharIndex = 0;
        characterIcon.sprite = charSprites[0];
        for (int i = 0; i < charButtons.Length; i++)
        {
            int index = i;
            charButtons[i].onSelect += () => ChangePlayer(index);
            charButtons[i].onSubmit += OnConfirmSelection;
            charButtons[i].onMouseEnter += () => ChangePlayer(index);
            charButtons[i].onMouseClick += OnConfirmSelection;
        }
    }

    #endregion

    #region CharacterSelect Methods

    private void ChangePlayer(int index)
    {
        characterIcon.sprite = charSprites[index];
        currentSelectedCharIndex = index;

        if (chosedPlayer != null)
        {
            chosedPlayer.SetActive(false);
            if (playerIndex == 0)
            {
                chosedPlayer.transform.SetParent(CharacterPool.Instance.Player1PoolParent);
            }

            if (playerIndex == 1)
            {
                chosedPlayer.transform.SetParent(CharacterPool.Instance.Player2PoolParent);
            }
        }

        if (playerIndex == 0)
        {
            if (index == 2)
            {
                chosedPlayer = CharacterPool.Instance.GetP1PooledObject(Random.Range(0, 1));
            }
            else
            {
                chosedPlayer = CharacterPool.Instance.GetP1PooledObject(index);
            }

            GameObject parent = GameObject.Find("Player1");
            chosedPlayer.transform.SetParent(parent.transform);
            chosedPlayer.transform.localPosition = Vector3.zero;
            chosedPlayer.SetActive(true);
        }

        if (playerIndex == 1)
        {
            if (index == 2)
            {
                chosedPlayer = CharacterPool.Instance.GetP2PooledObject(Random.Range(0, 1));
            }
            else
            {
                chosedPlayer = CharacterPool.Instance.GetP2PooledObject(index);
            }

            GameObject parent = GameObject.Find("Player2");
            chosedPlayer.transform.SetParent(parent.transform);
            chosedPlayer.transform.localPosition = Vector3.zero;
            chosedPlayer.SetActive(true);
        }
    }

    private void OnConfirmSelection()
    {
        chosedPlayer.GetComponent<PlayerStateMachine>().PlayerIndex = playerIndex;
        PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].IsReady = true;
        UIManager.Instance.CheckAllPlayerReady();
    }

    #endregion
}
