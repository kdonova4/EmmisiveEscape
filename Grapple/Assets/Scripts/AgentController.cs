using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    private enum States
    {
        Roaming,
        TargetAquired,
        Searching,
        Disabled
        
    }
    States state;
    public Camera cam;
    private float waitTime;
    public float startWaitTime;
    public NavMeshAgent agent;
    public Transform[] moveSpots;
    private int randomSpot;
    public Material sensorEye;
    public Transform sensorForward;
    public Transform sensorRight;
    public Transform sensorLeft;
    public Transform sensorBottom;
    public float sensorLength = 20;
    public Transform player;
    public float rightAngle = 30f;
    public float rightAngle2 = 30f;
    public float bottomrightAngle2 = 30f;
    public float leftAngle = 30f;
    public float leftAngle2 = 30f;
    public float bottomleftAngle2 = 30f;
    public float switchTime = 1.5f;
    public float scanTime = 2.5f;
    private Vector3 playerLastPosition;
    private bool scanning;
    public float fireRate = .35f;
    public float damage = 10f;
    public AudioSource agentIdleSound;
    public AudioSource agentSpeak;
    public ParticleSystem roboFlash;
    public AudioSource roboShoot;
    public Rigidbody playerrb;
    public float angle = 20;
    private Quaternion scanPoint1 = Quaternion.Euler(0, 90, 0);
    private Quaternion scanPoint2 = Quaternion.Euler(0, -90, 0);
    public float rotateTimer = 5.0f;
    private bool isRotating = false;
    private Rigidbody rgbd;
    [SerializeField] private Transform gun;
    private SimpleShoot gunForce;
    private bool forceApply = false;
    public Light spotlight;
    public ParticleSystem distortion;
    // Update is called once per frame
    void Start()
    {
        rgbd = GetComponent<Rigidbody>();
        gunForce = gun.GetComponent<SimpleShoot>();
        state = States.Roaming;
        playerrb = player.GetComponent<Rigidbody>();
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);
    }

    void FixedUpdate()
    {

        Sensors();
    }

    void Sensors()
    {
        if (state != States.Disabled)
        {

        
                Vector3 forwardStartPos = sensorForward.position;
            Vector3 rightStartPos = sensorRight.position;
            Vector3 leftStartPos = sensorLeft.position;
            Vector3 bottomStartPos = sensorBottom.position;

            RaycastHit hitBottom;
            if (Physics.Raycast(bottomStartPos, sensorBottom.transform.forward, out hitBottom, sensorLength))
            {

            }
            //Debug.DrawLine(sensorBottom.position, hitBottom.point);

            RaycastHit hitBottomLeft;
            if (Physics.Raycast(bottomStartPos, Quaternion.Euler(0, bottomleftAngle2, 0) * transform.forward, out hitBottomLeft, sensorLength))
            {

            }
            //Debug.DrawLine(sensorBottom.position, hitBottomLeft.point);

            RaycastHit hitBottomRight;
            if (Physics.Raycast(bottomStartPos, Quaternion.Euler(0, bottomrightAngle2, 0) * transform.forward, out hitBottomRight, sensorLength))
            {

            }
            //Debug.DrawLine(sensorBottom.position, hitBottomRight.point);

            RaycastHit hitForward;
            if (Physics.Raycast(forwardStartPos, sensorForward.transform.forward, out hitForward, sensorLength))
            {

            }
            //Debug.DrawLine(sensorForward.position, hitForward.point);

            //right center sensor
            RaycastHit hitRight;
            if (Physics.Raycast(rightStartPos, Quaternion.Euler(0, rightAngle, 0) * transform.forward, out hitRight, sensorLength))
            {

            }
            //Debug.DrawLine(sensorRight.position, hitRight.point);

            RaycastHit hitRight2;
            if (Physics.Raycast(rightStartPos, Quaternion.Euler(0, rightAngle2, 0) * transform.forward, out hitRight2, sensorLength))
            {

            }
            //Debug.DrawLine(sensorRight.position, hitRight2.point);

            RaycastHit hitLeft;
            if (Physics.Raycast(leftStartPos, Quaternion.Euler(0, leftAngle, 0) * transform.forward, out hitLeft, sensorLength))
            {

            }
            //Debug.DrawLine(sensorLeft.position, hitLeft.point);

            RaycastHit hitLeft2;
            if (Physics.Raycast(leftStartPos, Quaternion.Euler(0, leftAngle2, 0) * transform.forward, out hitLeft2, sensorLength))
            {

            }
            //Debug.DrawLine(sensorLeft.position, hitLeft2.point);

            RaycastHit hitDown;
            if (Physics.Raycast(forwardStartPos, sensorForward.transform.forward - Vector3.up - new Vector3(0,angle,0), out hitDown, sensorLength))
            {

            }
            //Debug.DrawLine(sensorForward.position, hitDown.point);


            if (hitRight.rigidbody == playerrb || hitRight2.rigidbody == playerrb || hitForward.rigidbody == playerrb || hitLeft.rigidbody == playerrb || hitLeft2.rigidbody == playerrb || hitDown.rigidbody == playerrb || hitBottomRight.rigidbody == playerrb || hitBottomLeft.rigidbody == playerrb || hitBottom.rigidbody == playerrb)
            {
                state = States.TargetAquired;
                agent.SetDestination(player.position);
                //agent.Stop();
                playerLastPosition = player.position;
                switchTime = 1.5f;
                agent.stoppingDistance = 10f;
                Vector3 direction = player.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
            
            }
            else
            {
            
                if (state != States.Roaming)
                {
                    state = States.Searching;
                
                }
            
            
            }
        }


    }
    void Rotate()
    {
            
       if(isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * Quaternion.Euler(0, -90, 0), 1f * Time.deltaTime);
        }
            
    }
    void RotateTimer()
    {
        rotateTimer -= Time.deltaTime;

        if (rotateTimer < 0)
        {
            isRotating = false;
            scanning = true;
        }
    }

    void Timer()
    {

        switchTime -= Time.deltaTime;

        if (switchTime < 0 )
        {
            agent.Resume();
            agent.SetDestination(playerLastPosition);
            agent.stoppingDistance = 1f;
            if (Vector3.Distance(transform.position, playerLastPosition) <= 5)
            {
                RotateTimer();
                Rotate();
                if(!isRotating)
                {
                    
                    isRotating = true;
                    rotateTimer = 5.0f;
                }

            }
        }

    }
    void ScanTimer()
    {
        scanTime -= Time.deltaTime;

        if(scanTime < 0)
        {
            scanning = false;
            state = States.Roaming;
        }
    }

    void ForceApply()
    {
        Vector3 raycastDir = gun.transform.position - transform.position;
        if (forceApply == false)
        {
            rgbd.AddForce(-raycastDir * gunForce.GunForce());
            forceApply = true;
        }
    }
    void Update()
    {
        if(state == States.Disabled)
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            rgbd.useGravity = true;
            rgbd.isKinematic = false;
            ForceApply();
            spotlight.gameObject.SetActive(false);
            distortion.gameObject.SetActive(false);
            agentIdleSound.Stop();

        }
        if (state != States.Disabled)
        {
            if (!agentIdleSound.isPlaying)
            {
                agentIdleSound.Play();
            }

            Roam();
            //Debug.Log(state);
            if (state == States.Searching)
            {

                Timer();
            }
            if (scanning == true)
            {
                ScanTimer();
            }
            if (scanning == false)
            {
                scanTime = 2.5f;
            }
            Shoot();
            if (state == States.TargetAquired)
            {
                FireRate();
            }
        }
            
    }
    void FireRate()
    {
        if (state == States.TargetAquired)
        {
            fireRate -= Time.deltaTime;
        }
    }


    void Shoot()
    {
        if (state == States.TargetAquired && fireRate < 0)
        {
            //Debug.Log("SHOOTING");
            RaycastHit hit;
            if (Physics.Raycast(sensorForward.transform.position, transform.forward, out hit, 1000))
            {
                //Debug.Log(hit.transform.name);
                fireRate = .35f;


            }
            roboFlash.Play();
            roboShoot.Play();
            
            Health dealDamage = hit.transform.GetComponent<Health>();
            if (dealDamage != null)
            {
                dealDamage.TakeDamage(damage);
            }

        }


    }
    public void SetDead()
    {
        state = States.Disabled;
    }
    void Roam()
    {
        if(state == States.Roaming)
        {
            agent.stoppingDistance = .1f;

            agent.SetDestination(moveSpots[randomSpot].position);
            
            
            if (Vector3.Distance(transform.position, moveSpots[randomSpot].position) <= 2)
            {
                if (waitTime <= 0)
                {
                    if(!agentSpeak.isPlaying)
                    {
                        agentSpeak.Play();
                    }
                    
                    randomSpot = Random.Range(0, moveSpots.Length);
                    waitTime = startWaitTime;
                }
                else
                {

                    waitTime -= Time.deltaTime;
                }
            }
            
            
        }

    }
}
