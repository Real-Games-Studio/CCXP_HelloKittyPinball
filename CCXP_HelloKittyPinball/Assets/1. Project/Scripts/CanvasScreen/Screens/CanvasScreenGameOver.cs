using System;
using System.Collections.Generic;
using System.IO;
using RealGames;
using SgLib;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CanvasScreenGameOver : CanvasScreen
{
   [SerializeField] private float gameOverDisplaytime = 2.0f;
   [SerializeField] private float gameOverDisplaytime_EDITOR = 10f;
   [SerializeField]private GameManager gameManager;
   [SerializeField] public TMP_Text finalMessageText; // deve sortear uma das mensagens finais
   [SerializeField] public List<string> finalMessages;
   [SerializeField] public TMP_Text finalScoreText;

   public AudioSource EndAudioSource;
    private bool writing = false;

    private void Start()
    {
        
        gameOverDisplaytime = JsonLoader.LoadGameSettings(Path.Combine(Application.streamingAssetsPath, "appconfig.json"))
            .TempoGameOver;
    }

    override public void TurnOn()
   {
        if (EndAudioSource != null)
        {
            if (!EndAudioSource.isPlaying)
            {
                EndAudioSource.Play();
            }
        }
       base.TurnOn();

     if(!writing)
     {
        if(finalMessages.Count > 0)
        {
            int randomIndex = Random.Range(0, finalMessages.Count);
            finalMessageText.SetText(finalMessages[randomIndex]);
        }

        finalScoreText.SetText(ScoreManager.Instance.Score.ToString());


       // aqui podemos colocar uma chamada para salvar o score do jogador

       if (ScoreManager.Instance != null)
       {
           ScoreManager.Instance.WriteScoreDataToCSV();
       }
           writing = true;

     }
#if UNITY_EDITOR
       Invoke("CallNextScreen", gameOverDisplaytime_EDITOR);
#else
       Invoke("CallNextScreen", gameOverDisplaytime);
       
#endif
   }

   override public void TurnOff()
   {
        writing = false;
       base.TurnOff();
   }
}
