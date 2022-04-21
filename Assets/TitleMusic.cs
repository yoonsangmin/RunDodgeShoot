using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusic : MonoBehaviour
{
    public AudioClip titleSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = titleSound;
        audioSource.volume = 0.6f;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
