using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    public static bool firstLoad = true;

    public GameManager gameManager;
    public Text score;
    public Text scoreInScoreBg;
    public Text bestScore;
    public GameObject buttons;

    [Header("Premium Buttons")]
    public GameObject leaderboardBtn;

    Animator scoreAnimator;
    bool hasCheckedGameOver = false;
    bool comboHeldLastFrame = false;

    void OnEnable()
    {
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        score.gameObject.SetActive(false);
        scoreInScoreBg.text = ScoreManager.Instance.Score.ToString();

        if (!firstLoad)
        {
            HideAllButtons();
        }
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        if (gameManager.gameOver && !hasCheckedGameOver)
        {
            hasCheckedGameOver = true;
            Invoke("ShowButtons", 1f);
        }

        bool comboHeld = Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D);
        if (comboHeld && !comboHeldLastFrame)
        {
            if (firstLoad || gameManager.gameOver)
            {
                HandlePlayButton();
            }
        }

        comboHeldLastFrame = comboHeld;
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    public void HandlePlayButton()
    {
        if (!firstLoad)
        {
            StartCoroutine(Restart());
        }
        else
        {
            HideAllButtons();
            gameManager.StartGame();
            gameManager.CreateBall();
            firstLoad = false;
        }
    }

    public void ShowButtons()
    {
        buttons.SetActive(true);
        score.gameObject.SetActive(false);
        scoreInScoreBg.text = ScoreManager.Instance.Score.ToString();
    }

    public void HideAllButtons()
    {
        buttons.SetActive(false);
        score.gameObject.SetActive(true);
    }

    public void FinishLoading()
    {
        if (firstLoad)
        {
            ShowButtons();
        }
        else
        {
            HideAllButtons();
        }
    }


    IEnumerator Restart()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
        #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServices.Init();
        #endif
        }
        #endif
    }

    public void ShowAchievementUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
        #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServices.Init();
        #endif
        }
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #endif
    }

    public void ShareScreenshot()
    {
        #if EASY_MOBILE
        ScreenshotSharer.Instance.ShareScreenshot();
        #endif
    }

    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void RateApp()
    {
        Utilities.Instance.RateApp();
    }

    public void OpenTwitterPage()
    {
        Utilities.Instance.OpenTwitterPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.Instance.OpenFacebookPage();
    }
}
