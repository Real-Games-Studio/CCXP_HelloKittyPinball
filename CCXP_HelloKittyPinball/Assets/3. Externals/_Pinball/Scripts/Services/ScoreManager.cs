using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ScoreData
{
    public int score;
    public DateTime dateAchieved;
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score;

    public bool HasNewHighScore { get; private set; }

    public static event Action<int> ScoreUpdated = delegate {};

    public List<ScoreData> ScoresHistory = new List<ScoreData>();


    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Reset();
    }
    
    public void AddScore(int amount)
    {
        Score += amount;

        // Fire event
        ScoreUpdated(Score);
    }

    public void Reset()
    {
        Score = 0;
        HasNewHighScore = false;
    }

    public void WriteScoreDataToCSV()
    {
        
    }



    public List<ScoreData> ReadScoreDataFromCSV()
    {
        return new List<ScoreData>();
    }


    public int GetPositionInRanking(int score)
    {
        List<ScoreData> allScores = ReadScoreDataFromCSV();
        allScores.Add(new ScoreData { score = score, dateAchieved = DateTime.Now });
        List<ScoreData> organizedScores = OrganizeScoresDescending(allScores);

        for (int i = 0; i < organizedScores.Count; i++)
        {
            if (organizedScores[i].score == score)
            {
                return i + 1; // Positions are 1-based
            }
        }

        return organizedScores.Count; // If not found, return last position
    }

    public int GetPositionInRankingOnCurrentDay(int score)
    {
        DateTime today = DateTime.Now.Date;
        List<ScoreData> allScores = ReadScoreDataFromCSV();
        List<ScoreData> todaysScores = allScores.FindAll(s => s.dateAchieved.Date == today);
        todaysScores.Add(new ScoreData { score = score, dateAchieved = DateTime.Now });
        List<ScoreData> organizedScores = OrganizeScoresDescending(todaysScores);

        for (int i = 0; i < organizedScores.Count; i++)
        {
            if (organizedScores[i].score == score)
            {
                return i + 1; // Positions are 1-based
            }
        }

        return organizedScores.Count; // If not found, return last position
    }

    public List<ScoreData> OrganizeScoresDescending(List<ScoreData> scores)
    {
        scores.Sort((a, b) => b.score.CompareTo(a.score));
        return scores;
    }

    public List<ScoreData> GetScoresByDate(TimeSpan from, TimeSpan to)
    {
        List<ScoreData> allScores = ReadScoreDataFromCSV();
        List<ScoreData> filteredScores = allScores.FindAll(scoreData =>
        {
            TimeSpan scoreTimeSpan = scoreData.dateAchieved.TimeOfDay;
            return scoreTimeSpan >= from && scoreTimeSpan <= to;
        });

        return filteredScores;
    }

}
