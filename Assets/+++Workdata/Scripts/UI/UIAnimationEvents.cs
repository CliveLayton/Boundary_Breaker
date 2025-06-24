using UnityEngine;

public class UIAnimationEvents : MonoBehaviour
{
    [SerializeField] private CanvasGroup startScreen;
    public void StartGame()
    {
        startScreen.HideCanvasGroup();
        GameStateManager.Instance.StartNewGame();
    }
}
