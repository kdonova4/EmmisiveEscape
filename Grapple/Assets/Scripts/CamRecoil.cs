using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRecoil : MonoBehaviour
{

    [Header("Recoil Settings:")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;
    [Space()]

    [Header("HipFire:")]
    public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);
    [Space()]

    [Header("Aiming:")]
    public Vector3 recoilRotaionAiming = new Vector3(0.5f, 0.5f, 1.5f);
    public Vector3 HeadBobbing = new Vector3(0.5f, 0.5f, 1.5f);
    [Space()]

    [Header("State")]
    public bool aiming;

    private Vector3 currentRotation;
    private Vector3 Rot;
    public SimpleShoot simp;
    private int cameraAmmo;
    // Start is called before the first frame update
    void Start()
    {
        cameraAmmo = simp.currentAmmo;
        
    }


    void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(Rot);
    }
    public void Fire()
    {
       
        Debug.Log(cameraAmmo);
        currentRotation += new Vector3(-recoilRotaionAiming.x, Random.Range(-recoilRotaionAiming.y, recoilRotaionAiming.y), Random.Range(-recoilRotaionAiming.z, recoilRotaionAiming.z));
    }
    public void HeadBob()
    {

        
        currentRotation += new Vector3(-2.5f,0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(simp.fullyLoaded == true)
        {
            cameraAmmo = simp.currentAmmo;
        }
        if (Input.GetMouseButtonDown(0) && cameraAmmo > 0 && simp.IsAnimating() && Input.GetMouseButton(1))
        {
            Fire();
            cameraAmmo--;
        }
        
    }
}
