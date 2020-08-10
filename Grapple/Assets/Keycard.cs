using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Keycard : MonoBehaviour
{

    private Material gateAccess;
    private Material keyAccess;
    public GameObject access;
    public GameObject keycard;
    public float range = 10;
    public Camera cam;
    public float disableDelay = 1f;
    private bool gateIsOpen = false;
    public AudioSource doorOpen;
    public AudioSource accessGranted;
    public AudioSource accessDenied;
    public Animator door;
    Color color;
    public PlayerMovement keycards;
    private Renderer accessRenderer;
    private Renderer keycardRenderer;
    public GameObject correctKeycard;
    public GameObject ck;
    RaycastHit hit;
    public Color cardColor;
    private OcclusionPortal doorOcc;
    public Transform portal;
    // Start is called before the first frame update
    void Start()
    {
        doorOcc = portal.GetComponent<OcclusionPortal>();
        ck = correctKeycard;
        keycards = FindObjectOfType(typeof(PlayerMovement)) as PlayerMovement;
        door = GetComponent<Animator>();
        accessRenderer = access.gameObject.GetComponent<Renderer>();
        gateIsOpen = false;
        
        gateAccess = access.gameObject.GetComponent<Renderer>().material;
        keycardRenderer = keycard.gameObject.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (keycard.active)
        {
            KeycardDisable();
        }

        
    }

    public void OpenDoor(Animator dooor)
    {
        if(gateIsOpen == false)
        {
            if(keycards.CorrectKeycard(correctKeycard))
            {
                dooor.Play("Open", 0, 0);
                doorOpen.Play();
                keycardRenderer.material.SetColor("_EmissiveColor", cardColor);
                keycard.SetActive(true);
                gateIsOpen = true;
                accessGranted.Play();
                accessRenderer.material.SetColor("_EmissiveColor", Color.green);
                doorOcc.open = true;
                gateIsOpen = true;
            }
            if (!keycards.CorrectKeycard(correctKeycard))
            {
                
                accessDenied.Play();
                
            }

        }
        
    }



    private void KeycardDisable()
    {
        disableDelay -= Time.deltaTime;

        if(disableDelay < 0)
        {
            disableDelay = 1f;
            keycard.SetActive(false);
        }
    }
}
