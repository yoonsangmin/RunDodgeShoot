using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{

    // 에너미 음향
    public AudioClip enemyKilledSound;
    private AudioSource enemyKiiledAudioSource;
    public AudioClip enemyHittedSound;
    private AudioSource enemyHittedAudioSource;

    // 파워 업 음향
    public AudioClip powerUpSound;
    private AudioSource powerUpAudioSource;

    // 폭탄 음향
    public AudioClip bombSound;
    private AudioSource bombAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        // 음향 초기화
        enemyKiiledAudioSource = gameObject.AddComponent<AudioSource>();
        enemyHittedAudioSource = gameObject.AddComponent<AudioSource>();
        powerUpAudioSource = gameObject.AddComponent<AudioSource>();
        bombAudioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Killed()
    {
        enemyKiiledAudioSource.clip = enemyKilledSound;
        enemyKiiledAudioSource.volume = 0.8f;
        enemyKiiledAudioSource.Play();
    }

    public void Hitted()
    {
        enemyHittedAudioSource.clip = enemyHittedSound;
        enemyHittedAudioSource.volume = 0.8f;
        enemyHittedAudioSource.Play();
    }

    public void PowerUp()
    {
        powerUpAudioSource.clip = powerUpSound;
        powerUpAudioSource.volume = 0.8f;
        powerUpAudioSource.Play();
    }

    public void BombExplosion()
    {
        bombAudioSource.clip = bombSound;
        bombAudioSource.volume = 0.8f;
        bombAudioSource.Play();
    }
}
