using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControl : ObjectControl
{
    // 음향
    private SoundControl soundControl = null;

    public GameObject particle;

    // Start is called before the first frame update
    void Start()
    {
        // 음향 초기화
        this.soundControl = GameObject.Find("GameRoot").GetComponent<SoundControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.soundControl.BombExplosion();

            GameObject go = Instantiate(particle, GameObject.FindGameObjectWithTag("GameRoot").transform);
            go.transform.position = this.transform.position;
            //go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(go, 2.0f);

            Destroy(this.gameObject);
        }
    }
}
