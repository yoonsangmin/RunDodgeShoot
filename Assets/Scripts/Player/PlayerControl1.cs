using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl1 : MonoBehaviour
{
    // 점프에 필요한 전역변수 선언 먼저.
    public float Speed; // 속도.
    public float speed { get { return Speed; } set { Speed = value; } }

    public float SPEED_MIN = 10.0f; // 속도의 최솟값.
    public float SPEED_MAX = 20.0f; // 속도의 최댓값.
    public float SPEED_PREV; // 이전 속도값

    private struct Key
    { // 키 조작 정보 구조체.
        public bool up; // D, F
        public bool down; // J, K
    };

    private Key aforekey; // 미리 입력한 키 조작 정보를 보관하는 변수.
    private Key key; // 딜레이가 끝나면 aforekey의 정보를 전달하고 참고해서 실행함

    public float input_delay = 0.1f; // 버튼 딜레이 설정
    public float input_timer = 0.0f; // 경과 시간.

    public float input_delay_MIN = 0.01f; // 딜레이 최솟값
    public float input_delay_PREV; // 이전 딜레이 값

    public enum STEP
    { // Player의 각종 상태를 나타내는 자료형 (*열거체)
        NONE = -1, // 상태정보 없음.
        Floor1 = 0, // 1층에서 달림
        Floor2, // 2층에서 날면서 달림
        MISS, // 실패.
        NUM, // 상태가 몇 종류 있는지 보여준다(=3).
    };
    public STEP step = STEP.NONE; // Player의 현재 상태.
    public STEP next_step = STEP.NONE; // Player의 다음 상태.
    public float step_timer = 0.0f; // 경과 시간.

    private bool is_colided = false; // 뭔가와 충돌했는가.

    private float Floor1_Ypos;
    private float Floor2_Ypos;

    public bool is_power_upped = false; // 파워 업 되었는지

    private GameRoot gameroot = null;

    // 속도, 연사 속도 업데이트 거리
    private int update_distance = 100;
    public int next_update_position; // 다음 업데이트 위치
    private float speed_update_parameter = 0.03f; // 속도 업데이트 파라미터
    private float delay_update_parameter = 0.2f; // 딜레이 업데이트 파라미터

    void Start()
    {
        this.next_step = STEP.Floor1;
        this.Floor1_Ypos = GameObject.Find("Floor1").transform.position.y;
        this.Floor2_Ypos = GameObject.Find("Floor2").transform.position.y;

        this.gameroot = GameObject.Find("GameRoot").GetComponent<GameRoot>();

        Speed = SPEED_MIN;
        SPEED_PREV = Speed;
        input_delay_PREV = input_delay;

        next_update_position = update_distance;
    }

    // Update is called once per frame
    void Update()
    {
        this.get_input_without_aforeinput(); // 입력 정보 취득.
        this.set_speed_and_inputdelay();    // 이동속도랑 인풋 딜레이를 정해줌
        this.speed_delay_setting();

        this.step_timer += Time.deltaTime; // 경과 시간을 진행한다.

        // 다음 상태가 정해져 있지 않으면 상태의 변화를 조사한다.
        if (this.next_step == STEP.NONE) {
            switch (this.step)
            { // Player의 현재 상태로 분기.
                case STEP.Floor1: // 달리는 중일 때.
                    if (this.key.up)
                    {
                        // 달리는 중이고 착지했고 W버튼이 눌렸다면.
                        // 다음 상태를 점프로 변경.
                        this.next_step = STEP.Floor2;
                    }
                    break;
                case STEP.Floor2: // 점프 중일 때.
                    if (this.key.down)
                    {
                        // 달리는 중이고 공중에 있고 S 버튼이 눌렸다면.
                        // 다음 상태를 점프로 변경.
                        this.next_step = STEP.Floor1;
                    }
                    break;
            }
        }

        // '다음 정보'가 '상태 정보 없음'이 아닌 동안(상태가 변할 때만).
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step; // '현재 상태'를 '다음 상태'로 갱신.
            this.next_step = STEP.NONE; // '다음 상태'를 '상태 없음'으로 변경.
            switch (this.step)
            { // 갱신된 '현재 상태'가.
                case STEP.Floor1: // '1층'일 때.
                                  // 1층 위치를 받아와서 1층으로 옮김
                    this.transform.position = new Vector3(this.transform.position.x, this.Floor1_Ypos, this.transform.position.z);
                    break;
                case STEP.Floor2: // '2층'일 때.
                                  // 2층 위치를 받아와서 2층으로 옮김
                    this.transform.position = new Vector3(this.transform.position.x, this.Floor2_Ypos, this.transform.position.z);
                    break;
            }
            // 상태가 변했으므로 경과 시간을 제로로 리셋.
            this.step_timer = 0.0f;
        }
        // 상태별로 매 프레임 갱신 처리.
        switch (this.step)
        {
            case STEP.Floor1: // 1층에서 달리는 중일 때.

                if (this.key.down)
                {
                    // gameroot.shoot();
                }

                    this.transform.Translate(new Vector3(0.0f, 0.0f, Speed * Time.deltaTime));
                break;

            case STEP.Floor2: // 점프 중일 때.

                if (this.key.up)
                {
                    // gameroot.shoot();
                }

                this.transform.Translate(new Vector3(0.0f, 0.0f, Speed * Time.deltaTime));
                break;
        }
        // Rigidbody의 속도를 위에서 구한 속도로 갱신.
    }

    // 키 입력을 조사해 그 결과를 바탕으로 맴버 변수 key의 값을 갱신한다.
    private void get_input()
    {
        this.input_timer += Time.deltaTime; // 경과 시간을 진행한다.

        // S, D키가 눌렸으면 true를 대입.
        this.aforekey.up |= Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.F);
        // J, K 키가 눌렸으면 true를 대입.
        this.aforekey.down |= Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K);

        this.key.up = false;
        this.key.down = false;

        //입력 시간 기다림
        if (this.input_timer > this.input_delay)
        {
            // S, D키가 눌렸으면 true를 대입.
            this.key.up = this.aforekey.up;
            // J, K 키가 눌렸으면 true를 대입.
            this.key.down = this.aforekey.down;

            this.aforekey.up = false;
            this.aforekey.down = false;
        }

        if (this.key.up || this.key.down)
            this.input_timer = 0;
    }

    // 입력 딜레이 중에 키를 입력하면 입력되지 않고 씹힐 수 있음, 
    private void get_input_without_aforeinput()
    {
        this.input_timer += Time.deltaTime; // 경과 시간을 진행한다.

        this.key.up = false;
        this.key.down = false;

        //입력 시간 기다림
        if (this.input_timer > this.input_delay)
        {
            // S, D키가 눌렸으면 true를 대입.
            this.key.up |= Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.F);
            // J, K 키가 눌렸으면 true를 대입.
            this.key.down |= Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K);
        }

        if (this.key.up || this.key.down)
            this.input_timer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 장애물과 부딪히면 게임 오버
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log("들어왔음");
            // 파워 업이 false
            if (is_power_upped)
                is_power_upped = false;
            else
                Debug.Log("게임 오버");
        }

        // 파워 아이템 먹으면
        if (other.gameObject.CompareTag("Power"))
        {
            if(!is_power_upped)
            {
                SPEED_PREV = speed;
                is_power_upped = true;
            }
        }
    }

    private void set_speed_and_inputdelay()
    {
        if(is_power_upped)
        {
            speed = SPEED_MAX;
            input_delay = input_delay_MIN;
        }

        else
        {
            speed = SPEED_PREV;
            input_delay = input_delay_PREV;
        }

    }

    private void speed_delay_setting()
    {
        if(gameroot.distance > next_update_position)
        {
            SPEED_PREV += (SPEED_MAX - SPEED_PREV) * this.speed_update_parameter;
            input_delay_PREV += (input_delay_MIN - input_delay_PREV) * this.delay_update_parameter;

            Debug.Log(input_delay_PREV);
            next_update_position += update_distance;
        }
    }
}