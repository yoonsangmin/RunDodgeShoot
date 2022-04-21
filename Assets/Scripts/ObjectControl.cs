using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 화면에서 넘어가면 삭제함
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
