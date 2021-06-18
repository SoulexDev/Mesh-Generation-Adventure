using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingHandler : MonoBehaviour
{
    public VolumeProfile baseProcessEffect;
    public VolumeProfile underWaterProcessEffect;
    public Volume volume;
    public Transform playerCam;
    private World world;
    public bool underWater;
    void Start()
    {
        world = FindObjectOfType<World>();
        volume = GetComponent<Volume>();
    }
    void Update()
    {
        if (playerCam.position.y < world.waterHeight / world.terrainSettings.terrainHeight * 5)
        {
            volume.profile = underWaterProcessEffect;
            underWater = true;
            RenderSettings.fogStartDistance = 20;
            RenderSettings.fogEndDistance = 100;
        }
        else if(volume.profile != baseProcessEffect)
        {
            volume.profile = baseProcessEffect;
            underWater = false;
            RenderSettings.fogStartDistance = 95;
            RenderSettings.fogEndDistance = 160;
        }
    }
}
