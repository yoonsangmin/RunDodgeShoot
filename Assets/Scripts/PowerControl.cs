using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerControl : Poolable
{
    public GameObject powerObj;
    private MapCreator mapCreator;

    // Start is called before the first frame update
    void Start()
    {
        mapCreator = FindObjectOfType<MapCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        powerObj.transform.Rotate(new Vector3(0.2f, 1.0f, 0.2f));

        if (this.mapCreator.isDelete(this.gameObject))
        { // 카메라에게 나 안보이냐고 물어보고 안 보인다고 대답하면
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.Play("PowerUp");

            Despawn();
            // Destroy(this.gameObject);
        }
            
    }

    public override void Despawn()
    {
        (pool as PowerPool).Despawn(this);
    }
}
