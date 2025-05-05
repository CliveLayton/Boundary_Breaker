using UnityEngine;

public static class UIExtensionMethods
{
    /// <summary>
    /// sets alpha to 0 and set interactable and blocksRaycasts to false
    /// </summary>
    /// <param name="myCanvasGroup">this canvas group</param>
    public static void HideCanvasGroup(this CanvasGroup myCanvasGroup)
    {
        myCanvasGroup.alpha = 0f;
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;
    }
    
    /// <summary>
    /// sets alpha to 1 and set interactable and blockRaycasts to true
    /// </summary>
    /// <param name="myCanvasGroup">this canvas group</param>
    public static void ShowCanvasGroup(this CanvasGroup myCanvasGroup)
    {
        myCanvasGroup.alpha = 1f;
        myCanvasGroup.interactable = true;
        myCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// sets only interactable and blockRaycasts to false to still display the menu
    /// </summary>
    /// <param name="myCanvasGroup"></param>
    public static void DisableInteraction(this CanvasGroup myCanvasGroup)
    {
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;
    }
}
