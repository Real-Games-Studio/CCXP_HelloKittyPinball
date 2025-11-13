using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScreenCTA : CanvasScreen
{
    [SerializeField]
    private GameManager gameManager;

    [System.Serializable]
    private struct CallToActionStage
    {
        public float holdDuration;
        public GameObject instructionObject;
        public bool canStartGame;
    }

    [SerializeField]
    private List<CallToActionStage> callToActionStages = new List<CallToActionStage>();

    private Coroutine stageRoutine;
    private int currentStageIndex = -1;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        DeactivateAllInstructions();
    }

    public override void TurnOn()
    {
        base.TurnOn();

        if (stageRoutine != null)
        {
            StopCoroutine(stageRoutine);
        }

        if (callToActionStages.Count == 0)
        {
            currentStageIndex = -1;
            return;
        }

        stageRoutine = StartCoroutine(RunStagesLoop());
    }

    public override void TurnOff()
    {

        if (stageRoutine != null)
        {
            StopCoroutine(stageRoutine);
            stageRoutine = null;
        }

        currentStageIndex = -1;
        DeactivateAllInstructions();

        base.TurnOff();
    }

    private IEnumerator RunStagesLoop()
    {
        int nextIndex = 0;

        while (true)
        {
            SetStage(nextIndex);

            CallToActionStage stage = callToActionStages[nextIndex];
            float duration = Mathf.Max(0f, stage.holdDuration);

            if (duration > 0f)
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }

            nextIndex = (nextIndex + 1) % callToActionStages.Count;
        }
    }

    private void SetStage(int stageIndex)
    {
        currentStageIndex = stageIndex;
        DeactivateAllInstructions();

        GameObject targetInstruction = callToActionStages[stageIndex].instructionObject;
        if (targetInstruction != null)
        {
            targetInstruction.SetActive(true);
        }
    }

    private void DeactivateAllInstructions()
    {
        for (int i = 0; i < callToActionStages.Count; i++)
        {
            GameObject instruction = callToActionStages[i].instructionObject;
            if (instruction != null)
            {
                instruction.SetActive(false);
            }
        }
    }

    private bool CurrentStageAllowsStart()
    {
        if (currentStageIndex < 0 || currentStageIndex >= callToActionStages.Count)
        {
            return false;
        }

        return callToActionStages[currentStageIndex].canStartGame;
    }

    private bool CanTriggerGameStart()
    {
        return gameManager != null && gameManager.GameState != GameState.Playing;
    }

    private void Update()
    {
        if (!IsOn())
        {
            return;
        }

        if (!CurrentStageAllowsStart())
        {
            return;
        }

        bool comboHeld = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D);

        if (comboHeld && CanTriggerGameStart())
        {
            Debug.Log("Combo held, starting game...");
            gameManager.StartGame();
            gameManager.CreateBall();

            CallNextScreen();
        }
    }
}
