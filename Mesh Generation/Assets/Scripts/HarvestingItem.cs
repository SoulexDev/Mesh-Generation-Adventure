using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingItem : MonoBehaviour
{
    public Harvestable harvestable;
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
                Instantiate(harvestable.droppedItems[t], transform.position, Quaternion.Euler(0,0,0));
            }
            Destroy(gameObject);
        }
    }
}
