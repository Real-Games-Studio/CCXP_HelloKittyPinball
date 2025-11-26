using SgLib;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasScreenGameplay : CanvasScreen
{
    [SerializeField]private GameManager gameManager;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    [SerializeField][Min(0.001f)] private float scoreTickInterval = 0.02f;

    private int displayedScore;
    private float scoreTickTimer;

    private bool _isGameOver;

    void Update()
    {
        if(IsOn())
        {
            timerText.SetText(gameManager.RemainingGameTime.ToString("F0"));
            HandleScoreTick();

            if(gameManager.GameState == GameState.GameOver)
            {
                if (!_isGameOver)
                {
                    _isGameOver = true;
                    Invoke("CallNextScreen", 1f);
                }
            }

        }
    }

    public override void TurnOn()
    {
        _isGameOver = false;
        base.TurnOn();
    }

    private void Start()
    {
        if(ScoreManager.Instance != null)
        {
            displayedScore = ScoreManager.Instance.Score;
        }
        UpdateScoreLabel();
    }

    private void HandleScoreTick()
    {
        if(ScoreManager.Instance == null)
        {
            return;
        }

        int targetScore = ScoreManager.Instance.Score;

        if(displayedScore >= targetScore)
        {
            if(displayedScore != targetScore)
            {
                displayedScore = targetScore;
                UpdateScoreLabel();
            }

            scoreTickTimer = 0f;
            return;
        }

        scoreTickTimer += Time.deltaTime;

        while(scoreTickTimer >= scoreTickInterval && displayedScore < targetScore)
        {
            scoreTickTimer -= scoreTickInterval;
            displayedScore++;
            UpdateScoreLabel();
        }
    }

    private void UpdateScoreLabel()
    {
        if(scoreText == null)
        {
            return;
        }

        scoreText.SetText(displayedScore.ToString());
    }
}
