using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCircuit : MonoBehaviour
{
    public AudioClip shortedOut;
    private AudioSource soundSource;
    public float minTime = 1f;
    public float maxTime = 5f;
    private float randomTimer;
    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
        soundSource.clip = shortedOut;
        randomTimer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        PlaySound();
    }

    void PlaySound()
    {
        randomTimer -= Time.deltaTime;

        if(randomTimer <=0)
        {
            soundSource.Play();
            randomTimer = Random.Range(minTime, maxTime);
        }
    }
}
