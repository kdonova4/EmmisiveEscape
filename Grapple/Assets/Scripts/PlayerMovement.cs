// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    //Assingables
    public Transform playerCam;
    public Transform orientation;
    public Camera cam;
    public Image pickupSprite;
    //Other
    private Rigidbody rb;
    public float range = 4f;
    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;
    [SerializeField]  private AudioClip[] stepSounds;
    [SerializeField] private AudioSource stepSource;
    [SerializeField] private AudioSource pickupSounds;
    [SerializeField] private AudioClip ammoPickup;
    [SerializeField] private AudioClip cardPickup;
    private Keycard doorControl;
    private DoorOpen smallDoorControl;
    private AmmoAmount ammo;
    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    public Transform Head;
    public Transform gun;
    public Transform flashlight;
    public CamRecoil bob;
    private SimpleShoot gunSway;
    private FlashlightController flashlightBob;
    public float bobRate = .50f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    private List<GameObject> keycards = new List<GameObject>();
    public GameObject smallDoor;
    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    
    //Input
    float x, y;
    bool jumping, sprinting, crouching;
    
    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        doorControl = FindObjectOfType(typeof(Keycard)) as Keycard;
        stepSource = GetComponent<AudioSource>();
        bob = Head.GetComponent<CamRecoil>();
        gunSway = gun.GetComponent<SimpleShoot>();
        flashlightBob = flashlight.GetComponent<FlashlightController>();
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
            // Make the game run as fast as possible
            Application.targetFrameRate = 300;
        pickupSprite.gameObject.SetActive(false);
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
        BobRate();
        KeycardPickup();
        KeycardTry();
        OpenSmallDoor();
        PickupAmmo();
        DisplayCursor();
    }


    void DisplayCursor()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {

        }


        if (hit.transform.CompareTag("Keycard") || hit.transform.tag == "Access" || hit.transform.tag == "Ammo" || hit.transform.tag == "Button" || hit.transform.tag == "Win")
        {
            pickupSprite.gameObject.SetActive(true);
        }
        else
        {
            pickupSprite.gameObject.SetActive(false);
        }
    }
    public void KeycardPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range + 10f))
        {

        }
        if (hit.transform.CompareTag("Keycard") && Input.GetKeyDown(KeyCode.Q))
        {
            
            if(hit.collider.gameObject.active)
            {
                pickupSounds.clip = cardPickup;
                pickupSounds.PlayOneShot(pickupSounds.clip);
            }
            hit.collider.gameObject.SetActive(false);
            keycards.Add(hit.collider.gameObject);

            

        }

    }
    void KeycardTry()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range + 10f))
        {

        }

        if (hit.collider.tag == "Access" && Input.GetKeyDown(KeyCode.Q))
        {
            doorControl = hit.transform.GetComponentInParent<Keycard>();
            Animator door = hit.collider.gameObject.GetComponentInParent<Animator>();
            doorControl.OpenDoor(door);
            

            Debug.Log(hit.collider.gameObject);
        }
        if (hit.collider.tag == "Win" && Input.GetKeyDown(KeyCode.Q))
        {
            
        }

    }

    void PickupAmmo()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range + 10f))
        {

        }
        if (hit.collider.tag == "Ammo" && Input.GetKeyDown(KeyCode.Q))
        {
            pickupSounds.clip = ammoPickup;
            pickupSounds.PlayOneShot(pickupSounds.clip);
            ammo = hit.transform.GetComponent<AmmoAmount>();
            int ammoPickedUp = ammo.GetAmmo();
            gunSway.AddTotalAmmo(ammoPickedUp);
            Destroy(hit.collider.gameObject);
            Debug.Log(hit.collider.gameObject);
        }
    }

    void OpenSmallDoor()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range + 10f))
        {

        }
        if (hit.collider.tag == "Button" && Input.GetKeyDown(KeyCode.Q))
        {
            smallDoorControl = hit.transform.GetComponentInParent<DoorOpen>();
            Animator door = hit.collider.gameObject.GetComponentInParent<Animator>();
            if(door.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) smallDoorControl.OpenSmallDoor(door);



            Debug.Log(hit.collider.gameObject);
        }

    }

    public bool CorrectKeycard(GameObject keycard)
    {
        if(keycards.Contains(keycard))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<GameObject> getKeycardList()
    {
        return keycards;
    }

    private void SelectFootstepSounds()
    {
        int n = Random.Range(1, stepSounds.Length);
        stepSource.clip = stepSounds[n];
        stepSource.PlayOneShot(stepSource.clip);
    }
    private void BobRate()
    {
        
        if(grounded && !crouching)
        {
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                
                bobRate -= Time.deltaTime;

                if (bobRate < 0 && !sprinting)
                {
                    SelectFootstepSounds();
                    bob.HeadBob();
                    bobRate = .50f;
                    gunSway.gunBobbing();
                    flashlightBob.FlashBobbing();
                    maxSpeed = 10f;
                }
                else if(bobRate < 0 && sprinting)
                {
                    SelectFootstepSounds();
                    bob.HeadBob();
                    bobRate = .25f;
                    gunSway.SprintGunBobbing();
                    flashlightBob.SprintFlashBobbing();
                    maxSpeed = 20f;
                }
            }
            
        }
        
    }
    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.C);
        sprinting = Input.GetKey(KeyCode.LeftShift);
        //Crouching
        if (Input.GetKeyDown(KeyCode.C))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.C))
            StopCrouch();
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }
    
    

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }
    
    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }
    
}
