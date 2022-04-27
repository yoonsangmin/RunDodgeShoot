using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public struct Range
    { // 범위를 표현하는 구조체.
        public float min; // 범위의 최솟값.
        public float max; // 범위의 최댓값.
    };

    public Range obj_width; // 오브젝트 거리 범위.
    public Range obj_number; // 오브젝트 개수 범위.
    public Range blank_width; // 오브젝트 개수 범위.
    public float bomb_rate; // 톱날 확률
    public float hp1_rate; // 체력 1 에너미 확률
    public float hp2_rate; // 체력 2 에너미 확률
    public float hp3_rate; // 체력 3 에너미 확률
    public float blank_rate; // 낭떠러지 에너미 확률
    public float bomb1_rate; // 폭탄 1개 에너미 확률
    public float bomb2_rate; // 폭탄 2개 에너미 확률
    public float bomb3_rate; // 폭탄 3개 에너미 확률

    public LevelData()
    {
        this.obj_width.min = 10; // 오브젝트 거리의 최솟값을 초기화.
        this.obj_width.max = 10; // 오브젝트 거리의 최댓값을 초기화.
        this.obj_number.min = 2; // 오브젝트 개수의 최솟값을 초기화.
        this.obj_number.max = 6; // 오브젝트 개수의 최댓값을 초기화.
        this.blank_width.min = 3; // 오브젝트 개수의 최댓값을 초기화.
        this.blank_width.max = 10; // 오브젝트 개수의 최댓값을 초기화.
        this.bomb_rate = 10; // 톱날 확률 초기화.
        this.hp1_rate = 100; // 체력 1 에너미 확률 초기화.
        this.hp2_rate = 0; // 체력 2 에너미 확률 초기화.
        this.hp3_rate = 0; // 체력 3 에너미 확률 초기화.
        this.blank_rate = 0; // 낭떠러지 확률 초기화.
        this.bomb1_rate = 0; // 폭탄 1개 확률 초기화.
        this.bomb2_rate = 0; // 폭탄 2개 확률 초기화.
        this.bomb3_rate = 0; // 폭탄 3개 확률 초기화.
    }
}

public class LevelControl : MonoBehaviour
{
    public List<LevelData> level_datas = new List<LevelData>();

    public void loadLevelData(TextAsset level_data_text)
    {
        string level_texts = level_data_text.text; // 텍스트 데이터를 문자열로 가져온다.
        string[] lines = level_texts.Split('\n'); // 개행 코드 '\'마다 분할해서 문자열 배열에 넣는다.
                                                  // lines 내의 각 행에 대해서 차례로 처리해 가는 루프.
        foreach (var line in lines)
        {
            if (line == "")
            { // 행이 빈 줄이면.
                continue; // 아래 처리는 하지 않고 반복문의 처음으로 점프한다.
            };
            Debug.Log(line); // 행의 내용을 디버그 출력한다.
            string[] words = line.Split(); // 행 내의 워드를 배열에 저장한다.
            int n = 0;
            // LevelData형 변수를 생성한다.
            // 현재 처리하는 행의 데이터를 넣어 간다.
            LevelData level_data = new LevelData();
            // words내의 각 워드에 대해서 순서대로 처리해 가는 루프.
            foreach (var word in words)
            {
                if(word.StartsWith("#")) { // 워드의 시작문자가 #이면.
                    break;
                } // 루프 탈출.
                if (word == "")
                { // 워드가 텅 비었으면.
                    continue;
                } // 루프의 시작으로 점프한다.

                // n 값을 0, 1, 2,...7로 변화시켜 감으로써 8항목을 처리한다.
                // 각 워드를 플롯값으로 변환하고 level_data에 저장한다.
                switch (n)
                {
                    case 0: break;
                    case 1: level_data.obj_width.min = float.Parse(word); break;
                    case 2: level_data.obj_width.max = float.Parse(word); break;
                    case 3: level_data.obj_number.min = float.Parse(word); break;
                    case 4: level_data.obj_number.max = float.Parse(word); break;
                    case 5: level_data.blank_width.min = float.Parse(word); break;
                    case 6: level_data.blank_width.max = float.Parse(word); break;
                    case 7: level_data.bomb_rate = float.Parse(word); break;
                    case 8: level_data.hp1_rate = float.Parse(word); break;
                    case 9: level_data.hp2_rate = float.Parse(word); break;
                    case 10: level_data.hp3_rate = float.Parse(word); break;
                    case 11: level_data.blank_rate = float.Parse(word); break;
                    case 12: level_data.bomb1_rate = float.Parse(word); break;
                    case 13: level_data.bomb2_rate = float.Parse(word); break;
                    case 14: level_data.bomb3_rate = float.Parse(word); break;
                }
                n++;
            }
            if (n >= 8)
            { // 8항목(이상)이 제대로 처리되었다면.
                this.level_datas.Add(level_data); // List 구조의 level_datas에 level_data를 추가한다.
            }
            else
            { // 그렇지 않다면(오류의 가능성이 있다).
                if (n == 0)
                { // 1워드도 처리하지 않은 경우는 주석이므로.
                  // 문제없다. 아무것도 하지 않는다.
                }
                else
                { // 그 이외이면 오류다.
                  // 데이터 개수가 맞지 않다는 것을 보여주는 오류 메시지를 표시한다.
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }
        if (this.level_datas.Count == 0)
        { // level_datas에 데이터가 하나도 없으면.
            Debug.LogError("[LevelData] Has no data.\n"); // 오류 메시지를 표시한다.
            this.level_datas.Add(new LevelData()); // level_datas에 기본 LevelData를 하나 추가해 둔다.
        }
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
