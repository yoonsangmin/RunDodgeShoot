using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerControl : ObjectControl
{
    // 음향
    private SoundControl soundControl = null;
    public GameObject powerObj;

    // Start is called before the first frame update
    void Start()
    {
        // 음향 초기화
        this.soundControl = GameObject.Find("GameRoot").GetComponent<SoundControl>();
    }

    // Update is called once per frame
    void Update()
    {
        powerObj.transform.Rotate(new Vector3(0.2f, 1.0f, 0.2f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.soundControl.PowerUp();

            Destroy(this.gameObject);
        }
            
    }
}
