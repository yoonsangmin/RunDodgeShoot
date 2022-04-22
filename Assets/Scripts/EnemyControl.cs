using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : ObjectControl
{
    public GameObject enemyModel;

    public int Hp = 1;

    public GameObject bodyObj;
    public GameObject leg1Obj;
    public GameObject leg2Obj;
    public GameObject leg3Obj;
    public Material[] MaterialsAccordingToHp;

    private MeshRenderer mesh = null;
    private MeshRenderer leg1mesh = null;
    private MeshRenderer leg2mesh = null;
    private MeshRenderer leg3mesh = null;
    private GameRoot gameroot = null;
    private PlayerControl playercontrol = null;

    private bool is_combo_reset = false;    // 콤보 끊었는지, 계속 끊지않게 해주는 플래그

    // Start is called before the first frame update
    void Start()
    {
        this.mesh = bodyObj.gameObject.GetComponent<MeshRenderer>();
        this.leg1mesh = leg1Obj.gameObject.GetComponent<MeshRenderer>();
        this.leg2mesh = leg2Obj.gameObject.GetComponent<MeshRenderer>();
        this.leg3mesh = leg3Obj.gameObject.GetComponent<MeshRenderer>();

        this.gameroot = GameObject.Find("GameRoot").GetComponent<GameRoot>();
        this.playercontrol = GameObject.Find("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어 뒤로 가면 콤보 끊김, 한 번만 실행 됨
        if (this.transform.position.x < playercontrol.transform.position.x && !is_combo_reset)
        {
            if (!playercontrol.is_power_upped)
            {
                is_combo_reset = true;
                this.gameroot.end_combo();
            }
        }
            

        // TODO: 수정 필요
        // 체력에 따라 머터리얼 변경
        //if (MaterialsAccordingToHp.Length >= Hp)
        //    if(mesh.material != MaterialsAccordingToHp[Hp - 1])
        //    {
        //        mesh.material = MaterialsAccordingToHp[Hp - 1];
        //        leg1mesh.material = MaterialsAccordingToHp[Hp - 1];
        //        leg2mesh.material = MaterialsAccordingToHp[Hp - 1];
        //        leg3mesh.material = MaterialsAccordingToHp[Hp - 1];
        //    }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 총알과 부딪히면 오브젝트 삭제
        if (other.gameObject.CompareTag("Bullet"))
        {
            Hp--;
            if (Hp == 0)
            {
                SoundManager.Instance.Play("EnemyKilled");
                SoundManager.Instance.Play("Hitted");
                this.gameroot.increase_combo();
                //Destroy(this.gameObject);


                enemyModel.GetComponent<Animator>().SetBool("IsDeath", true);


                mesh.material.color = new Color(mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, 0);
                leg1mesh.material.color = new Color(leg1mesh.material.color.r, leg1mesh.material.color.g, leg1mesh.material.color.b, 0);
                leg2mesh.material.color = new Color(leg2mesh.material.color.r, leg2mesh.material.color.g, leg2mesh.material.color.b, 0);
                leg3mesh.material.color = new Color(leg3mesh.material.color.r, leg3mesh.material.color.g, leg3mesh.material.color.b, 0);


                this.GetComponent<SphereCollider>().enabled = false;
                Destroy(this.gameObject, 0.2f);
            }
            else
            {
                Debug.Log("에너미 히티드");
                SoundManager.Instance.Play("Hitted");
            }
        }
    }

    //// 화면에서 넘어가면 삭제함
    //private void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
