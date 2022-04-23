using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Poolable
{
    private static ObjectPool<T> instace;
    public static ObjectPool<T> Instace
    {
        get
        {
            return instace;
        }
    }

    [SerializeField]
    protected T poolObject;
    [SerializeField]
    private int allocateCount;

    protected List<T> pool = new List<T>();
    protected int additionCount = 5;

    private void Awake()
    {
        if (instace)
        {
            Destroy(gameObject);
            return;
        }
        instace = this;

        for (int i = 0; i < allocateCount; i++)
        {
            Allocate();
        }
    }

    protected virtual void Allocate()
    {
        T allocateObject = Instantiate(poolObject.gameObject, this.gameObject.transform).GetComponent<T>();
        allocateObject.Create(this);
        pool.Add(allocateObject.GetComponent<T>());
    }

    public GameObject Spawn()
    {
        if (pool.Count == 0)
        {
            for (int i = 0; i < additionCount; i++)
            {
                Allocate();
            }
        }

        Poolable obj = pool[0];
        pool.RemoveAt(0);
        obj.gameObject.SetActive(true);
        obj.Spawn();
        return obj.gameObject;
    }

    public void Despawn(Poolable obj)
    {
        obj.gameObject.SetActive(false);
        pool.Add(obj as T);
    }
}
