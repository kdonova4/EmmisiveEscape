using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Position")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotation")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 50f;
    public float smoothRotationAmount = 12f;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    //Internal privates
    private Vector3 initailPosition;

    private Quaternion initialRotation;

    private float InputX;
    private float InputY;

    // Start is called before the first frame update
    void Start()
    {
        initailPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButton(1))
        {
            return;
        }
        else
        {
            CalculateSway();

            MoveSway();
            if(!Input.GetKey(KeyCode.Mouse2))
            {
                TiltSway();
            }
            
        }
        
        
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
