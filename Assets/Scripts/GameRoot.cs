using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoot : MonoBehaviour
{
    public ButtonControl buttonControl;

    public InputField nameField;

    private float bullet_speed = 30f; // 총알 속도, 플레이어 속도랑 더해서 사용함
    public Vector3 bullet_offset = new Vector3(1.0f, 0.0f, 0.0f);

    public GameObject p_bullet;

    public float delta_score_timer = 0.0f; // 델타 스코어 (다음 점수) 시간
    public int delta_Score = 0; // 델타 스코어(다음 점수)
    
    
    public int sumScore = 0;   // 최종 점수
    public int enemyKillScore = 0;   // 몬스터 처치 점수
    public int combo = 0; // 현재 콤보
    public int maxCombo = 0; // 최대 콤보
    public int comboBonusScore = 0; // 최대 콤보 추가 점수

    public int bonusScorePeriodCombo = 100;  // 몇 콤보 마다 추가 점수가 있는지
    public int bonusScorePerMaxCombo = 1000; // 콤보당 추가 점수

    public float limitTime = 60;

    private float start_x;
    public int distance = 0; // 이동 거리
    private float distance_scale = 0.4f;

    private PlayerControl playercontrol = null;

    public GUIStyle guistyle; // 폰트 스타일.

    public Text limitTimeText;
    public Text scoreText;
    public Text distanceText;
    public Text deltaScoreText;
    public Text comboText;
    public GameObject comboPanel;

    // 음향
    public AudioClip inGameSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        enemyKillScore = 0;
        combo = 0;
        this.playercontrol = GameObject.Find("Player").GetComponent<PlayerControl>();

        this.guistyle.fontSize = 16;

        
        this.start_x = playercontrol.transform.position.x;


        // 음향 초기화
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = inGameSound;
        audioSource.volume = 0.6f;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // 이동거리
        distance = (int)((playercontrol.transform.position.x - start_x) * distance_scale);
        // 플레이 타임 플레이어 살아있을 때만 증가하게 하기
        if (playercontrol.step != PlayerControl.STEP.MISS)
        {
            if (limitTime > 0)
                limitTime -= Time.deltaTime;
            else if (limitTime <= 0)
                limitTime = 0;
        }
        delta_score_timer += Time.deltaTime;

        // 맥스 콤보 저장
        maxCombo = Mathf.Max(maxCombo, combo);
        // 맥스 콤보 추가 점수
        comboBonusScore = (maxCombo / bonusScorePeriodCombo) * bonusScorePerMaxCombo;

        // 최종 점수 저장
        sumScore = distance + enemyKillScore + comboBonusScore;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (nameField.text != "")
            {
                RegisterScore();
                buttonControl.ToTitleScene();
            }
        }

        if (playercontrol.is_power_upped)
            comboText.color = new Color(0.7924528f, 0.6735982f, 0.1756853f);
        else
            comboText.color = new Color(1, 1, 1);
    }

    public void shoot()
    {
        BulletControl bulletControl = Instantiate(p_bullet).GetComponent<BulletControl>();  // 총알 풀에서 총알 총알을 하나 사용함
        bulletControl.transform.position = playercontrol.transform.position + bullet_offset;    // 총알 위치를 오프셋으로 옮겨줌
        bulletControl.bullet_speed = bullet_speed;   // 총알 속도 넘겨줌
    }

    public void increase_combo()
    {
        combo++;
    }

    public void end_combo()
    {
        combo = 0;
    }

    // 콤보, 거리, 스코어, 시간 표시
    void OnGUI()
    {
        float x = Screen.width / 2 - 10.0f;
        float y = 5.0f;
        if (combo > 1)
        {
            //GUI.Label(new Rect(x - 380, y + 80f, 200.0f, 20.0f), "Combo: " + combo.ToString(), guistyle);
            comboPanel.SetActive(true);
            comboText.text = "Combo: " + combo.ToString();
        }
        else
        {
            comboPanel.SetActive(false);
            comboText.text = "";
        }
        //GUI.Label(new Rect(x - 250, y, 200.0f, 20.0f), "LimitTime: " + limitTime.ToString(), guistyle);
        //GUI.Label(new Rect(x + 180, y, 200.0f, 20.0f), "Distacne: " + distance.ToString(), guistyle);
        //GUI.Label(new Rect(x - 30, y, 200.0f, 20.0f), "Score: " + sumScore.ToString(), guistyle);

        limitTimeText.text = "LimitTime: " + limitTime.ToString();
        scoreText.text = "Score: " + sumScore.ToString();
        distanceText.text = "Distacne: " + distance.ToString();

        if (delta_Score > 0 && delta_score_timer < 1.0f)
        {
            this.guistyle.fontSize = 12;
            //GUI.Label(new Rect(x, y + 20, 200.0f, 20.0f), "+ " + delta_Score.ToString(), guistyle);
            this.guistyle.fontSize = 16;

            deltaScoreText.text = "+ " + delta_Score.ToString();
        }
        else
        {
            deltaScoreText.text = "";
        }
    }

    public void kill_enemy_score()
    {
        delta_score_timer = 0.0f;
        delta_Score = 10 + combo / 10;
        enemyKillScore += delta_Score;
    }

    public void RegisterScore()
    {
        Rank r = new Rank();
        r.name = nameField.text;
        r.distance = distance;
        r.combo = maxCombo;
        r.score = sumScore;
        TitleRoot.Instance.UpdateRank(r);
    }
}
