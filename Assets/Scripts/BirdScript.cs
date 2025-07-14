using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D MyRigidbody;
    public AudioSource JumpSFX;
    public AudioSource BackgroundMusic;
    public float FlapStrength;
    public float FloatTime = 1.0f;
    public float SpeedUpStrength = 1.1f; //multiplier
    public float CameraShakeDuration = 1.0f;
    public float CameraShakeMagnitude = 1.0f;
    public bool _birdIsAlive = true;

    private Animator[] _birdAnime;
    private LogicScript _logic;
    private TimeScript _timeManager;
    private float _topDeadZone;
    private float _bottomDeadZone;
    private float _timer;
    private float _pipeOriginalMoveSpeed;
    private float _pipeOriginalSpawnRate;

    // Start is called before the first frame update
    void Start()
    {
        _birdAnime = GetComponentsInChildren<Animator>();
        _logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        _timeManager = GameObject.FindObjectOfType<TimeScript>();
        MyRigidbody.gravityScale = 0.33f; //Openning buffer time

        _topDeadZone = transform.position.y + 40f;
        _bottomDeadZone = transform.position.y - 40f;

        _pipeOriginalMoveSpeed = _timeManager.PipeMoveScript.MoveSpeed;
        _pipeOriginalSpawnRate = _timeManager.PipeSpawnScript.SpawnRate;

        //Debug.Log("initial pipe move speed:" + _pipeOriginalMoveSpeed
        //        + "\n initial pipe spawn rate:" + _pipeOriginalSpawnRate);
        //Debug.Log("Main Camera Position:" +  _logic.MainCamera.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 81;
        //Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;   - Best Supported Refresh Rate
        //Application.targetFrameRate = -1;                                                     - Unlimited Refresh Rate

        if ( _birdIsAlive 
           && !_logic.GamePaused
           && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
           && !EventSystem.current.IsPointerOverGameObject()) //prevent trigger both flap & click-pause
        {
            MyRigidbody.gravityScale = 6.5f;
            MyRigidbody.velocity = Vector2.up * FlapStrength;
            JumpSFX.Play();
            _birdAnime[0].SetBool("isFlap", true);
            _birdAnime[1].SetBool("isFlap", true);
            transform.rotation = Quaternion.Euler(0, 0, 10);
            _timer = 0;

            //StartCoroutine(_logic.CameraShake(CameraShakeDuration, CameraShakeMagnitude));
        }

        if (_timer <= FloatTime)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _birdAnime[0].SetBool("isFlap", false);
            _birdAnime[1].SetBool("isFlap", false);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _timer = 0;

        }


        if (transform.position.y > _topDeadZone || transform.position.y < _bottomDeadZone)
        {
            BirdDown();

            //if (transform.position.y > _topDeadZone)
            //{
            //    Debug.Log("Bird.position.y: " + transform.position.y + ";\n TopDeadZone: " + _topDeadZone);
            //    Debug.Log("Bird.position.y > TopDeadZone");
            //}
            //else if (transform.position.y < _bottomDeadZone)
            //{
            //    Debug.Log("Bird.position.y: " + transform.position.y + ";\n BotDeadZone: " + _bottomDeadZone);
            //    Debug.Log("Bird.position.y < BotDeadZone");
            //}
            //else
            //{
            //    Debug.Log("Something else");
            //}

        }

    }

    private void OnCollisionEnter2D(Collision2D collision) //has physical reaction
    {
        BirdDown();
        _timeManager.PipeMoveScript.MoveSpeed = _pipeOriginalMoveSpeed;
        _timeManager.PipeSpawnScript.SpawnRate = _pipeOriginalSpawnRate;
    }

    private void BirdDown()
    {
        MyRigidbody.gravityScale = 6.5f;
        BackgroundMusic.Stop();
        _logic.GameOver();
        _birdIsAlive = false;
        if (transform.position.y <= _bottomDeadZone)
        {
            gameObject.SetActive(false);
        }
        GetComponent<CircleCollider2D>().enabled = false;

    }

    [ContextMenu("Test Mode (Afterplay)")]
    private void FloatInAir()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) //no physical reaction
    {
        if (collision.gameObject.layer == 6 && _birdIsAlive)
        {
            _logic.AddScore(1);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                StartCoroutine(_logic.CameraShake(CameraShakeDuration, CameraShakeMagnitude));
            }

        }

        if (_logic.PlayerScore < 5) //conditions that trigger speedup
        {

            _timeManager.SpeedUp(SpeedUpStrength);
            _timeManager.PipeMoveScript.MoveSpeed *= SpeedUpStrength;
        }
    }
}
