using System.Collections.Generic;
using SgLib;
using TMPro;
using UnityEngine;

public class CanvasScreenGameOver : CanvasScreen
{
   [SerializeField] private float gameOverDisplaytime = 2.0f;
   [SerializeField]private GameManager gameManager;
   [SerializeField] public TMP_Text finalMessageText; // deve sortear uma das mensagens finais
   [SerializeField] public List<string> finalMessages;
   [SerializeField] public TMP_Text finalScoreText;


   override public void TurnOn()
   {
       base.TurnOn();

       if(finalMessages.Count > 0)
       {
           int randomIndex = Random.Range(0, finalMessages.Count);
           finalMessageText.SetText(finalMessages[randomIndex]);
       }

       finalScoreText.SetText(ScoreManager.Instance.Score.ToString());


       // aqui podemos colocar uma chamada para salvar o score do jogador


       Invoke("CallNextScreen", gameOverDisplaytime);
   }
}
