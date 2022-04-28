using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Rank
{
    public string name;
    public int distance;
    public int combo;
    public int score;

    public Rank() { }
}

[System.Serializable]
public class GameData
{
    public List<Rank> ranking = new List<Rank>();
    public float masterVolume = 1.0f;
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;
}

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (!_instance)
            {
                GameObject go = new GameObject();
                go.name = "SaveManager";
                _instance = go.AddComponent(typeof(SaveManager)) as SaveManager;
                DontDestroyOnLoad(go);
                if (_instance == null)
                    Debug.Log("SaveManager does not exist");
            }
            return _instance;
        }
    }

    private string GameDataFileName = @"\GameData.json";

    private GameData _gameData;
    public GameData gameData
    {
        get
        {
            if (_gameData == null)
            {
                LoadRank();
                SaveRank();
            }
            return _gameData;
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

    //[SerializeField]
    //public List<Rank> ranking = new List<Rank>();

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
        if (gameData.ranking.Count < 5)
        {
            gameData.ranking.Add(rank);
        }
        else
        {
            if (gameData.ranking.Exists(element => element.score < rank.score))
            {
                gameData.ranking.RemoveAt(4);
                gameData.ranking.Add(rank);
            }
        }
        gameData.ranking.Sort((a, b) => b.score.CompareTo(a.score));
    }

    public int GetRanking(int score)
    {
        int count = 0;

        foreach (Rank rank in gameData.ranking)
        {
            if (rank.score > score) count++;
        }

        return count;
    }

    public void LoadRank()
    {
        string filePath = Application.dataPath + GameDataFileName;
        
        if (File.Exists(filePath))
        {
            Debug.Log("Load Success");
            string FromJsonData = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }
        else
        {
            Debug.Log("New File Generate");

            _gameData = new GameData();
        }
    }

    public void SaveRank()
    {
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.dataPath + GameDataFileName;

        File.WriteAllText(filePath, ToJsonData);
        Debug.Log("Save Complete");
    }

    private void OnApplicationQuit()
    {
        SaveRank();
    }
}
