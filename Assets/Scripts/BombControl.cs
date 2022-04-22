using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControl : ObjectControl
{
    public GameObject particle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.Play("BombExploded");

            GameObject go = Instantiate(particle, GameObject.FindGameObjectWithTag("GameRoot").transform);
            go.transform.position = this.transform.position;
            //go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(go, 2.0f);

            Destroy(this.gameObject);
        }
    }
}
