using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingItem : MonoBehaviour
{
    public Harvestable harvestable;
    private GameObject droppedObject;
    private Rigidbody droppedObjRb;
    private ParticleSystem mineParticles;
    private Material rockMat;
    private Mesh rockMesh;
    private float health;
    private bool playAnim = false;
    bool stoplerp = false;
    private void Start()
    {
        health = harvestable.health;
        rockMat = GetComponent<Material>();
        rockMesh = GetComponent<Mesh>();
    }
    public void TakeDamage(Tool tool)
    {
        if (harvestable.harvestingTool == Harvestable.HarvestingTool.AxePick)
            health -= tool.damage;
        Debug.Log($"Health {health}");
        Debug.Log($"Tool Damage{tool.damage}");
        Debug.Log($"Health {health}");
        ParticleSystemRenderer particleRender = harvestable.mineParticles.GetComponent<ParticleSystemRenderer>();
        particleRender.sharedMaterial = rockMat;
        particleRender.mesh = rockMesh;
        mineParticles = Instantiate(harvestable.mineParticles, transform.position, Quaternion.identity, transform);
        if (health <= 0)
        {
            stoplerp = false;
            playAnim = true;
            for (int t = 0; t < harvestable.droppedItems.Length; t++)
            {
                droppedObject =  Instantiate(harvestable.droppedItems[t], transform.position, Quaternion.Euler(Random.Range(0,90), Random.Range(0, 90), Random.Range(0, 90)));
                droppedObjRb = droppedObject.GetComponent<Rigidbody>();
                if(droppedObjRb != null)
                    droppedObjRb.AddForce(Vector3.up * 2, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }
    IEnumerator DestroyParticleMesh()
    {
        if(mineParticles != null)
            if(mineParticles.isStopped)
                Destroy(mineParticles);
        return null;
    }
    private void Update()
    {
        if (playAnim)
        {
            PlayMineAnim();
        }
    }
    void PlayMineAnim()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale - new Vector3(0.1f, 0.1f, 0.1f), 1);
        if (transform.localScale == transform.localScale - new Vector3(0.1f, 0.1f, 0.1f))
            playAnim = false;
    }
}
