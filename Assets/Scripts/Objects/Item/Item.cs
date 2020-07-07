using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public int weight;
    public int health = 100;
    public ItemType itemType;
    public ItemClass itemClass;
    public string action;
    private string highlightMatPath = "Materials/HighlightMaterial";
    private Material highlightMaterial;
    private void Start()
    {
        highlightMaterial = Resources.Load<Material>(highlightMatPath);
    }
    public abstract void UseItem();
    public abstract void DestroyItem();

    public void HighlightItem()
    {
        MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        List<Material> matArray = meshRenderer.materials.ToList<Material>();
        if (!matArray.Contains(highlightMaterial))
        {
            matArray.Add(highlightMaterial);
        }
        meshRenderer.materials = matArray.ToArray();
    }

    public void UnHighlightItem()
    {
        MeshRenderer meshRenderer = transform.gameObject.GetComponent<MeshRenderer>();
        List<Material> matArray = meshRenderer.materials.ToList<Material>();
        if (matArray.Contains(highlightMaterial))
        {
            matArray.Remove(highlightMaterial);
        }
        meshRenderer.materials = matArray.ToArray();
    }
}
