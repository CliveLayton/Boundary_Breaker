using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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
                chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles = new Vector3(
                    chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles.x, 100,
                    chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles.z);
                chosedPlayer.transform.SetParent(CharacterPool.Instance.Player1PoolParent);
            }

            if (playerIndex == 1)
            {
                chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles = new Vector3(
                    chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles.x, 260,
                    chosedPlayer.GetComponent<PlayerStateMachine>().Anim.transform.eulerAngles.z);
                chosedPlayer.transform.SetParent(CharacterPool.Instance.Player2PoolParent);
            }
        }
        
        if (GameStateManager.Instance.currentState != GameStateManager.GameState.InMainMenu)
        {
            return;
        }

        if (playerIndex == 0)
        {
            if (index == 2)
            {
                chosedPlayer = CharacterPool.Instance.GetP1PooledObject(Random.Range(0, 2));
            }
            else
            {
                chosedPlayer = CharacterPool.Instance.GetP1PooledObject(index);
            }
        }

        if (playerIndex == 1)
        {
            if (index == 2)
            {
                chosedPlayer = CharacterPool.Instance.GetP2PooledObject(Random.Range(0, 2));
            }
            else
            {
                chosedPlayer = CharacterPool.Instance.GetP2PooledObject(index);
            }
        }
    }

    private void OnConfirmSelection()
    {
        PlayerStateMachine player = chosedPlayer.GetComponent<PlayerStateMachine>();
        if (playerIndex == 0)
        {
            UIManager.Instance.Player1 = player;
        }
        else if (playerIndex == 1)
        {
            UIManager.Instance.Player2 = player;
        }
        player.PlayerIndex = playerIndex;
        PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].IsReady = true;
        UIManager.Instance.PlayerSelectionUI(playerIndex, true);
        UIManager.Instance.CheckAllPlayerReady();
    }

    public void OnDeselect(InputAction.CallbackContext context)
    {
        if (context.performed && chosedPlayer != null)
        {
            PlayerConfigurationManager.Instance.PlayerConfigs[playerIndex].IsReady = false;
            UIManager.Instance.PlayerSelectionUI(playerIndex, false); 
        }
    }
    

    #endregion
}
