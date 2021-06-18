using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    public Image healthBar;
    public float currentHealth;
    public float normalizedHealth;
    private void Update()
    {
        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        normalizedHealth = currentHealth / maxHealth;
        healthBar.rectTransform.localScale = new Vector3(normalizedHealth, 1, 1);
    }
}
