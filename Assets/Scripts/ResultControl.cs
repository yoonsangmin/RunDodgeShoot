using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MonoBehaviour
{
    private GameRoot gameRoot = null;

    public Text RankText;
    public Text EncourageText;

    public Text bonusScoreText;

    public Text Distance;
    public Text EnemyKill;
    public Text MaxCombo;
    public Text ComboBonus;
    public Text Sum;

    // Start is called before the first frame update
    void Start()
    {
        gameRoot = GameObject.Find("GameRoot").GetComponent<GameRoot>();
    }

    // Update is called once per frame
    void Update()
    {
        Distance.text = gameRoot.distance.ToString();
        EnemyKill.text = gameRoot.enemyKillScore.ToString();
        MaxCombo.text = gameRoot.maxCombo.ToString();
        ComboBonus.text = gameRoot.comboBonusScore.ToString();
        Sum.text = gameRoot.sumScore.ToString();

        bonusScoreText.text = "최대 콤보 " + gameRoot.bonusScorePeriodCombo.ToString() + "당 " + gameRoot.bonusScorePerMaxCombo.ToString() + "점";

        int rank = TitleRoot.Instance.GetRanking(gameRoot.sumScore) + 1;

        if (rank <= 5)
        {
            RankText.text = rank.ToString() + "등 입니다!";

            if(rank == 1)
            {
                EncourageText.text = "당신을 이길 자는 없습니다!";
            }
            else if (rank <= 3)
            {
                EncourageText.text = "아쉬워요! 최고를 향해 다시 도전해 보세요!";
            }
            else if (rank <= 5)
            {
                EncourageText.text = "랭크 인! 하지만 아직 멀었어요~";
            }
        }
    }

    private void OnEnable()
    {
       
    }
}
