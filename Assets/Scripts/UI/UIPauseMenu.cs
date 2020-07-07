using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{

    private UnityAction openPauseMenu;
    private UnityAction closePauseMenu;
    private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();

    }
    private void Awake()
    {
        openPauseMenu = new UnityAction(OpenPauseMenu);
        closePauseMenu = new UnityAction(ClosePauseMenu);
    }
    // Update is called once per frame0
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EventManager.StartListening("openPauseMenu", openPauseMenu);
        EventManager.StartListening("closePauseMenu", closePauseMenu);
    }

    private void OnDisable()
    {
        EventManager.StopListening("openPauseMenu", openPauseMenu);
        EventManager.StopListening("closePauseMenu", closePauseMenu);
    }

    private void OpenPauseMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        UIHandler.HideHUD();
    }

    private void ClosePauseMenu()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        UIHandler.ShowHUD();
    }
}
