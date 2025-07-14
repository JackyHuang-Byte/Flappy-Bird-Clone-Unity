using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LogicScript : MonoBehaviour
{
    public AudioSource BackgroundMusic;
    public AudioSource GameOverSFX;
    public AudioSource ScoreUpSFX;
    public AudioSource ClickSFX;
    public bool GamePaused = false;
    public Button PauseButton;
    public GameObject GameOverScreen;
    public GameObject PausePanel;
    public GameObject SettingScreen;
    public GameObject Canvas;
    public GameObject MainCamera;
    public int PlayerScore;
    public Text ScoreText;
    public Text HighScoreText;
    public Text CurrentScoreText;
    public Text NewHighScoreText;
    public Text NewHighScore;

    private bool _gameOverSFXPlayed = false;
    private bool _isOnSettingMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text = null;
        HighScoreText.text = null;
        Time.timeScale = 1.0f;

        PausePanel.SetActive(true);
        Button[] buttons = Canvas.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.onClick.AddListener(ClickSFX.Play);
        }

        PausePanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameOverSFXPlayed && !_isOnSettingMenu)
        {
            OnClickPause();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _isOnSettingMenu)
        {
            _isOnSettingMenu = false;
        }

        

    }


        [ContextMenu("Increase Score")]
    public void AddScore(int scoreToAdd)
    {
        PlayerScore += scoreToAdd;
        ScoreText.text = PlayerScore.ToString();
        ScoreUpSFX.Play();

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        /*trigger Game Over SFX only once*/
        if (!_gameOverSFXPlayed)
        {
            GameOverSFX.Play();
            _gameOverSFXPlayed = true;
        }

        ScoreText.gameObject.SetActive(false);
        PauseButton.gameObject.SetActive(false);
        PausePanel.SetActive(false);
        GameOverScreen.SetActive(true);

        if (PlayerPrefs.GetInt("Game Score") >= PlayerScore)
        {
            HighScoreText.text = PlayerPrefs.GetInt("Game Score").ToString();
        }
        else
        {
            PlayerPrefs.SetInt("Game Score", PlayerScore);
            HighScoreText.gameObject.SetActive(false);
            CurrentScoreText.gameObject.SetActive(false);
            NewHighScoreText.gameObject.SetActive(true);
            NewHighScore.gameObject.SetActive(true);

        }

        HighScoreText.text = "HIGH SCORE\n" + PlayerPrefs.GetInt("Game Score");
        CurrentScoreText.text = "SCORE\n" + PlayerScore.ToString();
        NewHighScore.text = PlayerScore.ToString();
    }

    [ContextMenu("Reset Player Score")]
    public void ResetPlayerScore()
    {
        PlayerPrefs.SetInt("Game Score", 0);
    }

    public void OnClickPause()
    {
        if (!GamePaused)
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0f;
            //BackgroundMusic.Pause();
            GamePaused = true;
        }
        else
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1f;
            //BackgroundMusic.Play();
            GamePaused = false;
        }
        
    }

    public void OnClickResume()
    {
        GamePaused = true;
        _isOnSettingMenu = false;

        OnClickPause();
    }

    public void OnClickSetting()
    {
        StopAllCoroutines();
        _isOnSettingMenu = true;
        SettingScreen.SetActive(true);
    }

    public void OnClickExit()
    {
        Time.timeScale = 1.0f; //avoid yield return malfunction by timescale
        SceneManager.LoadScene("Title Screen", LoadSceneMode.Single);
    }

    public IEnumerator CameraShake(float duration, float maxMagnitude)
    {
        

        float elapsed = 0;
        float decayPoint = duration / 3; //start ease-in after one/third of total duration
        float magnitude;

        Vector3 cameraOriPos = MainCamera.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            while (GamePaused)
            {
                yield return null;
            }

            if (elapsed < decayPoint)
            {
                magnitude = Mathf.Lerp(0, maxMagnitude, EaseOut(elapsed / decayPoint));

                Debug.Log("Phase #1: " + elapsed + "; Magnitude: " + magnitude);
            }
            else
            {
                float t = (elapsed -  decayPoint) / (duration - decayPoint);

                magnitude = Mathf.Lerp(maxMagnitude, 0, EaseIn(t));

                Debug.Log("Phase #1: " + elapsed + "; Magnitude: " + magnitude);
            }

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            MainCamera.transform.position = new Vector3(cameraOriPos.x + x, cameraOriPos.y + y , cameraOriPos.z);

            yield return null;

        }

        MainCamera.transform.position = cameraOriPos;

        Debug.Log("End of OnClick Event");
    }

    private float EaseOut(float t)
    {
        //Cubric
        t = 1 - t;
        return 1 - EaseIn(t);
    }

    private float EaseIn(float t)
    {
        //Cubric
        return Mathf.Pow(t, 3);
    }
}
