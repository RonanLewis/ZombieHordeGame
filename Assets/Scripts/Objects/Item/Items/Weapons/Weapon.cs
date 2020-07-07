using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void UseItem()
    {
        Debug.Log("Equipped");
    }

    public override void DestroyItem()
    {

    }
}
