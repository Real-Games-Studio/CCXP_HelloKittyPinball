using UnityEngine;
using System.Collections;
using SgLib;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isChecked;

    [SerializeField]
    private float idleSpeedThreshold = 0.1f;
    [SerializeField]
    private float idleDurationBeforeRespawn = 3f;
    private float idleTimer;

    public float MinBallLaunchForce = 47;
    public float MaxBallLaunchForce = 52;
    [SerializeField]
    private float maxSpeed = 25f;

    [SerializeField]
    private float _maxSpeedAfterStart = 25f;
    // Use this for initialization
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
        //transform.position += (Random.value >= 0.5f) ? (new Vector3(0.2f, 0)) : (new Vector3(-0.2f, 0));
        gameObject.SetActive(true);
        ShootUp();
    }

    public void MaxSpeedReset()
    {
        maxSpeed  = _maxSpeedAfterStart;
    }
    public void ShootUp()
    {
        var BallLaunchForce = Random.Range(MinBallLaunchForce, MaxBallLaunchForce);
        _rigidbody2D.AddForce(Vector2.up*BallLaunchForce, ForceMode2D.Impulse);
        idleTimer = 0f;
    }

    void FixedUpdate()
    {
        // Clamp velocity to keep impulse stacking from pushing the ball too fast.
        var velocity = _rigidbody2D.linearVelocity;
        if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            _rigidbody2D.linearVelocity = velocity.normalized * maxSpeed;
        }

        MonitorIdleState();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Dead") && !gameManager.gameOver)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.eploring);
            gameManager.CheckGameOver(gameObject);
            Exploring();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gold") && !gameManager.gameOver)
        {
            Debug.Log(other.name);
            SoundManager.Instance.PlaySound(SoundManager.Instance.hitGold);
            ScoreManager.Instance.AddScore(1);
            gameManager.CheckAndUpdateValue();

            ParticleSystem particle = Instantiate(gameManager.hitGold, other.transform.position, Quaternion.identity) as ParticleSystem;
            var main = particle.main;
            main.startColor = other.gameObject.GetComponent<SpriteRenderer>().color;
            particle.Play();
            Destroy(particle.gameObject, 1f);
            Destroy(other.gameObject);
            //gameManager.CreateTarget();
        }
    }

    /// <summary>
    /// Handle when player die
    /// </summary>
    public void Exploring()
    {
        ParticleSystem particle = Instantiate(gameManager.die, transform.position, Quaternion.identity) as ParticleSystem;
        var main = particle.main;
        main.startColor = spriteRenderer.color;
        particle.Play();
        Destroy(particle.gameObject, 1f);
        Destroy(gameObject);
    }

    private void MonitorIdleState()
    {
        if (gameManager == null || gameManager.gameOver)
        {
            return;
        }

        float speed = _rigidbody2D.linearVelocity.magnitude;
        if (speed <= idleSpeedThreshold)
        {
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= idleDurationBeforeRespawn)
            {
                ForceRespawn();
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }

    private void ForceRespawn()
    {
        idleTimer = 0f;

        if (gameManager != null)
        {
            gameManager.CheckGameOver(gameObject);
        }

        Exploring();
    }

}
