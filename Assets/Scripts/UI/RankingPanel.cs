using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RankingPanel : MonoBehaviour
{
    [Serializable]
    public struct TextField
    {
        public Text name;
        public Text distance;
        public Text combo;
        public Text score;
    };

    public TextField[] rankingText;

    // Start is called before the first frame update
    void Start()
    {
        UpdateFields();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFields();
    }

    private void UpdateFields()
    {
        for (int i = 0; i < rankingText.Length; i++)
        {
            if (SaveManager.Instance.gameData.ranking.Count < i + 1)
            {
                rankingText[i].name.text = "-";
                rankingText[i].distance.text = "-";
                rankingText[i].combo.text = "-";
                rankingText[i].score.text = "-";
            }
            else
            {
                rankingText[i].name.text = SaveManager.Instance.gameData.ranking[i].name;
                rankingText[i].distance.text = ToStringAndAddComma(SaveManager.Instance.gameData.ranking[i].distance);
                
                if (SaveManager.Instance.gameData.ranking[i].combo == 0)
                    rankingText[i].combo.text = "0";
                else
                    rankingText[i].combo.text = ToStringAndAddComma(SaveManager.Instance.gameData.ranking[i].combo);
                
                rankingText[i].score.text = ToStringAndAddComma(SaveManager.Instance.gameData.ranking[i].score);
            }
        }
    }

    public string ToStringAndAddComma(int i)
    {
        string s = "";

        while(i > 0)
        {
            int remain = i % 1000;

            s = remain.ToString() + s;
            
            if(i / 1000 != 0)
            {
                if (remain / 100 == 0)
                    s = "0" + s;
                if (remain / 10 == 0)
                    s = "0" + s;
            }
            
            i /= 1000;
            if(i > 0)
                s = "," + s;
        }

        return s;
    }
}
