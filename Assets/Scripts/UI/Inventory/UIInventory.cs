using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public GameObject itemFieldInfoPrefab;

    public GameObject itemInfoPanel;
    private GameObject itemNamePanel;
    private GameObject itemDescriptionPanel;
    private GameObject itemDetailsPanel;
    private GameObject itemActionButton;


    private UnityAction inventoryUpdate;
    private UnityAction openInventory;
    private UnityAction closeInventory;
    private GameObject player;
    private CanvasGroup canvasGroup;
    private Transform inventoryContent;

    private void Awake()
    {
        inventoryUpdate = new UnityAction(UpdateInventoryUI);
        openInventory = new UnityAction(OpenInventory);
        closeInventory = new UnityAction(CloseInventory);
    }

    private void OnEnable()
    {
        EventManager.StartListening("inventoryUpdate", inventoryUpdate);
        EventManager.StartListening("openInventory", openInventory);
        EventManager.StartListening("closeInventory", closeInventory);
    }

    private void OnDisable()
    {
        EventManager.StopListening("inventoryUpdate", inventoryUpdate);
        EventManager.StopListening("openInventory", openInventory);
        EventManager.StopListening("closeInventory", closeInventory);
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = transform.GetComponent<CanvasGroup>();
        inventoryContent = transform.GetComponentInChildren<GridLayoutGroup>().transform;
        player = GameObject.FindGameObjectWithTag("Player");

        itemNamePanel = itemInfoPanel.transform.Find("ItemNamePanel").gameObject;
        itemDescriptionPanel = itemInfoPanel.transform.Find("ItemDetailsPanel/ItemDescription").gameObject;
        itemDetailsPanel = itemInfoPanel.transform.Find("ItemDetailsPanel/ItemDetails").gameObject;
        itemActionButton = itemInfoPanel.transform.Find("ItemActionPanel/ActionButton").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rt = (RectTransform)inventoryContent;
        rt.anchoredPosition = Vector3.zero;
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
        List<GameObject> inv = player.GetComponent<PlayerInventory>().inventory;
        for (int i = 0; i < inv.Count; i++)
        {
            GameObject newItem; // Create GameObject instance
            newItem = Instantiate(itemSlotPrefab, inventoryContent);
            newItem.GetComponent<UIItem>().item = inv[i];
            Item item = inv[i].GetComponent<Item>();
            newItem.GetComponent<Button>().onClick.AddListener(() => DisplayItemStats(newItem.GetComponent<UIItem>()));
            newItem.GetComponent<UIItem>().icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/RPG_inventory_icons/" + item.itemClass.ToString());
        }

    }
    private void BuildItemSlot()
    {

    }
    private void OpenInventory()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        PlayerCameraController.isLocked = true;
        GameHandler.UnlockCursor();
        PlayerMovementController.isLocked = true;
    }
    private void CloseInventory()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        PlayerCameraController.isLocked = false;
        GameHandler.LockCursor();
        PlayerMovementController.isLocked = false;
        CloseInfoPanel();
    }

    private void DisplayItemStats(UIItem uiItem)
    {
        OpenInfoPanel();
        Item item = uiItem.item.GetComponent<Item>();
        player.GetComponent<PlayerInventory>().selectedItem = item.gameObject;
        foreach (FieldInfo fieldInfo in item.GetType().GetFields())
        {

            if (fieldInfo.Name == "description")
            {
                itemDescriptionPanel.GetComponentInChildren<Text>().text = fieldInfo.GetValue(item).ToString();
                continue;
            }
            if (fieldInfo.Name == "itemName")
            {
                itemNamePanel.GetComponentInChildren<Text>().text = fieldInfo.GetValue(item).ToString();
                continue;
            }
            if (fieldInfo.Name == "action")
            {
                itemActionButton.GetComponent<Button>().onClick.AddListener(item.UseItem);
                itemInfoPanel.transform.Find("ItemActionPanel/DropButton").GetComponent<Button>().onClick.AddListener(() => player.GetComponent<PlayerInventory>().Drop(uiItem.item));
                itemInfoPanel.transform.Find("ItemActionPanel/DropButton").GetComponent<Button>().onClick.AddListener(() => CloseInfoPanel());
                itemActionButton.GetComponentInChildren<Text>().text = fieldInfo.GetValue(item).ToString();
                continue;
            }
            if (fieldInfo.Name == "itemType" || fieldInfo.Name == "itemClass")
            {
                continue;
            }
            GameObject newItem; // Create GameObject instance
            newItem = Instantiate(itemFieldInfoPrefab, itemDetailsPanel.GetComponentInChildren<VerticalLayoutGroup>().transform);
            newItem.transform.Find("FieldName").GetComponent<Text>().text = StringExtensions.FirstLetterToUpper(fieldInfo.Name);
            newItem.transform.Find("FieldValue").GetComponent<Text>().text = fieldInfo.GetValue(item).ToString();

        }
    }

    private void OpenInfoPanel()
    {
        foreach (Transform child in itemDetailsPanel.GetComponentInChildren<VerticalLayoutGroup>().transform)
        {
            Destroy(child.gameObject);
        }
        CanvasGroup cg = itemInfoPanel.GetComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CloseInfoPanel()
    {
        foreach (Transform child in itemDetailsPanel.GetComponentInChildren<VerticalLayoutGroup>().transform)
        {
            Destroy(child.gameObject);
        }
        CanvasGroup cg = itemInfoPanel.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
