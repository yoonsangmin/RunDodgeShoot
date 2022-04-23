using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPool : ObjectPool<PowerControl>
{
    public GameObject Spawn(Vector3 objPosition)
    {
        PowerControl obj = Spawn().GetComponent<PowerControl>();
        obj.transform.position = objPosition;
        return obj.gameObject;
    }
}
