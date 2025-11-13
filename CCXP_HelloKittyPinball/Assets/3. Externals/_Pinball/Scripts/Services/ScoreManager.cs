using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

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

    public ScoreData? LastRecordedScore { get; private set; }

    private const string CsvHeader = "score,dateAchieved";

    private string ScoreDataFilePath => Path.Combine(Application.streamingAssetsPath, "scoredata.csv");


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
            EnsureScoreStorageReady();
        }

        
    }

    void Start()
    {
        Reset();
        ScoresHistory = ReadScoreDataFromCSV();
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
        LastRecordedScore = null;
    }

    public void WriteScoreDataToCSV()
    {
        Debug.Log("ScoreManager: Writing score data to CSV.");

        EnsureScoreStorageReady();

        ScoreData newData = new ScoreData
        {
            score = Score,
            dateAchieved = DateTime.Now
        };

        LastRecordedScore = newData;

        Debug.Log($"ScoreManager: New score recorded - {newData.score} at {newData.dateAchieved}.");

        try
        {
            string line = string.Format(CultureInfo.InvariantCulture, "{0},{1}", newData.score, newData.dateAchieved.ToString("o", CultureInfo.InvariantCulture));
            Debug.Log($"ScoreManager: Writing line to CSV - {line}.");

            File.AppendAllText(ScoreDataFilePath, line + Environment.NewLine);
            ScoresHistory.Add(newData);
            UpdateHighScoreFlag(newData.score);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScoreManager: Failed to write score data. {ex.Message}");
        }
    }



    public List<ScoreData> ReadScoreDataFromCSV()
    {
        EnsureScoreStorageReady();

        List<ScoreData> results = new List<ScoreData>();

        try
        {
            string[] lines = File.ReadAllLines(ScoreDataFilePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("score", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string[] columns = line.Split(',');
                if (columns.Length < 2)
                {
                    continue;
                }

                if (int.TryParse(columns[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedScore) &&
                    DateTime.TryParse(columns[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsedDate))
                {
                    results.Add(new ScoreData { score = parsedScore, dateAchieved = parsedDate });
                }
                else
                {
                    Debug.LogWarning($"ScoreManager: Failed to parse score entry '{line}'.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScoreManager: Failed to read score data. {ex.Message}");
        }

        ScoresHistory = new List<ScoreData>(results);
        return results;
    }

    public List<ScoreData> GetTopScores(int count, bool onlyToday = false)
    {
        List<ScoreData> allScores = ReadScoreDataFromCSV();

        if (onlyToday)
        {
            DateTime today = DateTime.Now.Date;
            allScores = allScores.FindAll(scoreData => scoreData.dateAchieved.Date == today);
        }

        List<ScoreData> orderedScores = OrganizeScoresDescending(allScores);
        return orderedScores.Take(Mathf.Max(0, count)).ToList();
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

    public int GetPositionInRankingForScore(ScoreData targetScore, bool onlyToday = false)
    {
        List<ScoreData> allScores = ReadScoreDataFromCSV();

        if (onlyToday)
        {
            DateTime today = DateTime.Now.Date;
            allScores = allScores.FindAll(scoreData => scoreData.dateAchieved.Date == today);
        }

        List<ScoreData> organizedScores = OrganizeScoresDescending(allScores);

        for (int i = 0; i < organizedScores.Count; i++)
        {
            ScoreData current = organizedScores[i];
            if (current.score == targetScore.score && current.dateAchieved == targetScore.dateAchieved)
            {
                return i + 1;
            }
        }

        for (int i = 0; i < organizedScores.Count; i++)
        {
            if (organizedScores[i].score == targetScore.score)
            {
                return i + 1;
            }
        }

        return organizedScores.Count > 0 ? organizedScores.Count : 0;
    }

    public int? GetLastRecordedScorePosition(bool onlyToday = false)
    {
        if (!LastRecordedScore.HasValue)
        {
            return null;
        }

        return GetPositionInRankingForScore(LastRecordedScore.Value, onlyToday);
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

    private void EnsureScoreStorageReady()
    {
        try
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            if (!File.Exists(ScoreDataFilePath))
            {
                File.WriteAllText(ScoreDataFilePath, CsvHeader + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScoreManager: Failed to initialize score data storage. {ex.Message}");
        }
    }

    private void UpdateHighScoreFlag(int lastScore)
    {
        try
        {
            List<ScoreData> allScores = ReadScoreDataFromCSV();

            if (allScores.Count == 0)
            {
                HasNewHighScore = true;
                return;
            }

            int highestScore = allScores.Max(scoreData => scoreData.score);
            HasNewHighScore = lastScore >= highestScore;
        }
        catch (Exception ex)
        {
            Debug.LogError($"ScoreManager: Failed to evaluate high score. {ex.Message}");
        }
    }
}
