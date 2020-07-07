using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public GameObject HUD;
    public GameObject PauseMenu;
    public GameObject Inventory;
    private static CanvasGroup canvasGroupHUD;
    private static CanvasGroup canvasGroupPauseMenu;

    private void Start()
    {
        canvasGroupHUD = HUD.GetComponent<CanvasGroup>();
        canvasGroupPauseMenu = PauseMenu.GetComponent<CanvasGroup>();
    }
    public static void HideHUD()
    {
        canvasGroupHUD.alpha = 0;
    }

    public static void ShowHUD()
    {
        canvasGroupHUD.alpha = 1;
    }
}
