using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public Component pool;

    public virtual void Create(Component pool)
    {
        this.pool = pool;
        gameObject.SetActive(false);
    }

    public virtual void Spawn() { }

    public abstract void Despawn();
}