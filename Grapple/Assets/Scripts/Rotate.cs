using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update

    public enum States
    {
        Searching,
        TargetAquired,
        Shooting,
        Rotating,
        Disabled

    }

    public States state = States.Searching;
    public BoxCollider turretSensor;
    public Transform player;
    public Transform firePoint;
    public Transform sensorPosition;
    public Transform rightSensorPosition;
    public Transform upSensorPosition;
    public Transform leftSensorPosition;
    public Transform rightSideAnglePos;
    public Transform leftSideAnglepos;
    private Quaternion startRotation;
    private bool reverse = false;

    [Header("Audio")]
    public AudioSource turretWarning;
    public AudioSource turretIdle;
    public AudioSource turretShoot;

    [Header("RayCasts")]
    public float upwardAngle = 30f;
    private float rotateSpeed = 5f;
    public float rightAngle = 30f;
    public float leftAngle = 30f;
    [Header("Spotlight")]
    public Light turretSpotlight;
    [Header("Sensors")]
    public float sensorLength = 20;
    private bool targetAquired;
    public float turretDamage = 15f;

    public Quaternion start = Quaternion.Euler(0, 0, 0);
    public Quaternion end = Quaternion.Euler(0, 90, 0);
    public Rigidbody playerrb;
    public float switchTime = 1.5f;
    private float beepTimer = 1f;
    public float fireRate = .75f;

    public ParticleSystem turretFlash;

    void Start()
    {
        startRotation = transform.rotation;
        playerrb = player.GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {

        Sensors();
    }


    private void Sensors()
    {
        if (state != States.Disabled)
        {
            RaycastHit hitCenter;
            Vector3 sensorStartPos = sensorPosition.position;
            Vector3 rightSensorStartPos = rightSensorPosition.position;
            Vector3 upSensorStartPos = upSensorPosition.position;
            Vector3 leftSensorStartPos = leftSensorPosition.position;
            Vector3 rightSideAngle = rightSideAnglePos.position;
            Vector3 leftSideAngle = leftSideAnglepos.position;
            // center sensor
            if (Physics.Raycast(sensorStartPos, sensorPosition.transform.forward, out hitCenter, sensorLength))
            {

            }
            //Debug.DrawLine(sensorPosition.position, hitCenter.point);

            //right center sensor
            RaycastHit hitRight;
            if (Physics.Raycast(rightSensorStartPos, rightSensorPosition.transform.forward, out hitRight, sensorLength))
            {

            }
            //Debug.DrawLine(rightSensorPosition.position, hitRight.point);


            RaycastHit hitUp;
            if (Physics.Raycast(upSensorStartPos, upSensorPosition.transform.forward + Vector3.up, out hitUp, sensorLength))
            {

            }
            //Debug.DrawLine(upSensorPosition.position, hitUp.point);


            RaycastHit hitLeft;
            if (Physics.Raycast(leftSensorStartPos, leftSensorPosition.transform.forward, out hitLeft, sensorLength))
            {

            }
            //Debug.DrawLine(leftSensorPosition.position, hitLeft.point);


            RaycastHit hitLeftAngle;
            if (Physics.Raycast(leftSideAngle, Quaternion.Euler(0, leftAngle, 0) * transform.forward, out hitLeftAngle, sensorLength))
            {

            }
            //Debug.DrawLine(leftSideAnglepos.position, hitLeftAngle.point);


            RaycastHit hitRightAngle;
            if (Physics.Raycast(rightSideAngle, Quaternion.Euler(0, rightAngle, 0) * transform.forward, out hitRightAngle, sensorLength))
            {

            }
            //Debug.DrawLine(rightSideAnglePos.position, hitRightAngle.point);


            if (hitRight.rigidbody == playerrb || hitCenter.rigidbody == playerrb || hitUp.rigidbody == playerrb || hitLeft.rigidbody == playerrb || hitLeftAngle.rigidbody == playerrb || hitRightAngle.rigidbody == playerrb)
            {
                state = States.TargetAquired;
                targetAquired = true;
                turretSpotlight.color = Color.red;
                Vector3 direction = player.position - transform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
                switchTime = 1.5f;
                if (!turretWarning.isPlaying)
                {
                    turretWarning.Play();
                }


            }
            else
            {
                if (state != States.Disabled)
                {
                    state = States.Searching;
                }

                targetAquired = false;


            }
        }



        
            
            
    }

   

    void Rotating()
    {

        if (Quaternion.Angle(end, transform.rotation) < 5)
        {
            reverse = true;
            Debug.Log(reverse);
        }
        else if (Quaternion.Angle(startRotation, transform.rotation) < 5)
        {
            reverse = false;
        }

        if (state == States.Rotating)
        {
            fireRate = .75f;
            if (reverse == false)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, end, Time.deltaTime * .5f);
            }
            else if (reverse)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * .5f);
            }

        }
    }

    void Timer()
    {
        
        switchTime -= Time.deltaTime;

        if(switchTime < 0)
        {
            state = States.Rotating;
            turretSpotlight.color = Color.yellow;
        }

    }

    void BeepTimer()
    {
         beepTimer -= Time.deltaTime;
        if (beepTimer < 0)
        {
            if(!turretIdle.isPlaying)
            {
                turretIdle.Play();
            }

          beepTimer = 1f;
        }
    }

    void FireRate()
    {
        if(state == States.TargetAquired)
        {
            fireRate -= Time.deltaTime;
        }
    }
    

    void Shoot()
    {
        if(state == States.TargetAquired && fireRate < 0 && state != States.Disabled)
        {

            RaycastHit hit;
            if (Physics.Raycast(firePoint.transform.position, transform.forward, out hit, 1000))
            {
                Debug.Log(hit.transform.name);
                fireRate = .75f;
                

            }
            turretShoot.Play();
            turretFlash.Play();
            Health dealDamage = hit.transform.GetComponent<Health>();
            if (dealDamage != null)
            {
                dealDamage.TakeDamage(turretDamage);
            }

        }

        
    }
    public void SetDead()
    {
        state = States.Disabled;
    }
    void Update()
    {
        
        if(state == States.Disabled)
        {
            turretSpotlight.gameObject.SetActive(false);
        }
        if(state != States.Disabled)
        {
            Shoot();
            if (state == States.TargetAquired)
            {
                FireRate();
            }

            if (state == States.Searching)
            {
                Timer();
            }

            if (state == States.Rotating)
            {
                BeepTimer();
            }
            Rotating();
        }
        

    }



}
