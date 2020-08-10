using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleShoot : MonoBehaviour
{
    public Animator gun;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject magPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    private Rigidbody rb;
    private float state = 0.0f;
    private float target_state = 0.0f;
    public float shotPower = 100f;
    const float kRecoilSpringStrength = 800.0f;
    const float kRecoilSpringDamping = 0.000001f;
    public AudioSource m_shootingSound;
    public AudioSource m_emptySound;
    public AudioSource m_magIn;
    public AudioSource m_magOut;
    public AudioSource m_slideReleased;
    public AudioSource m_silencedShot;
    private Rotate turret;
    private Quaternion q = Quaternion.Euler(-360,0,0);
    private Vector3 initialPosition;
    private Quaternion initialRotation = Quaternion.Euler (-9,0,0);

    public float damage = 100f;
    public Ammo ammo;
    public float impactForce = 30f;
    public GameObject impactEffect;
    public ParticleSystem mFlash;
    public ParticleSystem barrelSmoke;

    public Text ammoDisplay;
    public Text maxAmmoDisplay;

    [Header("Recoil & Aiming")]
    public Transform hipPosition;
    public Transform aimPosition;

    public Vector3 aimCoordinates = new Vector3(0.692f, -0.32f, 0.13f);
    public Quaternion rotaationCoord = Quaternion.Euler(18.91f, -17.8f, 0f);
    public Vector3 reloadPosition = new Vector3(2.5f,-0.22f,0.852f);
    public Quaternion reloadRotation = Quaternion.Euler(-7.05f, -38.82f, 0f);

    [Header("Transforms")]
    public Transform slidePosition;
    public Transform slideLockPosition;
    public Transform firePoint;
    public Transform magLoadedPosition;
    public Transform magUnloadedPosition;
    public Transform magPosition;
    public Transform mag;
    public Transform magUnloaded;
    public Rigidbody rbmag;
    [Space()]


    public GameObject silencer;
    public GameObject flashlight;
    public float weaknessMod = 100;

    //Recoil
    public Transform recoilPosition;
    public Transform rotationPoint;

    //Speed
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 98f;

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;

    public Vector3 RecoilRotation = new Vector3(10, 5, 7);
    public Vector3 RecoilKickback = new Vector3(0.015f, 0f, -0.2f);

    public Vector3 RecoilRotationAim = new Vector3(10, 4, 6);
    public Vector3 REcoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;
    private Vector3 velocity = Vector3.zero;
    private bool isAnimating;
    public bool aiming;
    private int shotsTaken;
    private bool slidereleased;
    private int bulletsMag = 7;
    public int maxAmmo = 7;
    public int currentAmmo = 7;
    private bool lightOn = false;
    private bool silencerOn = false;
    private bool reloadState;
    public bool fullyLoaded;
    [Space()]
    [Header("Flashlight")]
    public AudioSource flashlightOn;
    public Transform flashlightHolster;
    public Transform flashlightHold;
    public Quaternion flashlightInitialRotation;
    public Transform flashlightAim;
    private Rotate rot;
    private AgentController agent;

    private Vector3 magInitPos;

    private bool magOut;
    private bool magIn;

    void FixedUpdate()
    {
        // Gun recoil
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, transform.localPosition, positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, rotationalRecoil, rotationalRecoilSpeed * Time.deltaTime);
        rotationPoint.localRotation = Quaternion.Euler(Rot);
    }

    void Start()
    {
           
        shotsTaken = 0;
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        flashlightInitialRotation = transform.localRotation;
        gun = GetComponent<Animator>();
        if (barrelLocation == null)
            barrelLocation = transform;
        rb = GetComponent<Rigidbody>();
        magInitPos = mag.transform.localPosition;
    }

   

    void LateUpdate()
    {
        if (currentAmmo == 0) 
        {
            slidePosition.localPosition = Vector3.Lerp(slidePosition.localPosition, slideLockPosition.localPosition, 40);
            slidereleased = true;
            
            
        }
        

        

    }

    void Hold()
    {
        if (!Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.Mouse2))
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, reloadPosition, 10f * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, reloadRotation, 10f * Time.deltaTime);
            }

        }
    }

    void ShowMag()
    {
        if (!Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.Mouse2) && Input.GetKey(KeyCode.E))
            {
                
                mag.transform.localPosition = Vector3.Lerp(mag.transform.localPosition, magUnloadedPosition.localPosition, 6f * Time.deltaTime);
                ammoDisplay.gameObject.SetActive(true);
                

            }
            if(!Input.GetKey(KeyCode.E))
            {
                mag.transform.localPosition = Vector3.Lerp(mag.transform.localPosition, magInitPos, 20f * Time.deltaTime);
                ammoDisplay.gameObject.SetActive(false);
                

            }

            if( Input.GetKeyUp(KeyCode.E) && Input.GetKey(KeyCode.Mouse2))
            {
                m_magOut.Stop();
               m_magIn.Play();
            }else if(Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.Mouse2))
            {
                m_magIn.Stop();
               m_magOut.Play();
            }

        }
    }

    void ReleaseMag()
    {

    }
    
    void Update()
    {
        Hold();
        ShowMag();

        if (Input.GetKey(KeyCode.Mouse2))
        {
            reloadState = true;
        }
        else
        {
            reloadState = false;
        }
        

        ammoDisplay.text = currentAmmo.ToString();
        maxAmmoDisplay.text = maxAmmo.ToString();
        if(!reloadState)
        {
            if (gun.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Input.GetMouseButtonDown(0) && currentAmmo > 0 && !Input.GetKey(KeyCode.Mouse2) && Input.GetMouseButton(1))
            {
                gun.Play("Shooting", 0, 0);
                

                Fire();

            }
            if (Input.GetMouseButtonDown(0) && currentAmmo == 0)
            {
                m_emptySound.Play();
            }



            Aim();
            
            if(Input.GetKeyDown(KeyCode.F))
            {
                SilencerOn();
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                SilencerOff();
            }

           // transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, 18f * Time.deltaTime);
            m_shootingSound = GetComponent<AudioSource>();
        }

        Reload();
        FlashlightAnimate();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            
            flashlight.active = !flashlight.active;
            flashlightOn.Play();
        }
        
            

    }

    void FlashlightAnimate()
    {
        if (flashlight.active && Input.GetKeyDown(KeyCode.Tab))
        {
            
            flashlight.transform.localPosition = Vector3.Slerp(flashlight.transform.localPosition, flashlightHolster.localPosition, 10f * Time.deltaTime);
            
        }
        else if(flashlight.active && Input.GetKeyDown(KeyCode.Tab))
        {
            
            flashlight.transform.localPosition = Vector3.Slerp(flashlight.transform.localPosition, flashlightHold.localPosition, 10f * Time.deltaTime);
            
        }
    }

    void SilencerOn()
    {
        silencer.SetActive(true);
        silencerOn = true;
    }
    void SilencerOff()
    {
        silencer.SetActive(false);
        silencerOn = false;
    }
    void setReloadStateTrue()
    {
        reloadState = true;
    }
    void setReloadStateTFalse()
    {
        reloadState = false;
    }
    public bool IsAnimating()
    {
        if(gun.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !reloadState)
        {
           return isAnimating = true;
        }
        else
        {
            return isAnimating = false;
        }
    }
    public void Reload()
    {

        if (maxAmmo <= 0) return;


        if (Input.GetKey(KeyCode.R) && gun.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && Input.GetKey(KeyCode.Mouse2) && reloadState && currentAmmo < 7 && !Input.GetKey(KeyCode.Mouse1) && mag.transform.localPosition == magInitPos)
        {
            

            gun.Play("Reload", 0, 0);
            
            fullyLoaded = true;
        }

    }

    void ReloadAmmo()
    {

        int loading = bulletsMag - currentAmmo;
        int bulletsDedeuct = (maxAmmo >= loading) ? loading : maxAmmo;

        maxAmmo -= bulletsDedeuct;
        currentAmmo += bulletsDedeuct;


    }

    void Aim()
    {
        if(Input.GetMouseButton(1) && currentAmmo > 0 && !Input.GetKey(KeyCode.Mouse2) && !reloadState) 
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimCoordinates, 10f * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotaationCoord, 10f * Time.deltaTime);
            flashlight.transform.localPosition = Vector3.Slerp(flashlight.transform.localPosition, flashlightAim.localPosition, 10f * Time.deltaTime);

        }
        else if (Input.GetMouseButtonUp(0))
        {

        }
    }

    public void Recoil()
    {
        rotationalRecoil += new Vector3(Random.Range(-RecoilRotation.x - 40, -RecoilRotation.x - 100), Random.Range(-RecoilRotation.y, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
        positionalRecoil += new Vector3(Random.Range(-RecoilKickback.x, RecoilKickback.x), Random.Range(-RecoilKickback.y, RecoilKickback.y), RecoilKickback.z);
    }
    public void gunBobbing()
    {
        rotationalRecoil += new Vector3(15f, 0, 0);
        //positionalRecoil += new Vector3(Random.Range(-RecoilKickback.x, RecoilKickback.x), Random.Range(-RecoilKickback.y, RecoilKickback.y), RecoilKickback.z);
    }
    
    public void SprintGunBobbing()
    {
        rotationalRecoil += new Vector3(30f, 0, 0);
        //positionalRecoil += new Vector3(Random.Range(-RecoilKickback.x, RecoilKickback.x), Random.Range(-RecoilKickback.y, RecoilKickback.y), RecoilKickback.z);
    }
    public float GunForce()
    {
        return impactForce;
    }
    public void AddTotalAmmo(int ammo)
    {
       maxAmmo += ammo;
    }
    void Fire()
    {
        if(!silencerOn)
        {
            m_shootingSound.Play();
            mFlash.Play();
            barrelSmoke.Play();

        }
        else
        {
            m_silencedShot.Play();
        }
        
        
        fullyLoaded = false;
        Debug.Log(fullyLoaded);
        shotsTaken = 0;
        currentAmmo--;
        Recoil();
        RaycastHit hit;
        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, 1000))
        {
            Debug.Log(hit.transform.name);

            Debug.DrawLine(firePoint.position, hit.point);

        }

        GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO, 2f);
        target target = hit.transform.GetComponent<target>();
        if(target.GetComponent<Collider>().tag == "Turret")
        {
            rot = hit.transform.GetComponent<Rotate>();
        }
        if(target.GetComponent<Collider>().tag == "Agent")
        {
            agent = hit.transform.GetComponent<AgentController>();
        }
        
        

        if (target != null)
        {

            target.TakeDamage(damage);
        }
        if(target.health <=0 && target.tag == "Turret")
        {
            rot.SetDead();
        }
        if (target.health <= 0 && target.tag == "Agent")
        {
            agent.SetDead();
            Debug.Log("AGENT DOOOOOOOWN");
        }
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * impactForce);
        }

        
        
    }
    void MagOutSound()
    {
        m_magOut.Play();
    }
    void MagInSound()
    {
        m_magIn.Play();
    }
    void SlideReleasedSound()
    {
        if(slidereleased)
        {
            m_slideReleased.Play();
            slidereleased = false;
        }
        
    }

    void Shoot()
    {

       // GameObject bullet;
         //bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        //bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        
        
       // GameObject tempFlash;
       
       // tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
        
        // Destroy(tempFlash, 0.5f);
        //  Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation).GetComponent<Rigidbody>().AddForce(casingExitLocation.right * 100f);





    }

    void CasingRelease()
    {
         GameObject casing;
        casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
    }


}
