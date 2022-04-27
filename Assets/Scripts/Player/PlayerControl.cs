using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameObject playerModel;

    public LayerMask layerMask;

    // 점프에 필요한 전역변수 선언 먼저.
    public float ACCELERATION = 10.0f; // 가속도.

    public float Speed; // 속도.
    public float speed { get { return Speed; } set { Speed = value; } }

    public float SPEED_MIN = 13.0f; // 속도의 최솟값.
    public float SPEED_MAX = 30.0f; // 속도의 최댓값.
    public float SPEED_PREV; // 이전 속도값

    public float zSpeed = 10.0f;

    public float JUMP_HEIGHT_MAX = 2.0f; // 점프 높이.
    //public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // 점프 후의 감속도.

    public float NARAKU_HEIGHT = -5.0f;

    private struct Key
    { // 키 조작 정보 구조체.
        public bool jump; // spacebar
        public bool left; // A
        public bool right; // D
        public bool shoot; // J, K

    };

    private Key aforekey; // 미리 입력한 키 조작 정보를 보관하는 변수.
    private Key key; // 딜레이가 끝나면 aforekey의 정보를 전달하고 참고해서 실행함

    public float input_delay = 0.5f; // 버튼 딜레이 설정
    public float input_timer = 0.0f; // 경과 시간.

    public float input_delay_MIN = 0.17f; // 딜레이 최솟값
    public float input_delay_PREV; // 이전 딜레이 값

    private float key_delay = 0.3f; // 버튼 딜레이 설정
    private float a_key_timer = 0.0f; // 경과 시간.
    private float d_key_timer = 0.0f; // 경과 시간.
    private float space_key_timer = 0.0f; // 경과 시간.

    public enum STEP
    { // Player의 각종 상태를 나타내는 자료형 (*열거체)
        NONE = -1, // 상태정보 없음.
        RUN = 0, // 달린다.
        JUMP, // 점프.
        MISS, // 실패.
        NUM, // 상태가 몇 종류 있는지 보여준다(=3).
    };

    public STEP step = STEP.NONE; // Player의 현재 상태.
    public STEP next_step = STEP.NONE; // Player의 다음 상태.

    public float step_timer = 0.0f; // 경과 시간.

    private bool is_landed = false; // 착지했는가.
    private bool is_colided = false; // 뭔가와 충돌했는가.
    private bool is_key_released = false; // 버튼이 떨어졌는가.

    private int multiJumpCount = 1; // 다중 점프 몇 번 가능한지
    private int left_multiJumpCount = 1; // 다중 점프 몇 번 했는지
    private bool doMultiJump; // 다중 점프 플래그

    public bool is_power_upped = false; // 파워 업 되었는지

    private float bonusTimeWhenGetPowerUpItem = 5f;

    // 속도, 연사 속도 업데이트 거리
    private int update_distance = 100;
    public int next_update_position; // 다음 업데이트 위치
    private float speed_update_parameter = 0.1f; // 속도 업데이트 파라미터
    private float delay_update_parameter = 0.2f; // 딜레이 업데이트 파라미터

    private GameRoot gameroot = null; // 게임 루트

    public GameObject powerUpEffect;    // 파워업 이펙트 효과
    public GameObject resultPanel;    // 결과 패널

    private bool isGameOverSoundPlayed = false; // 게임 오버 사운드 1회 재생 플래그

    private float bullet_speed = 30f; // 총알 속도, 플레이어 속도랑 더해서 사용함
    public Vector3 bullet_offset = new Vector3(1.0f, 0.0f, 0.0f);

    // 끼인거 확인
    private Vector3 prevPos;
    private float stuckedTimer = 0.0f;
    private float stuckedTime = 1.0f;

    void Start()
    {
        this.step = STEP.JUMP;

        // 게임 루트 가져옴
        this.gameroot = GameObject.Find("GameRoot").GetComponent<GameRoot>();

        // 초기 속도 설정
        Speed = SPEED_MIN;
        SPEED_PREV = Speed;

        input_delay_PREV = input_delay;

        next_update_position = update_distance;

        // 결과 화면 초기화
        resultPanel.SetActive(false);
    }

    private void check_landed() // 착지했는지 조사
    {
        this.is_landed = false; // 일단 false로 설정.
        do
        {
            Vector3 s = this.transform.position; // Player의 현재 위치.
            Vector3 e = s + Vector3.down * 1.0f; // s부터 아래로 1.0f로 이동한 위치.
            RaycastHit hit;
            // s부터 e 사이에 아무것도 없을 때. *out: method 내에서 생선된 값을 반환때 사용.
            if(!Physics.Linecast(s, e, out hit, layerMask)) {
                break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
            }
            // s부터 e 사이에 뭔가 있을 때 아래의 처리가 실행.
            if(this.step == STEP.JUMP) { // 현재, 점프 상태라면.
                if(this.step_timer < 0.3f) { // 경과 시간이 0.3f 미만이라면.
                    break; // 아무것도 하지 않고 do~while 루프를 빠져나감(탈출구로).
                }
            }
            // s부터 e 사이에 뭔가 있고 JUMP 직후가 아닐 때만 아래가 실행.
            this.is_landed = true;
        } while (false);
        // 루프의 탈출구.
    }

    // Update is called once per frame
    void Update()
    {
        if(this.step != STEP.MISS)
        {
            if ((int)prevPos.x == (int)this.transform.position.x)
            {
                stuckedTimer += Time.deltaTime;
                if (stuckedTimer > stuckedTime)
                {
                    Debug.Log("꼈음");
                    next_step = STEP.MISS;
                    this.GetComponent<Rigidbody>().AddForce(Vector3.left * 50000f);
                }
            }
            else
            {
                stuckedTimer = 0.0f;
            }

            if (gameroot.limitTime == 0)
                next_step = STEP.MISS;
        }

        // this.get_input_without_aforeinput(); // 입력 정보 취득.
        this.get_input(); // 입력 정보 취득.

        this.set_speed_and_inputdelay(); // 게임 진행에 따른 스피드를 prev 스피드에 넣어 놓음
        this.speed_delay_setting(); // 파워 업 아이템 적용 유무에 따라 prev 스피드 또는 맥스 스피드를 현재 스피드에 넣음
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity; // 속도를 설정.
        this.check_landed(); // 착지 상태인지 체크.

        if(this.is_landed && this.step != STEP.MISS) // 착지했으면 속도를 다시 늘려줌
            velocity.x = Speed;

        //Debug.Log(is_landed);
        //Debug.Log(step_timer);
        //Debug.Log(step);

        //// 플레이어 현재 위치 저장
        //Vector3 curPos = this.transform.position;
        //switch (playerLane)
        //{
        //    case 1:
        //        curPos.z = LANE1_Zpos;
        //        break;
        //    case 2:
        //        curPos.z = LANE2_Zpos;
        //        break;
        //    case 3:
        //        curPos.z = LANE3_Zpos;
        //        break;
        //}
        //// 플레이어 레인 값에 따라 플레이어 Z값을 바꾼 후 적용해줌
        //this.transform.position = curPos;

        switch (this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                // 현재 위치가 한계치보다 아래면.
                if (this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS; // '실패' 상태로 한다.
                }
                break;
        }

        this.step_timer += Time.deltaTime; // 경과 시간을 진행한다.
                                           // 다음 상태가 정해져 있지 않으면 상태의 변화를 조사한다.
        if(this.next_step == STEP.NONE) {
            switch (this.step)
            { // Player의 현재 상태로 분기.
                case STEP.RUN: // 달리는 중일 때.
                    if (!this.is_landed)
                    {
                        this.next_step = STEP.MISS;
                        // 달리는 중이고 착지하지 않은 경우 아무것도 하지 않는다.
                    }
                    else
                    {
                        if (this.key.jump)
                        {
                            // 달리는 중이고 착지했고 스페이스 버튼이 눌렸다면.
                            // 다음 상태를 점프로 변경.
                            this.next_step = STEP.JUMP;
                        }
                    }
                    break;
                case STEP.JUMP: // 점프 중일 때.
                    //Debug.Log(this.is_landed);
                    //Debug.Log(this.step_timer);
                    if (this.is_landed)
                    {
                        // 다중 점프 횟수를 초기화
                        this.left_multiJumpCount = 0;

                        // 착지 중에 점프했으면 점프를 함
                        if (this.key.jump)
                        {
                            // 달리는 중이고 착지했고 스페이스 버튼이 눌렸다면.
                            // 다음 상태를 점프로 변경.
                            this.next_step = STEP.JUMP;
                        }
                        else
                        {
                            // 점프 중이고 착지했다면 다음 상태를 주행 중으로 변경.
                            this.next_step = STEP.RUN;
                        }
                    }
                    else
                    {
                        if (this.key.jump)
                        {
                            // 점프 중이고 착지했고 스페이스 버튼이 눌렸다면.
                            // 한 번 더 점프하라고 알려줌
                            if (this.left_multiJumpCount < this.multiJumpCount)
                            {
                                // 점프 횟수 증가
                                this.left_multiJumpCount++;
                                // 점프 하라고 넘겨줌
                                this.doMultiJump = true;
                            }
                        }
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
                case STEP.JUMP: // '점프'일 때.
                                // 최고 도달점 높이(JUMP_HEIGHT_MAX)까지 점프할 수 있는 속도를 계산.
                    velocity.y = Mathf.Sqrt(2.0f * -Physics.gravity.y * JUMP_HEIGHT_MAX);

                    // 점프 소리
                    SoundManager.Instance.Play("Jump");

                    //// '버튼이 떨어졌음을 나타내는 플래그'를 클리어한다.
                    //this.is_key_released = false;
                    break;
                case STEP.MISS:
                    velocity.x = 5.0f;
                    break;
            }
            // 상태가 변했으므로 경과 시간을 제로로 리셋.
            this.step_timer = 0.0f;
        }
        // 상태별로 매 프레임 갱신 처리.
        switch (this.step)
        {
            case STEP.RUN: // 달리는 중일 때.
                           // 속도를 높인다.
                           // 가속도 없이 속도를 고정시킴
                velocity.x = Speed;

                // 플레이어 현재 위치
                Vector3 curPos = this.transform.position;
                // 플레이어 튕기는 거 예방
                // 플레이어 현재 위치 저장
                curPos.y = 1.0f;

                //velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
                //// 속도가 최고 속도 제한을 넘으면.
                //if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)
                //{
                //    // 최고 속도 제한 이하로 유지한다.
                //    velocity.x *= PlayerControl.SPEED_MAX /
                //    Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                //}
                // 이동키를 누르면 플레이어 레인 값을 바꿈

                if (this.key.left)
                {
                    // playerLane = (playerLane - 1 == 0) ? 1 : playerLane - 1;
                    curPos.z += zSpeed * Time.deltaTime;
                    if (curPos.z > gameroot.leftBoundary)
                        curPos.z = gameroot.leftBoundary;
                }
                if (this.key.right)
                {
                    // playerLane = (playerLane + 1 == 4) ? 3 : playerLane + 1;
                    curPos.z -= zSpeed * Time.deltaTime;
                    if (curPos.z < gameroot.rightBoundary)
                        curPos.z = gameroot.rightBoundary;
                }

                // 플레이어 위치 값에 y값을 고정함
                this.transform.position = curPos;

                if (this.key.shoot)
                {
                    shoot();
                }
                break;
            case STEP.JUMP: // 점프 중일 때.
                            //do
                            //{
                            //    // '버튼이 떨어진 순간'이 아니면.
                            //    if (!Input.GetMouseButtonUp(0))
                            //    {
                            //        break; // 아무것도 하지 않고 루프를 빠져나간다.
                            //    }
                            //    // 이미 감속된 상태면(두 번이상 감속하지 않도록).
                            //    if (this.is_key_released)
                            //    {
                            //        break; // 아무것도 하지 않고 루프를 빠져나간다.
                            //    }
                            //    // 상하방향 속도가 0 이하면(하강 중이라면).
                            //    if (velocity.y <= 0.0f)
                            //    {
                            //        break; // 아무것도 하지 않고 루프를 빠져나간다.
                            //    }
                            //    // 버튼이 떨어져 있고 상승 중이라면 감속 시작.
                            //    // 점프의 상승은 여기서 끝.
                            //    velocity.y *= JUMP_KEY_RELEASE_REDUCE;
                            //    this.is_key_released = true;
                            //} while (false);
                            // 다중 점프 하라고 하면
                if (this.doMultiJump)
                {
                    Debug.Log("이중점프");
                    // 최고 도달점 높이(JUMP_HEIGHT_MAX)까지 점프할 수 있는 속도를 계산.
                    velocity.y = Mathf.Sqrt(2.0f * -Physics.gravity.y * JUMP_HEIGHT_MAX);

                    // 플래그 클리어
                    this.doMultiJump = false;

                    // 점프 소리
                    SoundManager.Instance.Play("Jump");
                }
                break;

            case STEP.MISS:
                // 게임오버 사운드 재생
                if (!isGameOverSoundPlayed)
                {
                    SoundManager.Instance.Play("GameOver");
                    isGameOverSoundPlayed = true;
                }

                // 가속도(ACCELERATION)를 빼서 Player의 속도를 느리게 해 간다.
                velocity.x -= ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)
                { // Player의 속도가 마이너스면.
                    velocity.x = 0.0f; // 0으로 한다.
                    // 결과 화면을 띄움
                    resultPanel.SetActive(true);
                }

                playerModel.GetComponent<Animator>().SetBool("IsDeath", true);
                break;
        }
        // Rigidbody의 속도를 위에서 구한 속도로 갱신.
        // (이 행은 상태에 관계없이 매번 실행된다).
        this.GetComponent<Rigidbody>().velocity = velocity;
        this.prevPos = this.transform.position;
    }


    // 키 입력을 조사해 그 결과를 바탕으로 맴버 변수 key의 값을 갱신한다.
    private void get_input()
    {
        this.input_timer += Time.deltaTime; // 경과 시간을 진행한다.

        // J, K 키가 눌렸으면 true를 대입.
        // this.aforekey.shoot |= Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K);
        this.aforekey.shoot = true;

        this.key.shoot = false;

        this.key.jump = false;



//#if UNITY_EDITOR
//        // A 키가 눌렸으면 true를 대입.
//        this.key.left = Input.GetKey(KeyCode.A);
//        // D 키가 눌렸으면 true를 대입.
//        this.key.right = Input.GetKey(KeyCode.D);
//        // Space 키가 눌렸으면 true를 대입.
//        this.key.jump = Input.GetKeyDown(KeyCode.Space);
//#endif

//#if UNITY_ANDROID
        this.key.left = this.aforekey.left;
        this.key.right = this.aforekey.right;
        this.key.jump = this.aforekey.jump;

        this.aforekey.jump = false;
//#endif

        // 총알 만 입력 시간 기다림
        if (this.input_timer > this.input_delay)
        {
            // J, K 키가 눌렸으면 true를 대입.
            this.key.shoot = this.aforekey.shoot;

            this.aforekey.shoot = false;
        }

        if (this.key.shoot)
            this.input_timer = 0;
    }

    // 입력 딜레이 중에 키를 입력하면 입력되지 않고 씹힐 수 있음, 
    private void get_input_without_aforeinput()
    {
        this.input_timer += Time.deltaTime; // 경과 시간을 진행한다.

        this.key.jump = false;
        this.key.left = false;
        this.key.right = false;
        this.key.shoot = false;

        // A 키가 눌렸으면 true를 대입.
        this.key.left = Input.GetKey(KeyCode.LeftArrow);
        // D 키가 눌렸으면 true를 대입.
        this.key.right = Input.GetKey(KeyCode.RightArrow);
        // Space 키가 눌렸으면 true를 대입.
        this.key.jump = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space);

        // 왼쪽키 딜레이
        if(this.key.left)
        {
            if(a_key_timer == 0)
            {
                a_key_timer = key_delay;
            }
            else
            {
                this.key.left = false;
                a_key_timer -= Time.deltaTime;
                if (a_key_timer <= 0)
                    a_key_timer = 0;
            }
        }
        else
        {
            a_key_timer = 0;
        }

        // 오른쪽키 딜레이
        if (this.key.right)
        {
            if (d_key_timer == 0)
            {
                d_key_timer = key_delay;
            }
            else
            {
                this.key.right = false;
                d_key_timer -= Time.deltaTime;
                if (d_key_timer <= 0)
                    d_key_timer = 0;
            }
        }
        else
        {
            d_key_timer = 0;
        }

        // 점프키 딜레이
        //if (this.key.jump)
        //{
        //    if (space_key_timer == 0)
        //    {
        //        space_key_timer = key_delay;
        //    }
        //    else
        //    {
        //        this.key.jump = false;
        //        space_key_timer -= Time.deltaTime;
        //        if (space_key_timer <= 0)
        //            space_key_timer = 0;
        //    }
        //}
        //else
        //{
        //    space_key_timer = 0;
        //}

        // 총알 만 입력 시간 기다림
        if (this.input_timer > this.input_delay)
        {
            // J, K 키가 눌렸으면 true를 대입.
            this.key.shoot = Input.GetKey(KeyCode.F);
        }

        if (this.key.shoot)
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
            {
                Debug.Log("게임 오버");
                this.next_step = STEP.MISS;
            }

            SoundManager.Instance.Play("Hitted");
        }

        // 파워 아이템 먹으면
        if (other.gameObject.CompareTag("Power"))
        {
            if (!is_power_upped)
            {
                SPEED_PREV = speed;
                is_power_upped = true;
            }

            gameroot.limitTime += bonusTimeWhenGetPowerUpItem;
        }
    }

    private void set_speed_and_inputdelay()
    {
        if (is_power_upped)
        {
            speed = SPEED_MAX;
            input_delay = input_delay_MIN;
            powerUpEffect.SetActive(true);
        }

        else
        {
            speed = SPEED_PREV;
            input_delay = input_delay_PREV;
            powerUpEffect.SetActive(false);
        }

    }

    private void speed_delay_setting()
    {
        if (gameroot.distance > next_update_position)
        {
            SPEED_PREV += (SPEED_MAX - SPEED_PREV) * this.speed_update_parameter;
            input_delay_PREV += (input_delay_MIN - input_delay_PREV) * this.delay_update_parameter;

            Debug.Log(input_delay_PREV);
            next_update_position += update_distance;
        }
    }

    public void shoot()
    {
        // 슈팅 사운드 재생
        SoundManager.Instance.Play("Shooting");
        // BulletControl bulletControl = Instantiate(p_bullet).GetComponent<BulletControl>();  // 총알 풀에서 총알 총알을 하나 사용함
        BulletControl bulletControl = BulletPool.Instance.Spawn().GetComponent<BulletControl>();
        bulletControl.transform.position = this.transform.position + bullet_offset;    // 총알 위치를 오프셋으로 옮겨줌
        bulletControl.bullet_speed = bullet_speed;   // 총알 속도 넘겨줌
    }

    public void OnJumpButtonClick()
    {
        this.aforekey.jump = true;
    }

    public void OnLeftButtonDown()
    {
        this.aforekey.left = true;
    }

    public void OnRightButtonDown()
    {
        this.aforekey.right = true;
    }

    public void OnLeftButtonUp()
    {
        this.aforekey.left = false;
    }

    public void OnRightButtonUp()
    {
        this.aforekey.right = false;
    }
}
