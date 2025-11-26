using System;
using System.Collections.Generic;
using System.IO;
using RealGames;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScreenRanking : CanvasScreen
{

    [SerializeField] private float displayDuration = 5f; // no final deve chamar outra tela
    [SerializeField] private float displayDuration_EDITOR = 10f; // no final deve chamar outra tela
    [SerializeField] private TMP_Text firstPlaceText;
    [SerializeField] private TMP_Text secondPlaceText;      
    [SerializeField] private TMP_Text thirdPlaceText;

    [SerializeField] private TMP_Text matchPositionText;
    [SerializeField] private TMP_Text matchScoreText;
    [SerializeField] private bool displayDaylyRanking = false;
    [SerializeField] private GameObject arrows;
    [SerializeField] private GameObject arrowsPos1;
    [SerializeField] private GameObject arrowsPos2;
    [SerializeField] private GameObject arrowsPos3;
    [SerializeField] private GameObject arrowsPosDefault;

    public AudioSource EndAudioSource;
    public override void TurnOn()
    {
        base.TurnOn();

        UpdateRankingDisplay();
#if UNITY_EDITOR
        Invoke("CallNextScreen", displayDuration_EDITOR);
#else
       Invoke("CallNextScreen", displayDuration);
       
#endif
    }

    public override void TurnOff()
    {
        if (EndAudioSource != null)
        {
            EndAudioSource.Stop();
        }
        base.TurnOff();
    }

    public override void CallNextScreen()
    {
        ResetGame();
    }

    private void Start()
    {
        GetDailyFromJson();
    }

    private void GetDailyFromJson()
    {
        var json = JsonLoader.LoadGameSettings(Path.Combine(Application.streamingAssetsPath, "appconfig.json"));
        displayDaylyRanking =json.RankingDiario;
    }

    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateRankingDisplay()
    {
        if (ScoreManager.Instance == null)
        {
            ClearRankingTexts();
            return;
        }

        List<ScoreData> topScores = ScoreManager.Instance.GetTopScores(3, displayDaylyRanking);

        AssignPlaceText(firstPlaceText, topScores, 0);
        AssignPlaceText(secondPlaceText, topScores, 1);
        AssignPlaceText(thirdPlaceText, topScores, 2);

        ScoreData? lastScore = ScoreManager.Instance.LastRecordedScore;

        if (lastScore.HasValue)
        {
            SetMatchScore(lastScore.Value.score);
            int? position = ScoreManager.Instance.GetLastRecordedScorePosition(displayDaylyRanking);
            SetMatchPosition(position);
        }
        else
        {
            int currentScore = ScoreManager.Instance.Score;
            SetMatchScore(currentScore);

            int position = displayDaylyRanking
                ? ScoreManager.Instance.GetPositionInRankingOnCurrentDay(currentScore)
                : ScoreManager.Instance.GetPositionInRanking(currentScore);

            SetMatchPosition(position > 0 ? position : (int?)null);
        }
    }

    private void AssignPlaceText(TMP_Text target, List<ScoreData> scores, int index)
    {
        if (target == null)
        {
            return;
        }

        if (scores != null && index < scores.Count)
        {
            ScoreData entry = scores[index];
            target.SetText($"{entry.score}");
        }
        else
        {
            target.SetText("-");
        }
    }

    private void SetMatchScore(int? score)
    {
        if (matchScoreText == null)
        {
            return;
        }

        matchScoreText.SetText(score.Value.ToString());
    }

    private void SetMatchPosition(int? position)
    {
        if (matchPositionText == null)
        {
            return;
        }

        switch (position)
        {
            case 1:
                arrows.transform.localPosition = arrowsPos1.transform.localPosition;
                break;
            case 2:
                arrows.transform.localPosition = arrowsPos2.transform.localPosition;
                break;
            case 3:
                arrows.transform.localPosition =  arrowsPos3.transform.localPosition;
                break;
            default:
                arrows.transform.localPosition = arrowsPosDefault.transform.localPosition;
                break;
        }
        matchPositionText.SetText(position.HasValue && position.Value > 0 ? $"{position.Value}" : "");
    }

    private void ClearRankingTexts()
    {
        AssignPlaceText(firstPlaceText, null, 0);
        AssignPlaceText(secondPlaceText, null, 0);
        AssignPlaceText(thirdPlaceText, null, 0);
        SetMatchScore(null);
        SetMatchPosition(null);
    }
}
