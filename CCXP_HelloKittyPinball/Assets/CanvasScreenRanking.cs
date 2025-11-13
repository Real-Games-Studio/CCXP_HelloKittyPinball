using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScreenRanking : CanvasScreen
{

    [SerializeField] private float displayDuration = 5f; // no final deve chamar outra tela
    [SerializeField] private TMP_Text firstPlaceText;
    [SerializeField] private TMP_Text secondPlaceText;      
    [SerializeField] private TMP_Text thirdPlaceText;

    [SerializeField] private TMP_Text matchPositionText;
    [SerializeField] private TMP_Text matchScoreText;


    public override void TurnOn()
    {
        base.TurnOn();

        //UpdateRankingDisplay();
        Invoke("ResetGame", displayDuration);
    }


    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
