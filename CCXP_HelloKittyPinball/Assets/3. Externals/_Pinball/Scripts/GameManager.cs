using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SgLib;
using System.Collections.Generic;
using System;

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
    public GameObject targetPointManager;
    public GameObject leftFlipper;
    public GameObject rightFlipper;
    public GameObject targetPrefab;
    public GameObject ushape;
    public GameObject background;
    public GameObject fence;
    [HideInInspector]
    public GameObject currentTargetPoint;
    [HideInInspector]
    public GameObject currentTarget;
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

    private List<GameObject> listBall = new List<GameObject>();
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
   
    // Use this for initialization
    void Start()
    {
        GameState = GameState.Prepare;

        ScoreManager.Instance.Reset();
        currentTargetPoint = null;
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


        //
        if (!UIManager.firstLoad)
        {
            StartGame();
            CreateBall();
        }
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(leftFlipperKey))
        {
            _leftFlipperActive = true;
        }
        if (Input.GetKeyDown(rightFlipperKey))
        {
            _rightFlipperActive = true;
        }
        if (Input.GetKeyUp(leftFlipperKey))
        {
            _leftFlipperActive = false;
            _lastRoudnLeftFlipperActive = false;
        }
        if (Input.GetKeyUp(rightFlipperKey))
        {
            _rightFlipperActive = false;
            _lastRoudnRightFlipperActive = false;
        }
        if (!gameOver && !UIManager.firstLoad)
        {
            //Vector3 mousePosition = Input.mousePosition;
            //float halfScreenWidth = Screen.width / 2f;
            //
            //bool rightMouseDown = Input.GetMouseButtonDown(0) && mousePosition.x >= halfScreenWidth;
            //bool leftMouseDown = Input.GetMouseButtonDown(0) && mousePosition.x < halfScreenWidth;
            //bool rightMouseHeld = Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && mousePosition.x >= halfScreenWidth;
            //bool leftMouseHeld = Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && mousePosition.x < halfScreenWidth;
            //
            //bool rightKeyDown = Input.GetKeyDown(KeyCode.D);
            //bool leftKeyDown = Input.GetKeyDown(KeyCode.A);
            //bool rightKeyHeld = Input.GetKey(KeyCode.D) && !Input.GetKeyDown(KeyCode.D);
            //bool leftKeyHeld = Input.GetKey(KeyCode.A) && !Input.GetKeyDown(KeyCode.A);
            //
            //if (rightMouseDown || leftMouseDown || rightKeyDown || leftKeyDown)
            //{
            //    SoundManager.Instance.PlaySound(SoundManager.Instance.flipping);
            //}
            //
            //if (rightMouseDown || rightKeyDown)
            //{
            //    AddTorque(rightFlipperRigid, -torqueForce);
            //}
            //
            //if (leftMouseDown || leftKeyDown)
            //{
            //    AddTorque(leftFlipperRigid, torqueForce);
            //}
            //
            //if (rightMouseHeld || rightKeyHeld)
            //{
            //    AddTorque(rightFlipperRigid, -torqueForce);
            //}
            //
            //if (leftMouseHeld || leftKeyHeld)
            //{
            //    AddTorque(leftFlipperRigid, torqueForce);
            //}
            if (_leftFlipperActive)
            {
                if (!_lastRoudnLeftFlipperActive)
                {
                    _lastRoudnLeftFlipperActive = true;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.flipping);
                }
                AddTorque(leftFlipperRigid, torqueForce);
            }
            if (_rightFlipperActive)
            {
                if (!_lastRoudnRightFlipperActive)
                {
                    _lastRoudnRightFlipperActive = true;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.flipping);
                }
                AddTorque(rightFlipperRigid, -torqueForce);
            }
        }
    }

    /// <summary>
    /// Fire game event, create gold
    /// </summary>
    public void StartGame()
    {
        GameState = GameState.Playing;

        //Enable goldPoint, create gold at that position and start processing
        GameObject targetPoint = targetPointManager.transform.GetChild(UnityEngine.Random.Range(0, targetPointManager.transform.childCount)).gameObject;
        targetPoint.SetActive(true);
        currentTargetPoint = targetPoint;
        Vector2 pos = Camera.main.ScreenToWorldPoint(currentTargetPoint.transform.position);
        currentTarget = Instantiate(targetPrefab, pos, Quaternion.identity) as GameObject;

        StartCoroutine(Processing());
    }

    void GameOver()
    {
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
        GameObject ball = Instantiate(ballPrefab, ballPoint.transform.position, Quaternion.identity) as GameObject;
        listBall.Add(ball);
    }

    /// <summary>
    /// Create gold 
    /// </summary>
    public void CreateTarget()
    {
        if (!gameOver)
        {
            //Stop all processing, disable current gold
            StopAllCoroutines();
            currentTargetPoint.SetActive(false);

            //Random new goldPoint and create new gold, then start processing
            GameObject goldPoint = targetPointManager.transform.GetChild(UnityEngine.Random.Range(0, targetPointManager.transform.childCount)).gameObject;
            while (currentTargetPoint == goldPoint)
            {
                goldPoint = targetPointManager.transform.GetChild(UnityEngine.Random.Range(0, targetPointManager.transform.childCount)).gameObject;
            }
            goldPoint.SetActive(true);
            currentTargetPoint = goldPoint;
            Vector2 goldPos = Camera.main.ScreenToWorldPoint(currentTargetPoint.transform.position);
            currentTarget = Instantiate(targetPrefab, goldPos, Quaternion.identity) as GameObject;
            StartCoroutine(Processing());
        }      
    }

    /// <summary>
    /// Check game over
    /// </summary>
    /// <param name="the ball"></param>
    public void CheckGameOver(GameObject ball)
    {
        //remove the ball from the list
        listBall.Remove(ball);

        //No ball left -> game over
        if (listBall.Count == 0)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            gameOver = true;

            currentTargetPoint.SetActive(false);

            ParticleSystem particle = Instantiate(hitGold, currentTarget.transform.position, Quaternion.identity) as ParticleSystem;
            var main = particle.main;
            main.startColor = currentTarget.gameObject.GetComponent<SpriteRenderer>().color;
            particle.Play();
            Destroy(particle.gameObject, 1f);
            Destroy(currentTarget.gameObject);

            GameOver();           
        }
    }

    /// <summary>
    /// Change background element color, enable obstacles, update processing time
    /// </summary>
    public void CheckAndUpdateValue()
    {
        if (ScoreManager.Instance.Score % scoreToIncreaseDifficulty == 0)
        {
            //Change background element color
            Color color = backgroundColor[UnityEngine.Random.Range(0, backgroundColor.Length)];
            ushapeSpriteRenderer.color = color;
            backgroundSpriteRenderer.color = color;
            fenceSpriteRenderer.color = color;
            leftFlipperSpriteRenderer.color = color;
            rightFlipperSpriteRenderer.color = color;

            //Enable obstacles
            if (obstacleCounter < obstacleManager.transform.childCount)
            {
                obstacleManager.transform.GetChild(obstacleCounter).gameObject.SetActive(true);
                obstacleCounter++;
            }

            //Update processing time
            if (targetAliveTime > minTargetAliveTime)
            {
                targetAliveTime -= targetAliveTimeDecreaseValue;
            }
            else
            {
                targetAliveTime = minTargetAliveTime;
            }
        }

        if (ScoreManager.Instance.Score % scoreToAddedBall == 0)
        {
            CreateBall();
        }
    }

    IEnumerator Processing()
    {
        Image img = currentTargetPoint.GetComponent<Image>();
        img.fillAmount = 0;
        float t = 0;
        while (t < targetAliveTime)
        {
            t += Time.deltaTime;
            float fraction = t / targetAliveTime;
            float newF = Mathf.Lerp(0, 1, fraction);
            img.fillAmount = newF;
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

            currentTargetPoint.SetActive(false);

            ParticleSystem particle = Instantiate(hitGold, currentTarget.transform.position, Quaternion.identity) as ParticleSystem;
            var main = particle.main;
            main.startColor = currentTarget.gameObject.GetComponent<SpriteRenderer>().color;
            particle.Play();
            Destroy(particle.gameObject, 1f);
            Destroy(currentTarget.gameObject);

            GameOver();
        }      
    }
}
