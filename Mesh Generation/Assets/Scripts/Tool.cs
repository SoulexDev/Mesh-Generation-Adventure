using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tools", menuName = "Tools/Tool")]
public class Tool : ScriptableObject
{
    public string toolName;
    public int damage = 10;
    public int swingSpeed = 1;
}
