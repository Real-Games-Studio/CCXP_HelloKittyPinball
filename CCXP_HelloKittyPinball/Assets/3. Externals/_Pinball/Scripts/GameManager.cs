using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SgLib;
using System.Collections.Generic;
using System;
using _1._Project.Scripts;
using UnityRawInput;

public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static int GameCount
    { 
        get { return _gameCount; } 
        private set { _gameCount = value; } 
    }

    private static int _gameCount = 0;

    public static event System.Action<GameState, GameState> GameStateChanged = delegate {};

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }

    public GameState _gameState = GameState.Prepare;
    
    [Header("Gameplay References")] 
    public GameObject ballPrefab;
    public GameObject ballPoint;
    public GameObject obstacleManager;
    public GameObject leftFlipper;
    public GameObject rightFlipper;
    public GameObject ushape;
    public GameObject background;
    public GameObject fence;
    public ParticleSystem die;
    public ParticleSystem hitGold;
    [HideInInspector]
    public bool gameOver;

    [Header("Gameplay Config")]
    public Color[] backgroundColor;
    public float torqueForce;
    public int scoreToIncreaseDifficulty = 10;
    public float targetAliveTime = 60;
    public float targetAliveTimeDecreaseValue = 2;
    public int minTargetAliveTime = 3;
    public int scoreToAddedBall = 15;
    public float BallLaunchForce = 15f;
    public int MaxAmountOfBallsInTotal = 2;
    public int MaxAmountOfBallsConcurrently = 2;
    [Tooltip("Automatically start gameplay when the scene loads")]
    public bool startOnSceneLoad = false;

    [Header("Game Timer")]
    [Tooltip("Total play time in seconds before the session ends")]
    public float initialGameDuration = 60f;

    internal List<GameObject> listBall = new List<GameObject>();
    private Rigidbody2D leftFlipperRigid;
    private Rigidbody2D rightFlipperRigid;
    private SpriteRenderer ushapeSpriteRenderer;
    private SpriteRenderer backgroundSpriteRenderer;
    private SpriteRenderer fenceSpriteRenderer;
    private SpriteRenderer leftFlipperSpriteRenderer;
    private SpriteRenderer rightFlipperSpriteRenderer;
    private int obstacleCounter = 0;
    private bool stopProcessing;
    
    private bool _leftFlipperActive;
    private bool _lastRoudnLeftFlipperActive;
    private bool _rightFlipperActive;
    private bool _lastRoudnRightFlipperActive;
    public KeyCode leftFlipperKey = KeyCode.A;
    public KeyCode rightFlipperKey = KeyCode.D;
    public RawKey leftFlipperRawKey = RawKey.A;
    public RawKey rightFlipperRawKey = RawKey.D;

    private float remainingGameTime;
    private bool timerActive;

    public float RemainingGameTime => Mathf.Max(0f, remainingGameTime);

    private void Awake()
    {
        RawInput.WorkInBackground = true;
        RawInput.InterceptMessages = false;
        RawInput.OnKeyDown += RawInputOnOnKeyDownHandler;
        RawInput.OnKeyUp += RawInputOnOnKeyUpHandler;
        RawInput.Start();
    }

    private void OnDestroy()
    {
        RawInput.WorkInBackground = true;
        RawInput.InterceptMessages = false;
        RawInput.OnKeyDown -= RawInputOnOnKeyDownHandler;
        RawInput.OnKeyUp -= RawInputOnOnKeyUpHandler;
        RawInput.Stop();
    }

    private void RawInputOnOnKeyDownHandler(RawKey obj)
    {
        if (obj == leftFlipperRawKey)
        {
            _leftFlipperActive = true;
        }
        if (obj == rightFlipperRawKey)
        {
            _rightFlipperActive = true;
        }
    }
    
    private void RawInputOnOnKeyUpHandler(RawKey obj)
    {
        if (obj == leftFlipperRawKey)
        {
            _leftFlipperActive = false;
            _lastRoudnLeftFlipperActive = false;
        }
        if (obj == rightFlipperRawKey)
        {
            _rightFlipperActive = false;
            _lastRoudnRightFlipperActive = false;
        }
    }
    // Use this for initialization
    void Start()
    {
        GameState = GameState.Prepare;

        ScoreManager.Instance.Reset();
        leftFlipperRigid = leftFlipper.GetComponent<Rigidbody2D>();
        rightFlipperRigid = rightFlipper.GetComponent<Rigidbody2D>();
        ushapeSpriteRenderer = ushape.GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer = background.GetComponent<SpriteRenderer>();
        fenceSpriteRenderer = fence.GetComponent<SpriteRenderer>();
        leftFlipperSpriteRenderer = leftFlipper.GetComponent<SpriteRenderer>();
        rightFlipperSpriteRenderer = rightFlipper.GetComponent<SpriteRenderer>();

        //Change color of backgorund, ushape, fence, flippers
        Color color = backgroundColor[UnityEngine.Random.Range(0, backgroundColor.Length)];
        ushapeSpriteRenderer.color = color;
        backgroundSpriteRenderer.color = color;
        fenceSpriteRenderer.color = color;
        leftFlipperSpriteRenderer.color = color;
        rightFlipperSpriteRenderer.color = color;


        if (startOnSceneLoad)
        {
            StartGame();
            CreateBall();
        }
    }
	
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(leftFlipperKey))
        //{
        //    _leftFlipperActive = true;
        //}
        //if (Input.GetKeyDown(rightFlipperKey))
        //{
        //    _rightFlipperActive = true;
        //}
        //if (Input.GetKeyUp(leftFlipperKey))
        //{
        //    _leftFlipperActive = false;
        //    _lastRoudnLeftFlipperActive = false;
        //}
        //if (Input.GetKeyUp(rightFlipperKey))
        //{
        //    _rightFlipperActive = false;
        //    _lastRoudnRightFlipperActive = false;
        //}
            if (_leftFlipperActive)
            {
                if (!_lastRoudnLeftFlipperActive)
                {
                    _lastRoudnLeftFlipperActive = true;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.flippingLeft);
                }
                AddTorque(leftFlipperRigid, torqueForce);
            }
            if (_rightFlipperActive)
            {
                if (!_lastRoudnRightFlipperActive)
                {
                    _lastRoudnRightFlipperActive = true;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.flippingRight);
                }
                AddTorque(rightFlipperRigid, -torqueForce);
            }
           // }

            if (timerActive && !gameOver && GameState == GameState.Playing)
            {
                remainingGameTime -= Time.deltaTime;
                if (remainingGameTime <= 0f)
                {
                    remainingGameTime = 0f;
                    TriggerTimeGameOver();
                }
            }
    }

    /// <summary>
    /// Fire game event, create gold
    /// </summary>
    public void StartGame()
    {
        if (GameState == GameState.Playing)
        {
            return;
        }

        ResetRuntimeState();
        GameState = GameState.Playing;
        gameOver = false;
        timerActive = true;
        remainingGameTime = Mathf.Max(0f, initialGameDuration);

        StartCoroutine(Processing());
    }

    void GameOver()
    {
        timerActive = false;
        GameState = GameState.GameOver;
    }

    void AddTorque(Rigidbody2D rigid, float force)
    {
        rigid.AddTorque(force);
    }

    /// <summary>
    /// Create a ball
    /// </summary>
    public void CreateBall()
    {
        if (gameOver || listBall.Count >= MaxAmountOfBallsInTotal)
        {
            return;
        }
        GameObject ball = Instantiate(ballPrefab, ballPoint.transform.position, Quaternion.identity) as GameObject;
        listBall.Add(ball);
    }

    /// <summary>
    /// Check game over
    /// </summary>
    /// <param name="the ball"></param>
    public void CheckGameOver(GameObject ball)
    {
        //remove the ball from the list
        listBall.Remove(ball);

        if (!gameOver && listBall.Count < MaxAmountOfBallsConcurrently)
        {
            CreateBall();
        }
    }

    /// <summary>
    /// Change background element color, enable obstacles, update processing time
    /// </summary>
    public void CheckAndUpdateValue()
    {
        //if (ScoreManager.Instance.Score % scoreToIncreaseDifficulty == 0)
        //{
        //    //Change background element color
        //    Color color = backgroundColor[UnityEngine.Random.Range(0, backgroundColor.Length)];
        //    ushapeSpriteRenderer.color = color;
        //    backgroundSpriteRenderer.color = color;
        //    fenceSpriteRenderer.color = color;
        //    leftFlipperSpriteRenderer.color = color;
        //    rightFlipperSpriteRenderer.color = color;
        //
        //    //Enable obstacles
        //    if (obstacleCounter < obstacleManager.transform.childCount)
        //    {
        //        obstacleManager.transform.GetChild(obstacleCounter).gameObject.SetActive(true);
        //        obstacleCounter++;
        //    }
        //
        //    //Update processing time
        //    if (targetAliveTime > minTargetAliveTime)
        //    {
        //        targetAliveTime -= targetAliveTimeDecreaseValue;
        //    }
        //    else
        //    {
        //        targetAliveTime = minTargetAliveTime;
        //    }
        //}
        //
        //if (ScoreManager.Instance.Score % scoreToAddedBall == 0)
        //{
        //    //CreateBall();
        //}
    }

    IEnumerator Processing()
    {
        float t = 0;
        while (t < targetAliveTime)
        {
            t += Time.deltaTime;
            float fraction = t / targetAliveTime;
            float newF = Mathf.Lerp(0, 1, fraction);
            yield return null;
        }

        if (!gameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            gameOver = true;
            for (int i = 0; i < listBall.Count; i++)
            {
                listBall[i].GetComponent<BallController>().Exploring();
            }


            GameOver();
        }      
    }

    private void TriggerTimeGameOver()
    {
        if (gameOver)
        {
            return;
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
        gameOver = true;

        StopAllCoroutines();

        for (int i = 0; i < listBall.Count; i++)
        {
            if (listBall[i] != null)
            {
                listBall[i].GetComponent<BallController>().Exploring();
            }
        }

        listBall.Clear();

        GameOver();
    }

    private void ResetRuntimeState()
    {
        StopAllCoroutines();
        timerActive = false;
        remainingGameTime = Mathf.Max(0f, initialGameDuration);
        gameOver = false;

        for (int i = 0; i < listBall.Count; i++)
        {
            if (listBall[i] != null)
            {
                Destroy(listBall[i]);
            }
        }

        listBall.Clear();
    }
}
