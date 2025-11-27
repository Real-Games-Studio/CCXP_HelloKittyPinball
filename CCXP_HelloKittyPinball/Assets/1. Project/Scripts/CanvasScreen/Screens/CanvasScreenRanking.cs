using System;
using System.Collections.Generic;
using System.IO;
using RealGames;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.Serialization;

public class CanvasScreenRanking : CanvasScreen
{

    [SerializeField] private float displayDuration = 5f; // no final deve chamar outra tela
    [SerializeField] private float displayDuration_EDITOR = 10f; // no final deve chamar outra tela
    [SerializeField] private TMP_Text firstPlaceText;
    [SerializeField] private TMP_Text secondPlaceText;
    [SerializeField] private TMP_Text thirdPlaceText;

    [SerializeField] private TMP_Text matchPositionText;
    [SerializeField] private TMP_Text matchScoreText;
    [FormerlySerializedAs("counterAnimator")] [SerializeField] private CounterAnimator PlayerCounterAnimator;
    [SerializeField] private CounterAnimator FirstPlayerCounterAnimator;
    [SerializeField] private CounterAnimator SecondPlayerCounterAnimator;
    [SerializeField] private CounterAnimator ThirdPlayerCounterAnimator;
    [SerializeField] private bool displayDaylyRanking = false;
    [SerializeField] private GameObject arrows;
    [SerializeField] private GameObject arrowsPos1;
    [SerializeField] private GameObject arrowsPos2;
    [SerializeField] private GameObject arrowsPos3;
    [SerializeField] private GameObject arrowsPosDefault;

    public int TimerPlayer = 2;
    public int TimerOld = 0;
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
        displayDuration = JsonLoader.LoadGameSettings(Path.Combine(Application.streamingAssetsPath, "appconfig.json"))
            .TempoRanking;
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
            int? position = ScoreManager.Instance.GetLastRecordedScorePosition(displayDaylyRanking);
            SetMatchPosition(position);
            if (position<=3)
            {
                if (matchScoreText == null)
                {
                    return;
                }

                SetMatchScore(-1, TimerOld);
                switch (position)
                {
                    case 1:
                        SetCoreFirst(lastScore.Value.score, TimerPlayer);
                        break;
                    case 2:
                        SetCoreSecond(lastScore.Value.score, TimerPlayer);
                        break;
                    case 3:
                        SetCoreThird(lastScore.Value.score, TimerPlayer);
                        break;
                }
            }
            else
            {
                SetMatchScore(lastScore.Value.score, TimerPlayer);
            }
        }
        else
        {
            int currentScore = ScoreManager.Instance.Score;

            int position = displayDaylyRanking
                ? ScoreManager.Instance.GetPositionInRankingOnCurrentDay(currentScore)
                : ScoreManager.Instance.GetPositionInRanking(currentScore);
            
            if (position<=3)
            {
                
                if (matchScoreText == null)
                {
                    return;
                }

                SetMatchScore(-1, TimerOld);
                switch (position)
                {
                    case 1:
                        SetCoreFirst(lastScore.Value.score, TimerPlayer);
                        break;
                    case 2:
                        SetCoreSecond(lastScore.Value.score, TimerPlayer);
                        break;
                    case 3:
                        SetCoreThird(lastScore.Value.score, TimerPlayer);
                        break;
                }
            }
            else
            {
                SetMatchScore(lastScore.Value.score, TimerPlayer);
            }

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

    private void SetCoreFirst(int? score, int timer)
    {
        if (matchScoreText == null)
        {
            return;
        }

        FirstPlayerCounterAnimator.StartCount(score.Value, timer);
    }
    private void SetCoreSecond(int? score, int timer)
    {
        if (matchScoreText == null)
        {
            return;
        }

        SecondPlayerCounterAnimator.StartCount(score.Value, timer);
    }
    private void SetCoreThird(int? score, int timer)
    {
        if (matchScoreText == null)
        {
            return;
        }

        ThirdPlayerCounterAnimator.StartCount(score.Value, timer);
    }
    private void SetMatchScore(int? score, int timer)
    {
        if (matchScoreText == null)
        {
            return;
        }

        PlayerCounterAnimator.StartCount(score.Value, timer);
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
                SetMatchScore(-1,TimerOld);
                matchPositionText.SetText("");
                break;
            case 2:
                arrows.transform.localPosition = arrowsPos2.transform.localPosition;
                SetMatchScore(-1,TimerOld);
                matchPositionText.SetText("");
                break;
            case 3:
                arrows.transform.localPosition =  arrowsPos3.transform.localPosition;
                SetMatchScore(-1,TimerOld);
                matchPositionText.SetText("");
                break;
            default:
                arrows.transform.localPosition = arrowsPosDefault.transform.localPosition;
                matchPositionText.SetText(position.HasValue && position.Value > 0 ? $"{position.Value}" : "");
                break;
        }
    }

    private void ClearRankingTexts()
    {
        AssignPlaceText(firstPlaceText, null, 0);
        AssignPlaceText(secondPlaceText, null, 0);
        AssignPlaceText(thirdPlaceText, null, 0);
        SetMatchScore(null,0);
        SetMatchPosition(null);
    }
}
