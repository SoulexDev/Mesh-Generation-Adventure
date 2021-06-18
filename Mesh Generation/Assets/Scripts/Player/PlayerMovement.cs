using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float x, y;
    public float moveSpeed = 20;
    public float maxSpeed = 10;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float groundDistance = 0.2f;
    private Transform feet;
    public Transform playerCam;
    public Transform orient;
    private GameObject head;
    private GameObject brain;
    private GameObject cameraHolder;
    private Collider playerCollider;
    private Rigidbody rb;
    public bool grounded = false;
    bool crouching = false;
    bool jumping = false;
    void Start()
    {
        orient = transform.GetChild(0);
        head = orient.GetChild(0).gameObject;
        cameraHolder = head.transform.GetChild(0).gameObject;
        brain = head.transform.GetChild(1).gameObject;
        feet = orient.transform.GetChild(1).transform;
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
    }
    void Update()
    {
        MyInput();
        GroundCheck();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    void MyInput()
    {
        float multiplier;
        if (!grounded)
            multiplier = 0.5f;
        else
            multiplier = 1;
        x = Input.GetAxisRaw("Horizontal") * moveSpeed * multiplier * Time.fixedDeltaTime;
        y = Input.GetAxisRaw("Vertical") * moveSpeed * multiplier * Time.fixedDeltaTime;
    }
    void Movement()
    {
        //Vector2 mag = FindVelRelativeToLook();
        //float xMag = mag.x, yMag = mag.y;
        //CounterMovement(x, y, mag);

        if (x >= maxSpeed) x = maxSpeed;
        if (y >= maxSpeed) y = maxSpeed;

        rb.velocity = (x * orient.right) + rb.velocity + (y * orient.forward);
        if (grounded && Mathf.Abs(x) <= 0 && Mathf.Abs(y) <= 0)
            rb.velocity = Vector3.zero;
            //rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, rb.velocity.magnitude);
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orient.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orient.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orient.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    void GroundCheck()
    {
        if (Physics.CheckSphere(feet.position, groundDistance, 3))
            grounded = true;
        else 
            grounded = false;
    }
    //private void OnCollisionStay(Collision other)
    //{
    //    //Make sure we are only checking for walkable layers
    //    int layer = other.gameObject.layer;
    //    if (whatIsGround != (whatIsGround | (1 << layer))) return;

    //    //Iterate through every collision in a physics update
    //    for (int i = 0; i < other.contactCount; i++)
    //    {
    //        Vector3 normal = other.contacts[i].normal;
    //        //FLOOR
    //        if (IsFloor(normal))
    //        {
    //            grounded = true;
    //            cancellingGrounded = false;
    //            normalVector = normal;
    //            CancelInvoke(nameof(StopGrounded));
    //        }
    //    }

    //    //Invoke ground/wall cancel, since we can't check normals with CollisionExit
    //    float delay = 3f;
    //    if (!cancellingGrounded)
    //    {
    //        cancellingGrounded = true;
    //        Invoke(nameof(StopGrounded), Time.deltaTime * delay);
    //    }
    //}

    //private void StopGrounded()
    //{
    //    grounded = false;
    //}
}