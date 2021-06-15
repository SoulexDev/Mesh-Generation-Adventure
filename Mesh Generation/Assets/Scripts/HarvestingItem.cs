using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingItem : MonoBehaviour
{
    public Harvestable harvestable;
    private GameObject droppedObject;
    private Rigidbody droppedObjRb;
    private float health;
    private void Start()
    {
        health = harvestable.health;
    }
    public void TakeDamage(Tool tool)
    {
        if (harvestable.harvestingTool == Harvestable.HarvestingTool.AxePick)
            health -= tool.damage;
        if (health <= 0)
        {
            for (int t = 0; t < harvestable.droppedItems.Length; t++)
            {
                droppedObject =  Instantiate(harvestable.droppedItems[t], transform.position, Quaternion.Euler(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)));
                droppedObjRb = droppedObject.GetComponent<Rigidbody>();
                if(droppedObjRb != null)
                    droppedObjRb.AddForce(transform.up * 2 + transform.forward * 1.5f, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }
}
