using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField]
    //Settings that should be accesible by the player
    [Header("Player Settings")]
    public bool headBob;
    public bool autoJump;
    public bool fieldOfViewIncreases;
    //Settings that control and change in the character controller component
    [Header("Character Controller Settings")]
    public float slopeLimit;
    public float stepHeight;
    public float minimumMovement;
    //Movement control
    [Header("Movement Control")]
    private static Transform orient;
    private float x;
    private float y;
    public float moveSpeed;
    public float jumpHeight;
    public float crouchingSpeed;
    [Range(2, 5)]
    public float runningSpeed;
    //Controlling somewhat realistic forces
    [Header("Force Control")]
    public float gravity;
    public float mass;
    public float airDrag;
    [Range(0, 1)]
    public float airSpeed;
    [Range(0, 1)]
    public float groundDistance;
    //Checks for the force control
    [Header("Force Control Checks")]
    public GameObject groundCheck;
    public bool isGrounded;
    public LayerMask ground;
    //Extras
    [Header("Extras")]
    private GameObject head;
    private GameObject brain;
    private GameObject cameraHolder;
    private Camera playerCam;
    [Range(30, 125)]
    public float fieldOfVision;
    [Range(0, 10)]
    public float headBobSpeed;
    public float headBobIntensity;
    [Range(1, 10)]
    public float crouchLerpSpeed;
    private Vector3 velocity;
    //Floats to store original values
    private float airSpeedContainer;
    private float crouchSpeedContainer;
    private float runningSpeedContainer;
    private float headBobSpeedContainer;
    private float headBobIntensityContainer;
    private float stepheightContainer;
    private float slopeLimitContainer;
    private float minimumMovementContainer;
    //Extra stuff
    private bool crouching = false;
    private bool running = false;
    private bool settingsPlaced = false;
    public bool creativeMode;
    public float creativeModeFlySpeed;
    private float yPos;
    private float walkTime;
    private Vector3 targetCamPos;
    private Vector3 currentVelocity;
    private float lerpTime;
    void Start()
    {
        //Allows the player to go up units like stairs
        if (autoJump)
            stepHeight = 1;
        orient = transform.GetChild(0);
        head = orient.GetChild(0).gameObject;
        cameraHolder = head.transform.GetChild(0).gameObject;
        brain = head.transform.GetChild(1).gameObject;
        playerCam = cameraHolder.transform.GetChild(0).GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();
        airSpeedContainer = airSpeed;
        crouchSpeedContainer = crouchingSpeed;
        runningSpeedContainer = runningSpeed;
        headBobSpeedContainer = headBobSpeed;
        headBobIntensityContainer = headBobIntensity;
        stepheightContainer = stepHeight;
        slopeLimitContainer = slopeLimit;
        minimumMovementContainer = minimumMovement;
        airSpeed = 1;
        crouchingSpeed = 1;
        playerCam.fieldOfView = fieldOfVision;
    }
    void Update()
    {
        //Allows for better readability
        DetectMyInput();
        DoMovement();
        DoCrouching();
        CharacterControllerSettings();
        if (isGrounded && headBob)
            DoHeadBob();
    }
    void DetectMyInput()
    {
        //Player movement inputs
        x = Input.GetAxisRaw("Horizontal") * moveSpeed * crouchingSpeed * runningSpeed;
        y = Input.GetAxisRaw("Vertical") * moveSpeed * crouchingSpeed * runningSpeed;
        //Creative Mode Inputs
        if (Input.GetKey(KeyCode.Space))
        {
            yPos = 1;
            yPos *= 25;
        }
        else
            yPos = Mathf.Lerp(yPos, 0, Time.deltaTime * 10);
        if (Input.GetKey(KeyCode.LeftControl))
        {
            yPos = 1;
            yPos *= -25;
        }
        else
            yPos = Mathf.Lerp(yPos, 0, Time.deltaTime * 10);
        //Crouching and running inputs
        if (Input.GetKey(KeyCode.LeftControl))
            crouching = true;
        else
            crouching = false;
        if (Input.GetKey(KeyCode.LeftShift))
            running = true;
        else
            running = false;
    }
    void DoMovement()
    {
        //Sets values when running
        if (running && y > 0.1f)
        {
            runningSpeed = runningSpeedContainer / 1.5f;
            headBobSpeed = headBobSpeedContainer * 1.5f;
            headBobIntensity = headBobIntensityContainer * 1.5f;
            if (lerpTime < 1)
            {
                lerpTime += 0.0055f;
            }
            if (fieldOfViewIncreases)
                playerCam.fieldOfView = Mathf.Lerp(fieldOfVision, fieldOfVision * 1.3f, lerpTime);
        }
        else
        {
            runningSpeed = 1;
            headBobSpeed = headBobSpeedContainer;
            headBobIntensity = headBobIntensityContainer;
            if (lerpTime > 0.0f)
            {
                lerpTime -= 0.0065f;
            }
            if (fieldOfViewIncreases)
                playerCam.fieldOfView = Mathf.Lerp(fieldOfVision, fieldOfVision * 1.3f, lerpTime);
        }
        //Check if the player is on the ground, not using the built in ground check because it is not reliable
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, ground);
        //Set movement
        Vector3 upDownMovement = (yPos * Vector3.up);
        Vector3 movePos = (y * orient.forward + x * orient.right);
        Vector3 airMovePos = (((y * airSpeed) * orient.forward) + ((x * airSpeed) * orient.right));
        //Make the player jump
        if (Input.GetButton("Jump") && isGrounded && !creativeMode)
        {
            StoreVelocity();
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * -gravity);
        }
        if (isGrounded)
        {
            airSpeed = 1;
            currentVelocity = Vector3.zero;
        }
        else if (!creativeMode)
        {
            movePos *= 0;
            airSpeed = airSpeedContainer;
            characterController.Move(new Vector3(currentVelocity.x + airMovePos.x, currentVelocity.y * 0, currentVelocity.z + airMovePos.z) * (mass / airDrag) * Time.deltaTime);
        }
        //if (isGrounded && velocity.y <= 0)
        //    velocity.y = -3;
        //Add forces to the player
        if (!creativeMode)
        {
            velocity.y -= gravity * Time.deltaTime;
            characterController.Move((movePos + velocity) * Time.deltaTime);
        }
        else
            characterController.Move(((movePos * creativeModeFlySpeed) + upDownMovement) * Time.deltaTime);
        if (movePos.magnitude > 0)
            StoreVelocity();
    }
    void StoreVelocity()
    {
        currentVelocity = characterController.velocity;
    }
    void DoCrouching()
    {
        //Do crouching :D
        if (crouching)
        {
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, new Vector3(0, -0.5f, 0), crouchLerpSpeed * 6 * Time.deltaTime);
            characterController.height = Mathf.Lerp(characterController.height, 1, crouchLerpSpeed * 6 * Time.deltaTime);
            characterController.center = Vector3.Lerp(characterController.center, new Vector3(0, -0.5f, 0), crouchLerpSpeed * 6 * Time.deltaTime);
            crouchingSpeed = crouchSpeedContainer;
        }
        else
        {
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, new Vector3(0, 0.5f, 0), crouchLerpSpeed * 4 * Time.deltaTime);
            characterController.height = Mathf.Lerp(characterController.height, 2, crouchLerpSpeed * 6 * Time.deltaTime);
            characterController.center = Vector3.Lerp(characterController.center, new Vector3(0, 0, 0), crouchLerpSpeed * 6 * Time.deltaTime);
            crouchingSpeed = 1;
        }
    }
    void DoHeadBob()
    {
        //Check if the player is moving
        if (Mathf.Abs(x) > 0.2 || Mathf.Abs(y) > 0.2)
            walkTime += Time.deltaTime;
        else
        {
            walkTime = 0;
            cameraHolder.transform.localPosition = Vector3.Lerp(cameraHolder.transform.localPosition, brain.transform.localPosition, 10 * Time.deltaTime);
        }
        //Set the head to bob
        targetCamPos = brain.transform.localPosition + CalculateHeadBob(walkTime);
        cameraHolder.transform.localPosition = Vector3.Lerp(brain.transform.localPosition, targetCamPos, 0.1f);
    }
    public Vector3 CalculateHeadBob(float time)
    {
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 camOffset = Vector3.zero;
        //Do cosine and sine calculations to move the camera
        if (time > 0)
        {
            horizontalOffset = Mathf.Cos(time * headBobIntensity * 2) * 0.1f * headBobSpeed;
            verticalOffset = Mathf.Sin(time * headBobIntensity * 1.5f) * 0.1f * headBobSpeed;
            camOffset = brain.transform.right * horizontalOffset + brain.transform.up * verticalOffset;
        }
        return camOffset;
    }
    void CharacterControllerSettings()
    {
        if (!isGrounded)
        {
            settingsPlaced = false;
            stepHeight = 0;
        }
        else
        {
            settingsPlaced = false;
            stepHeight = stepheightContainer;
        }
        //Set the character controller settings
        if (!settingsPlaced)
        {
            characterController.slopeLimit = slopeLimit;
            characterController.stepOffset = stepHeight;
            characterController.minMoveDistance = minimumMovement;
            settingsPlaced = true;
        }
    }
}
