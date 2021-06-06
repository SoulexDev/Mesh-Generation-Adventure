using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structures", menuName = "Structures/NewStructure", order = 1)]
public class Structure : ScriptableObject
{
    public GameObject prefabObject;
    public float scatterAmount;
    public float regionDensity;
}