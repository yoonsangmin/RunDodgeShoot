using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public GameObject enemy;
    public GameObject saw;
    public GameObject powerItem;
    private int obj_count = 0;

    public void create_enemy(Vector3 obj_position, int hp)
    {
        GameObject go = GameObject.Instantiate(this.enemy) as GameObject;
        go.GetComponent<EnemyControl>().Hp = hp;
        go.transform.position = obj_position;
        this.obj_count++;
    }

    public void create_bomb(Vector3 obj_position)
    {
        GameObject go = GameObject.Instantiate(this.saw) as GameObject;
        go.transform.position = obj_position;
        this.obj_count++;
    }

    public void create_powerItem(Vector3 obj_position)
    {
        GameObject go = GameObject.Instantiate(this.powerItem) as GameObject;
        go.transform.position = obj_position;
        this.obj_count++;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
