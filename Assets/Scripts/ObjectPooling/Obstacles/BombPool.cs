using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPool : ObjectPool<BombControl>
{
    public GameObject Spawn(Vector3 objPosition)
    {
        BombControl obj = Spawn().GetComponent<BombControl>();
        obj.transform.position = objPosition;
        return obj.gameObject;
    }
}
