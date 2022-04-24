using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombControl : Poolable
{
    public GameObject effectPrefab;
    private MapCreator mapCreator;

    // Start is called before the first frame update
    void Start()
    {
        mapCreator = FindObjectOfType<MapCreator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.mapCreator.isDelete(this.gameObject))
        { // 카메라에게 나 안보이냐고 물어보고 안 보인다고 대답하면
            Despawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.Play("BombExploded");

            effectPrefab.SetActive(true);

            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }

            //GameObject go = Instantiate(particle, GameObject.FindGameObjectWithTag("GameRoot").transform);
            //go.transform.position = this.transform.position;
            //go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            // Destroy(go, 2.0f);

            // Destroy(this.gameObject, 2.0f);
            Invoke("Despawn", 2.0f);
        }
    }

    public override void Despawn()
    {
        CancelInvoke("Despawn");
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop();
        }

        effectPrefab.SetActive(false);

        (pool as BombPool).Despawn(this);
    }
}
