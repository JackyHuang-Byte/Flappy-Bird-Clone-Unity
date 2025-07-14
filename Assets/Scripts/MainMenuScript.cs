using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public AudioSource ClickSFX;
    public GameObject SettingScreen;
    public float DelayTime = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.onClick.AddListener(ClickSFX.Play);
        }

        CanvasScaler[] canvasScalers = FindObjectsOfType<CanvasScaler>();

        foreach (CanvasScaler canvasScaler in canvasScalers)
        {
            canvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

        }

        Debug.Log("Canvas Scaler Found:" + canvasScalers.Length);

        if (!PlayerPrefs.HasKey("InitialStart"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetFloat("Master Volume", 1f);
            PlayerPrefs.SetFloat("Music Volume", 1f);
            PlayerPrefs.SetFloat("SFX Volume", 1f);

            PlayerPrefs.SetInt("InitialStart", 1);
            PlayerPrefs.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayGame()
    {
        StartCoroutine(LoadSceneAfterSFX("Game Screen"));
    }

    public void Setting()
    {
        SettingScreen.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneAfterSFX(string scene)
    {
        Time.timeScale = 1.0f; //avoid yield return & timescale conflict
        ClickSFX.Play();
        yield return new WaitForSeconds(ClickSFX.clip.length + DelayTime);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);

    }

}
