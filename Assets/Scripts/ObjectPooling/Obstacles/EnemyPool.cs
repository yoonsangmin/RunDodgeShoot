using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyControl>
{
    public GameObject Spawn(Vector3 objPosition, int hp)
    {
        EnemyControl obj = Spawn().GetComponent<EnemyControl>();
        obj.Hp = hp;
        obj.transform.position = objPosition;
        obj.ChangeMaterial();
        return obj.gameObject;
    }
}
