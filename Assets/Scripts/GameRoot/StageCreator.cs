using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCreator : MonoBehaviour
{
    public LayerMask layerMask;     // 땅 레이어

    public float OBJ_WIDTH_MIN = 5.0f;   // 오브젝트 생성 폭 최솟값
    public float OBJ_WIDTH_MAX = 12.0f;   // 오브젝트 생성 폭 최댓값

    public float OBJ_WIDTH = 10.0f; // 오브젝트 생성 폭

    public float OBJ_INITIAL_POS = 20f;   // 오브젝트 생성 오프셋
    public float OBJ_OFFSET = 35f;   // 오브젝트 생성 오프셋
    //public static int BLOCK_NUM_IN_SCREEN = 28; // 화면 내에 들어가

    public int OBJ_NUMBER = 0;           // 현재 생성한 오브젝트 개수
    public int OBJ_NUMBER_TO_MAKE = 5;   // 오브젝트 연속으로 나올 개수

    public int OBJ_NUMBER_MIN = 2;   // 오브젝트 연속으로 나올 개수 최솟값
    public int OBJ_NUMBER_MAX = 5;  // 오브젝트 연속으로 나올 개수 최댓값

    private float OBJ_ZPOS_TO_MAKE;  // 오브젝트 생성될 Z좌표
    private float OBJ_PREV_ZPOS;  // 오브젝트 이전 Z좌표

    public float ObjZDamp = 0.5f;

    private MapCreator mapCreator = null;

    public enum Type
    { // Object의 각종 상태를 나타내는 자료형 (*열거체)
        NONE = -1, // 상태정보 없음.
        ENEMY = 0, // 몬스터
        BOMB, // 톱
        POWER, // 파워업 아이템.
        NUM, // 상태가 몇 종류 있는지 보여준다(=3).
    };

    public Type last_type = Type.NONE;
    public Type type = Type.NONE; // 현재 생성해야 하는 오브젝트

    public float bomb_spawn_rate = 30;        // 톱날이 나올 확률 100 - 30 = 몬스터가 나올 확률

    // 총합 100으로 해도 될 듯
    public float enemy_hp1_spawn_rate = 80;  // 몬스터 체력별로 나올 확률
    public float enemy_hp2_spawn_rate = 20;
    public float enemy_hp3_spawn_rate = 0;

    // 총합 100으로 해도 될 듯
    public float bomb_1_spawn_rate = 80;  // 폭탄 나올 확률
    public float bomb_2_spawn_rate = 20;
    public float bomb_3_spawn_rate = 0;


    public int power_up_spawn_distance = 100; // 파워업 아이템이 나올 간격
    public int next_power_up_spawn_position; // 다음 파워업 위치

    private Vector3 last_obj_position; // 마지막에 생성한 오브젝트의 위치.
    private PlayerControl player = null; // 씬상의 Player를 보관.
    private GameRoot game_root; // 각종 게임 정보를 받아옴

    // 블럭 생성 위치
    private float obj_generate_x;

    // 레벨 컨트롤
    private LevelControl level_control = null;
    public TextAsset level_data_text = null;

    public float update_distance = 100; // 업데이트 거리

    private bool isObjectOnLift = false;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.game_root = this.gameObject.GetComponent<GameRoot>();
        this.level_control = this.gameObject.GetComponent<LevelControl>();
        this.level_control.LoadLevelData(this.level_data_text);

        this.mapCreator = this.gameObject.GetComponent<MapCreator>();

        update_level(); // 최초 레벨 값 적용해 줌

        obj_generate_x = this.player.transform.position.x + (float)OBJ_INITIAL_POS; //  오브젝트 거리 0으로 만들고 오브젝트 오프셋 까지 생성함
        type = Type.ENEMY;

        // 오브젝트 위치 랜덤으로 정함
        //int rand = Random.Range(0, 3);
        //if (rand == 0) OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
        //else if (rand == 1) OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
        //else OBJ_ZPOS_TO_MAKE = LANE3_Zpos;

        OBJ_ZPOS_TO_MAKE = Random.Range(game_root.rightBoundary + ObjZDamp, game_root.leftBoundary - ObjZDamp);

        while (obj_generate_x < this.player.transform.position.x + (float)OBJ_OFFSET)
        {
            if (OBJ_NUMBER == OBJ_NUMBER_TO_MAKE)
            {
                // 만든 오브젝트 초기화
                OBJ_NUMBER = 0;

                // 만들 오브젝트에 따라서 몇개 연속으로 나올지 선택해 줌
                select_obj_number();

                //rand = Random.Range(0, 2);
                //if (OBJ_ZPOS_TO_MAKE == LANE1_Zpos)
                //{
                //    if (rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE3_Zpos;
                //}
                //else if (OBJ_ZPOS_TO_MAKE == LANE2_Zpos)
                //{
                //    if (rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE3_Zpos;
                //}
                //else if (OBJ_ZPOS_TO_MAKE == LANE3_Zpos)
                //{
                //    if (rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
                //}

                OBJ_ZPOS_TO_MAKE = Random.Range(game_root.rightBoundary + ObjZDamp, game_root.leftBoundary - ObjZDamp);
            }


            spawn_obj();    // 오브젝트 - 몬스터 하나 생성

            // 다음 범위 랜덤으로 정해 줌
            OBJ_WIDTH = Random.Range(OBJ_WIDTH_MIN, OBJ_WIDTH_MAX);
            obj_generate_x += OBJ_WIDTH;
        }
        
        // 다음 파워 업 포지션 거리 정해 줌
        next_power_up_spawn_position = power_up_spawn_distance;
    }

    private void spawn_obj()
    {
        Vector3 spawn_position = Vector3.zero; // 이제부터 만들 블록의 위치.

        spawn_position.x = obj_generate_x;
        // 블록의 Y위치는 앞에서 정한 Floor 높이로.
        spawn_position.z = OBJ_ZPOS_TO_MAKE;
        spawn_position.y = 1.0f;

        switch (this.type)
        {
            case Type.ENEMY:
                // 몬스터일 경우
                // 총 이동거리에 따라 오브젝트 hp 확률이 달라짐
                int hp = 1;
                float sum_rate = enemy_hp1_spawn_rate + enemy_hp2_spawn_rate + enemy_hp3_spawn_rate;
                float param = Random.Range(1, sum_rate);

                if (param <= enemy_hp1_spawn_rate) hp = 1;
                else if (param <= enemy_hp1_spawn_rate + enemy_hp2_spawn_rate) hp = 2;
                else hp = 3;

                // this.obj_creator.create_enemy(spawn_position, hp); // 이제까지의 코드에서 설정한 block_position을 건네준다.
                (EnemyPool.Instance as EnemyPool).Spawn(spawn_position, hp);
                break;
            case Type.NONE:
                // 아무것도 생성 안함
                break;
            case Type.BOMB:
                // 폭탄 생성함
                //sum_rate = bomb_1_spawn_rate + bomb_2_spawn_rate + bomb_3_spawn_rate;
                //param = Random.Range(1, sum_rate);

                (BombPool.Instance as BombPool).Spawn(spawn_position);

                //if (param <= bomb_1_spawn_rate)
                //{
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);
                //}
                //else if (param <= bomb_1_spawn_rate + bomb_2_spawn_rate)
                //{
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);

                //    // 두번 째 폭탄 생성
                //    float next1 = 0, next2 = 0;

                //    for (int i = 0; i < 3; i++)
                //    {
                //        float temp = 0;
                //        if (i == 0) temp = LANE1_Zpos;
                //        else if (i == 1) temp = LANE2_Zpos;
                //        else if (i == 2) temp = LANE3_Zpos;

                //        if (next1 == 0)
                //        {
                //            if (temp != OBJ_ZPOS_TO_MAKE)
                //                next1 = temp;
                //        }
                //        else if (next2 == 0)
                //        {
                //            if (temp != OBJ_ZPOS_TO_MAKE)
                //                next2 = temp;
                //        }
                //    }

                //    int rand = Random.Range(0, 2);
                //    if (rand == 0)
                //        spawn_position.z = next1;
                //    else if (rand == 1)
                //        spawn_position.z = next2;

                //    // this.obj_creator.create_bomb(spawn_position);
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);
                //}
                //else
                //{
                //    spawn_position.z = LANE1_Zpos;
                //    // this.obj_creator.create_bomb(spawn_position);
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);
                //    spawn_position.z = LANE2_Zpos;
                //    // this.obj_creator.create_bomb(spawn_position);
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);
                //    spawn_position.z = LANE3_Zpos;
                //    // this.obj_creator.create_bomb(spawn_position);
                //    (BombPool.Instance as BombPool).Spawn(spawn_position);
                //}
                break;
            case Type.POWER:
                // 파워 아이템 생성함
                // this.obj_creator.create_powerItem(spawn_position);
                (PowerPool.Instance as PowerPool).Spawn(spawn_position);
                break;
        }



        this.last_type = this.type;    //마지막으로 생성된 오브젝트 저장
        last_obj_position = spawn_position; // last_block의 위치를 이번 위치로 갱신.

        OBJ_NUMBER++;
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어의 X + 오프셋, 오브젝트를 생성하는 문턱 값
        obj_generate_x = this.player.transform.position.x + (float)OBJ_OFFSET;

        // 오브젝트를 만들어야 할 개수만큼 만들었으면 또는 파워 업 아이템이 나와야 하면 연속을 끊음
        if (OBJ_NUMBER == OBJ_NUMBER_TO_MAKE || game_root.distance + (float)OBJ_OFFSET > next_power_up_spawn_position)
        {
            // 레벨 업데이트
            update_level();
            // 만든 오브젝트 초기화
            OBJ_NUMBER = 0;
            // 만들 오브젝트 확률로 계산해서 선택해 줌
            select_next_obj();
            // 만들 오브젝트에 따라서 몇개 연속으로 나올지 선택해 줌
            select_obj_number();
            // 같은 오브젝트가 연속으로 나오면 높이를 바꿔줌
            if (last_type == type)
            {
                //int rand = Random.Range(0, 2);
                //if (OBJ_ZPOS_TO_MAKE == LANE1_Zpos)
                //{
                //    if(rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE3_Zpos;
                //}
                //else if(OBJ_ZPOS_TO_MAKE == LANE2_Zpos)
                //{
                //    if (rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE3_Zpos;
                //}
                //else if (OBJ_ZPOS_TO_MAKE == LANE3_Zpos)
                //{
                //    if (rand == 0)
                //        OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
                //    else
                //        OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
                //}

                OBJ_ZPOS_TO_MAKE = Random.Range(game_root.rightBoundary + ObjZDamp, game_root.leftBoundary - ObjZDamp);
            }
            // 다른 오브젝트일 경우 위치를 랜덤으로 정함
            else
            {
                //int rand = Random.Range(0, 3);
                //if(rand == 0) OBJ_ZPOS_TO_MAKE = LANE1_Zpos;
                //else if (rand == 1) OBJ_ZPOS_TO_MAKE = LANE2_Zpos;
                //else OBJ_ZPOS_TO_MAKE = LANE3_Zpos;

                OBJ_ZPOS_TO_MAKE = Random.Range(game_root.rightBoundary + ObjZDamp, game_root.leftBoundary - ObjZDamp);
            }
        }


        // 마지막에 만든 오브젝트와 거리가 벌어지면 
        if (this.last_obj_position.x + OBJ_WIDTH < obj_generate_x)
        {
            // 아래에 레이를 쏴 보고 땅이 없으면 OBJ_WIDTH를 늘려준다
            Vector3 s = this.last_obj_position; // 다음 오브젝트를 생성할 위치
            s.x += OBJ_WIDTH;
            Vector3 e = s + Vector3.down * 1.1f; // s부터 아래로 1.1f로 이동한 위치.
            RaycastHit hit;
            // s부터 e 사이에 아무것도 없을 때. *out: method 내에서 생선된 값을 반환때 사용.
            if (!Physics.Linecast(s, e, out hit, layerMask))
            {
                Debug.Log("땅이 없음");
                OBJ_WIDTH += 1.0f;// 히트한 경우 땅이 없음 땅을 늘려줘야 함
                // 빈 칸 때문에 오브젝트가 밀렸다고 알려 줌
                isObjectOnLift = true;
            }
            // 땅이 시작하는 위치이면 8.0f 만큼 더 넓혀 줌
            else if (isObjectOnLift)
            {
                OBJ_WIDTH += 8.0f;
                // 빈 칸 때문에 오브젝트가 밀렸다고 알려 주는 플래그 초기화
                isObjectOnLift = false;
            }
            // 땅이 있으면 오브젝트 생성함
            else
            {
                s.x -= 1.0f;
                e = s + Vector3.down * 1.1f; // s부터 아래로 1.1f로 이동한 위치.
                // 땅이 있긴 한데 앞부분에 땅이 없으면
                if (!Physics.Linecast(s, e, out hit, layerMask))
                {
                    Debug.Log("땅에 걸침");
                    OBJ_WIDTH += 1.0f;// 히트한 경우 땅이 없음 땅을 늘려줘야 함
                                      // 빈 칸 때문에 오브젝트가 밀렸다고 알려 줌
                    isObjectOnLift = true;
                }

                else
                {
                    // 빈 칸 때문에 오브젝트가 밀렸다고 알려 주는 플래그 초기화
                    isObjectOnLift = false;

                    // 몬스터를 만든다.
                    this.spawn_obj();

                    // 총 이동거리에 따라 오브젝트 사이의 거리가 달라짐

                    // 오브젝트 간 거리를 정해 줌
                    OBJ_WIDTH = Random.Range(OBJ_WIDTH_MIN, OBJ_WIDTH_MAX);
                }
            }
        }
    }

    private void select_next_obj()
    {
        switch (this.last_type)
        {
            case Type.BOMB:
                type = Type.NONE;
                break;
            default:
                // 거리에 따라 powerup이 나옴
                if(game_root.distance + (float)OBJ_OFFSET > next_power_up_spawn_position)
                {
                    type = Type.POWER;
                    next_power_up_spawn_position += power_up_spawn_distance; // 다음 위치 설정해 줌
                }
                else
                {
                    // 총 이동거리에 따라 오브젝트 생성 확률이 달라짐

                    // 확률에 따라 몬스터 또는 폭탄이 나옴
                    int bomb_percentage = Random.Range(1, 100);

                    if (bomb_percentage <= bomb_spawn_rate)
                        type = Type.BOMB;
                    else
                        type = Type.ENEMY;
                }
                break;
        }
    }

    private void select_obj_number()
    {
        switch (this.type)
        {
            case Type.ENEMY:
                // 몬스터일 경우 연속으로 몇개 나오는지 계산
                OBJ_NUMBER_TO_MAKE = Random.Range(OBJ_NUMBER_MIN, OBJ_NUMBER_MAX);
                break;
            default:
                // 나머지일 경우 하나만 나옴
                OBJ_NUMBER_TO_MAKE = 1;
                break;
        }
    }

    private void update_level()
    {
        int idx = (int)((game_root.distance + (float)OBJ_OFFSET) / update_distance) % level_control.level_datas.Count;

        //Debug.Log(idx);

        LevelData level_data = level_control.level_datas[idx];

        OBJ_WIDTH_MIN = level_data.obj_width.min;
        OBJ_WIDTH_MAX = level_data.obj_width.max;

        OBJ_NUMBER_MIN = (int)level_data.obj_number.min;
        OBJ_NUMBER_MAX = (int)level_data.obj_number.max;

        mapCreator.BLANK_MIN = level_data.blank_width.min;
        mapCreator.BLANK_MAX = level_data.blank_width.max;

        bomb_spawn_rate = level_data.bomb_rate;

        enemy_hp1_spawn_rate = level_data.hp1_rate;
        enemy_hp2_spawn_rate = level_data.hp2_rate;
        enemy_hp3_spawn_rate = level_data.hp3_rate;

        mapCreator.BLANK_RATE = level_data.blank_rate / 100;

        bomb_1_spawn_rate = level_data.bomb1_rate;
        bomb_2_spawn_rate = level_data.bomb2_rate;
        bomb_3_spawn_rate = level_data.bomb3_rate;
    }
}
