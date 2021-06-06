using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FakeStarSkyboxController : MonoBehaviour
{
    private LightingManager lightingManager;
    public Material starSkyMaterial;
    private float lerpAlpha = 0;
    private void Start()
    {
        lightingManager = GameObject.FindObjectOfType<LightingManager>();
    }
    private void Update()
    {
        lerpAlpha = Mathf.Clamp01(lerpAlpha);
        transform.rotation = lightingManager.DirectionalLight.transform.rotation;
        if (lightingManager.TimeOfDay > 19)
            starSkyMaterial.SetFloat("StarAlpha", lerpAlpha += 0.001f);
        if (lightingManager.TimeOfDay > 5 && lightingManager.TimeOfDay < 19)
            starSkyMaterial.SetFloat("StarAlpha", lerpAlpha -= 0.001f);
    }
}
