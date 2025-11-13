using SgLib;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasScreenGameplay : CanvasScreen
{
    [SerializeField]private GameManager gameManager;
    public TMP_Text timerText;
    public TMP_Text scoreText;



    void Update()
    {
        if(IsOn())
        {
            timerText.SetText(gameManager.RemainingGameTime.ToString("F0"));
            scoreText.SetText(ScoreManager.Instance.Score.ToString());

            if(gameManager.GameState == GameState.GameOver)
            {

                Invoke("CallNextScreen", 1f);
            }

        }
    }
}
