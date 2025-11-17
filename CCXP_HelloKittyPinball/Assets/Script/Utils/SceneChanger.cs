using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Utils
{
    public class SceneChanger : MonoBehaviour
    {
        private int availableScenes;
        private bool isChangingScene;
        [Header("Text")] 
        public Color textColor;
        public TextMeshProUGUI textUI;
        private string uiString;
        [Header("Text Background")] 
        public Color colorBackground;
        public TextMeshProUGUI textUIBackground;
        private string uiStringBackground;
        private void Start()
        {
            availableScenes = SceneManager.sceneCountInBuildSettings;
            SetText();
            SetTextBackground();
            SetUITextsTo(false);
        }

        private void SetText()
        {
            uiString = "Pres the scene number to change scene";
            for (int i = 0; i < availableScenes; i++)
            {
                uiString += $"\nscene {i}: {SceneManager.GetSceneByBuildIndex(i)}";
            }
            textUI.text = uiString;
            textUI.color = textColor;
        }
        private void SetTextBackground()
        {
            uiStringBackground = $"<mark=#{ColorUtility.ToHtmlStringRGBA(colorBackground)} padding=\"32, 32, 0, 0$\">{uiString}</mark>";
            textUIBackground.SetText(uiStringBackground);
        }

        private void SetUITextsTo(bool isActive)
        {
            textUI.gameObject.SetActive(isActive);
            textUIBackground.gameObject.SetActive(isActive);
        }
        private void ChangeScene(int index)
        {
            if (availableScenes>=index)
            {
                if (IsActiveScene(index))
                {
                    return;
                }
                SceneManager.LoadScene(index, LoadSceneMode.Single);
                isChangingScene = false;          
                SetUITextsTo(false);
            }
        }

        private static bool IsActiveScene(int index)
        {
            return SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(index);
        }
        private void Update()
        {
            if (!isChangingScene)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    isChangingScene = true;
                    SetUITextsTo(true);

                }
                return;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetUITextsTo(false);
                isChangingScene = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                ChangeScene(0);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)|| Input.GetKeyDown(KeyCode.Keypad1))
            {
                ChangeScene(1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)|| Input.GetKeyDown(KeyCode.Keypad2))
            {
                ChangeScene(2);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)|| Input.GetKeyDown(KeyCode.Keypad3))
            {
                ChangeScene(3);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)|| Input.GetKeyDown(KeyCode.Keypad4))
            {
                ChangeScene(4);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)|| Input.GetKeyDown(KeyCode.Keypad5))
            {
                ChangeScene(5);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6)|| Input.GetKeyDown(KeyCode.Keypad6))
            {
                ChangeScene(6);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7)|| Input.GetKeyDown(KeyCode.Keypad7))
            {
                ChangeScene(7);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8)|| Input.GetKeyDown(KeyCode.Keypad8))
            {
                ChangeScene(8);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha9)|| Input.GetKeyDown(KeyCode.Keypad9))
            {
                ChangeScene(9);
                return;
            }
        }
    }
}