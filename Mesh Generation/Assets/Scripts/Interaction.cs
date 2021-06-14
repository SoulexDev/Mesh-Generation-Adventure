using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public HarvestingItem harvestableItem;
    public Tool tool;
    private Camera playerCam;
    private void Start()
    {
        playerCam = GetComponent<Camera>();
    }
    void Update()
    {
        MyInput();
    }
    void MyInput()
    {
        if (Input.GetButtonDown("Fire1"))
            SwingItem();
    }
    void SwingItem()
    {
        Invoke("DoSwing", 0.7f);
    }
    void DoSwing()
    {
        RaycastHit hit;
        if (Physics.SphereCast(playerCam.transform.position, 0.6f, playerCam.transform.forward, out hit, 0.5f))
        {
            if(harvestableItem == null)
                harvestableItem = hit.collider.gameObject.GetComponent<HarvestingItem>();
            if(harvestableItem != null)
                harvestableItem.TakeDamage(tool);
        }
    }
}
