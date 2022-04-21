using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public float BLOCK_WIDTH = 20.0f; // 블록의 폭.

    public float BLANK_WIDTH = 1.0f; // 공백의 폭.
    public float BLANK_MIN = 3.0f;   // 공백 최소 폭
    public float BLANK_MAX = 10.0f;  // 공백 최대 폭

    public float BLANK_WHEN_POWER_UPPED = 12.0f;  // 파워 업 상태에서 최대 넓이

    public float BLANK_RATE = 0.1f; // 공백 나올 확률

    public float BLOCK_HEIGHT = 0.2f; // 블록의 높이.
    public int BLOCK_NUM_IN_SCREEN = 8; // 화면 내에 들어가는 블록의 개수.

    private struct FloorBlock
    { // 블록에 관한 정보를 모아서 관리하는 구조체 (여러 개의 정보를 하나로 묶을 때 사용).
        public bool is_created; // 블록이 만들어졌는가.
        public Vector3 position; // 블록의 위치.
    };
    private FloorBlock last_block; // 마지막에 생성한 블록.
    private PlayerControl player = null; // 씬상의 Player를 보관.
    private BlockCreator block_creator; // BlockCreator를 보관.

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.last_block.is_created = false;
        this.block_creator = this.gameObject.GetComponent<BlockCreator>();

        InitialCreate();
    }

    // MapCreator.cs
    private void create_floor_block()
    {
        Vector3 block_position; // 이제부터 만들 블록의 위치.

        if(!this.last_block.is_created) { // last_block이 생성되지 않은 경우.
                                          // 블록의 위치를 일단 Player와 같게 한다.
            block_position = this.player.transform.position;
            // 그러고 나서 블록의 X 위치를 화면 절반만큼 왼쪽으로 이동.
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
            // 블록의 Y위치는 0으로.
            block_position.y = 0.0f;
        } 
        
        else
        { // last_block이 생성된 경우
          // 이번에 만들 블록의 위치를 직전에 만든 블록과 같게.
            block_position = this.last_block.position;
        }
        block_position.x += BLOCK_WIDTH + BLANK_WIDTH; // 블록을 넓이 + 공백 넓이 만큼 오른쪽으로 이동.
                                         // BlockCreator 스크립트의 createBlock()메소드에 생성을 지시!.
        this.block_creator.createBlock(block_position); // 이제까지의 코드에서 설정한 block_position을 건네준다.
        this.last_block.position = block_position; // last_block의 위치를 이번 위치로 갱신.
        this.last_block.is_created = true; // 블록이 생성되었으므로 last_block의 is_created를 true로 설정.
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어의 X위치를 가져온다.
        float block_generate_x = this.player.transform.position.x;
        // 그리고 대략 반 화면만큼 오른쪽으로 이동.
        // 이 위치가 블록을 생성하는 문턱 값이 된다.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;
        // 마지막에 만든 블록의 위치가 문턱 값보다 작으면.
        while (this.last_block.position.x < block_generate_x)
        {
            // 블록을 만든다.
            // 공백인지 결정한다.
            float rand = Random.Range(0.01f, 1);
            // 공백이면 공백 넓이를 정해 줌
            if (rand < BLANK_RATE)
            {
                // 플레이어가 파워 업 되어 있는지 확인한다. = 파워업 상태에선 폭이 넓어야 함
                //if (this.player.is_power_upped)
                //{
                //    BLANK_WIDTH = BLANK_WHEN_POWER_UPPED;
                //}
                //else
                //{
                //    float width = Random.Range(BLANK_MIN, BLANK_MAX);
                //    Debug.Log(width);
                //    BLANK_WIDTH = width;
                //}
                float width = Random.Range(BLANK_MIN, BLANK_MAX);
                Debug.Log(width);
                BLANK_WIDTH = width;
            }
            // 공백이 아니면 공백 0으로 만듦
            else BLANK_WIDTH = 0.0f;

            this.create_floor_block();
        }
    }

    void InitialCreate()
    {
        // 플레이어의 X위치를 가져온다.
        float block_generate_x = this.player.transform.position.x;
        // 그리고 대략 반 화면만큼 오른쪽으로 이동.
        // 이 위치가 블록을 생성하는 문턱 값이 된다.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;
        // 마지막에 만든 블록의 위치가 문턱 값보다 작으면.
        while (this.last_block.position.x < block_generate_x)
        {
            // 블록을 만든다.
            BLANK_WIDTH = 0.0f;

            this.create_floor_block();
        }
    }

    public bool isDelete(GameObject block_object)
    {
        bool ret = false; // 반환값.
                          // Player로부터 반 화면만큼 왼쪽에 위치, 이 위치가 사라지느냐 마느냐를 결정하는 문턱 값이 됨.
        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);
        // 블록의 위치가 문턱 값보다 작으면(왼쪽),
        if (block_object.transform.position.x < left_limit)
        {
            ret = true; // 반환값을 true(사라져도 좋다)로
        }
        return (ret); // 판정 결과를 돌려줌.
    }
}
