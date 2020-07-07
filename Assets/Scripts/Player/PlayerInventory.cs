using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    public GameObject selectedItem;
    private Camera playerCam;
    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameHandler.isPaused)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen)
            {
                EventManager.TriggerEvent("closeInventory");
                isOpen = false;
            }
            else
            {
                EventManager.TriggerEvent("openInventory");
                isOpen = true;
            }
        }
    }

    public void Add(GameObject itemGameObject)
    {
        if (!inventory.Contains(itemGameObject))
        {
            itemGameObject.SetActive(false);
            inventory.Add(itemGameObject);
            UpdateEvent();
        }
    }

    public void Delete(GameObject itemGameObject)
    {
        if (inventory.Contains(itemGameObject))
        {
            inventory.Remove(itemGameObject);
            Destroy(itemGameObject);
            UpdateEvent();
        }
    }

    public void Drop(GameObject itemGameObject)
    {
        if (inventory.Contains(itemGameObject))
        {
            itemGameObject.SetActive(true);
            itemGameObject.transform.position = playerCam.transform.position;
            itemGameObject.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * 10, ForceMode.Impulse);
            inventory.Remove(itemGameObject);
            UpdateEvent();
        }
    }

    private void UpdateEvent()
    {
        EventManager.TriggerEvent("inventoryUpdate");
    }

    public void DropSelectedItem()
    {
        if (inventory.Contains(selectedItem))
        {
            selectedItem.SetActive(true);
            selectedItem.transform.position = playerCam.transform.position;
            selectedItem.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * 10, ForceMode.Impulse);
            inventory.Remove(selectedItem);
            selectedItem = null;
            UpdateEvent();
        }
    }
}
