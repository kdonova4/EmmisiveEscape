using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Position")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 50f;
    public float smoothRotationAmount = 12f;
    public float bobAmount = 25f;
    public float sprintBobAmount = 25f;

    public Transform recoilPosition;
    public Transform rotationPoint;

    Vector3 rotationalRecoil;
    public Vector3 RecoilRotation = new Vector3(10, 5, 7);

    Vector3 positionalRecoil;
    Vector3 Rot;
    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 98f;

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;
    [Space]

    public GameObject flashlight;
    public AudioSource flashlightOn;
    public AudioSource flashlightOff;
    //Internal privates
    private Vector3 initailPosition;

    private Quaternion initialRotation;
    private Vector3 velocity = Vector3.zero;
    private float InputX;
    private float InputY;

    // Start is called before the first frame update
    void Start()
    {
        initailPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        flashlight.gameObject.SetActive(false);
    }
    void FixedUpdate()
    {
        rotationalRecoil = Vector3.SmoothDamp(rotationalRecoil, Vector3.zero, ref velocity, rotationalReturnSpeed * Time.deltaTime);
        //rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, transform.localPosition, positionalReturnSpeed * Time.deltaTime);
        
        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.deltaTime);
        rotationPoint.localRotation = Quaternion.Euler(Rot);
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            return;
        }
        else
        {
            

           CalculateSway();

            MoveSway();

            TiltSway();

        }
        

               
    }

    public void FlashBobbing()
    {
        rotationalRecoil += new Vector3(bobAmount, 0, 0);
        //positionalRecoil += new Vector3(Random.Range(-RecoilKickback.x, RecoilKickback.x), Random.Range(-RecoilKickback.y, RecoilKickback.y), RecoilKickback.z);
    }
    public void SprintFlashBobbing()
    {
        rotationalRecoil += new Vector3(sprintBobAmount, 0, 0);
        //positionalRecoil += new Vector3(Random.Range(-RecoilKickback.x, RecoilKickback.x), Random.Range(-RecoilKickback.y, RecoilKickback.y), RecoilKickback.z);
    }

    void JumpSway()
    {

    }

    void LandSway()
    {

    }

  

    void MoveSway()
    {
        InputX = -Input.GetAxis("Mouse X");
        InputY = -Input.GetAxis("Mouse Y");
    }

    void CalculateSway()
    {
        
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);


        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Slerp(transform.localPosition, finalPosition + initailPosition, Time.deltaTime * smoothAmount);
    }

    void TiltSway()
    {

        float tiltX = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltY = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);


        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            rotationX ? -tiltY : 0f, 
            rotationY ? tiltX : 0f, 
            rotationZ ? tiltY : 0));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotationAmount);
    }
}
