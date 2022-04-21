using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private float Bullet_speed;
    public float bullet_speed { get { return Bullet_speed; } set { Bullet_speed = value; } }

    private float range = 60.0f;

    private PlayerControl Player;
    private GameRoot gameroot;

    // 음향 관련
    public AudioClip flyingSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        this.Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.gameroot = GameObject.FindGameObjectWithTag("GameRoot").GetComponent<GameRoot>();

        // 음향 초기화
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = flyingSound;
        audioSource.volume = 0.6f;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // 나중에 풀어야 됨
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;
        velocity.x = Player.GetComponent<PlayerControl>().speed + this.Bullet_speed;   // 총알 속도 플레이어 속도와 함께 넘겨줌
        this.GetComponent<Rigidbody>().velocity = velocity;

        // 거리가 멀어지면 지워버림
        if(transform.position.x - Player.transform.position.x > range)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            gameroot.kill_enemy_score();
            Destroy(this.gameObject);
        }
    }

    // 화면에서 넘어가면 풀에 집어 넣고 액티브 상태 false
    //private void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
