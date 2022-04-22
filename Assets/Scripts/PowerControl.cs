using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerControl : ObjectControl
{
    public GameObject powerObj;

    // Start is called before the first frame update
    void Start()
    {

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
            SoundManager.Instance.Play("PowerUp");

            Destroy(this.gameObject);
        }
            
    }
}
