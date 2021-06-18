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
            Invoke("SwingItem", 0.7f);
    }
    void SwingItem()
    {
        Debug.Log("Swang");
        RaycastHit hit;
        if (Physics.SphereCast(playerCam.transform.position, 1.3f, playerCam.transform.forward, out hit, 1.3f))
        {
            if(harvestableItem == null)
                harvestableItem = hit.collider.gameObject.GetComponent<HarvestingItem>();
            if(harvestableItem != null)
                harvestableItem.TakeDamage(tool);
        }
    }
}