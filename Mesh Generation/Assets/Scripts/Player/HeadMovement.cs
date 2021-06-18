using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMovement : MonoBehaviour
{
    public Transform camHolder;
    void Update()
    {
        transform.rotation = Quaternion.Euler(camHolder.rotation.eulerAngles.z, camHolder.rotation.eulerAngles.y + 90, camHolder.rotation.eulerAngles.x);
    }
}
