using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform orient;
    private Transform cameraHolder;
    private float x, z;
    public float cameraXSpeed, cameraYSpeed;
    private PlayerMovement player;
    bool cursorLocked = true;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindObjectOfType<PlayerMovement>();
        orient = player.transform.GetChild(0);
        cameraHolder = orient.transform.GetChild(0).gameObject.transform.GetChild(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            cursorLocked = !cursorLocked;
        if (cursorLocked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        x += Input.GetAxisRaw("Mouse X") * cameraXSpeed * Time.fixedDeltaTime;
        z += Input.GetAxisRaw("Mouse Y") * cameraYSpeed * Time.fixedDeltaTime;

        orient.rotation = Quaternion.Euler(0, x, 0);
        cameraHolder.rotation = Quaternion.Euler(-z, x, 0);
    }
}