using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip doorClose;
    [SerializeField] private AudioClip confirm;
    private AudioSource doorSounds;
    void Start()
    {
        doorSounds = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSmallDoor(Animator smallDoor)
    {
        smallDoor.Play("SmallOpen", 0, 0);
        
    }
    void ConfirmSound()
    {
        doorSounds.clip = confirm;
        doorSounds.Play();
    }
    void OpenSound()
    {
        
        doorSounds.clip = doorOpen;
        doorSounds.Play();
    }
    void CloseSound()
    {
        doorSounds.clip = doorClose;
        doorSounds.Play();
    }

}
