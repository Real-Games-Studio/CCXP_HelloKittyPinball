using System.Collections.Generic;
using UnityEngine;

public class CanvasScreenCTA : CanvasScreen
{
    private GameManager gameManager;


    [System.Serializable]
    private struct CallToActionStage
    {
        public float holdDuration;
        public GameObject instructionObject;
        public bool canStartGame;
    }

    private List<CallToActionStage> callToActionStages;


    void Update()
    {
        if(IsOn())
        {
            // Handle the update logic for the CTA screen here
            bool comboHeld = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D);
           
           if(comboHeld)
           {
               gameManager.StartGame();
               gameManager.CreateBall();
           }
       }
   }
}
