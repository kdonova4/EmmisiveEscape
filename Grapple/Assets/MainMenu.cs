using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Camera menuCam;
    private OptionsMenu optionsMenu;
    private Vector3 camInitPos;
    private Quaternion camInitRot;
    public float camMoveSpeed;
    public Transform camOptionPos;
    public Quaternion camOptionRot;
    private bool isMoving;
    public Light OptionsLight;
    public Light MenuLight;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        MenuLight.gameObject.SetActive(true);
        OptionsLight.gameObject.SetActive(false);
        optionsMenu = FindObjectOfType(typeof(OptionsMenu)) as OptionsMenu;
        camInitPos = menuCam.transform.position;
        camInitRot = menuCam.transform.rotation;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clickSound;
    }
    public void PlayGame()
    {      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetIsMoving(bool val)
    {
        isMoving = val;
    }

    public void Update()
    {
        if(isMoving)
        {
            menuCam.transform.position = Vector3.Slerp(menuCam.transform.position, camOptionPos.position, Time.deltaTime * camMoveSpeed);
            menuCam.transform.rotation = Quaternion.Slerp(menuCam.transform.rotation, camOptionRot, Time.deltaTime * camMoveSpeed);
        }
        if(menuCam.transform.position  == camOptionPos.position)
        {
            isMoving = false;
        }
        
    }

    public void CamMovement()
    {
        audioSource.Play();
        MenuLight.gameObject.SetActive(false);
        OptionsLight.gameObject.SetActive(true);
        optionsMenu.SetGoingBack(false);
        isMoving = true;
    }

    public Vector3 GetInitPos()
    {
        return camInitPos;
    }

    public Quaternion GetInitRot()
    {
        return camInitRot;
    }    

    public void Quit()
    {
        Application.Quit();
        audioSource.Play();
    }
}
