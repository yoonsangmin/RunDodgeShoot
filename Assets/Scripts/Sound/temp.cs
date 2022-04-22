using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp : MonoBehaviour
{
    float time = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (time > 2.0f)
        {
            SoundManager.Instance.Play("Hitted");
            Debug.Log("Àç»ý");
            time = 0.0f;
        }
        else
        {
            time += Time.deltaTime;
        }
    }
}
