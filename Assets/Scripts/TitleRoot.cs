using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rank
{
    public string name;
    public int distance;
    public int combo;
    public int score;

    public Rank() { }
}

public class TitleRoot : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static TitleRoot _instance;
    // 인스턴스에 접근하기 위한 프로퍼티
    public static TitleRoot Instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(TitleRoot)) as TitleRoot;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);
    }


    public List<Rank> ranking = new List<Rank>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateRank(Rank rank)
    {
        if (ranking.Count < 5)
        {
            ranking.Add(rank);
        }
        else
        {
            if (ranking.Exists(element => element.score < rank.score))
            {
                ranking.RemoveAt(4);
                ranking.Add(rank);
            }
        }
        ranking.Sort((a, b) => b.score.CompareTo(a.score));
    }

    public int GetRanking(int score)
    {
        int count = 0;

        foreach (Rank rank in ranking)
        {
            if (rank.score > score) count++;
        }

        return count;
    }
}
