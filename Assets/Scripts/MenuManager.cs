using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Text bestScoreText;

    public bool IsGunMode 
    {
        set
        {
            DataManager.Instance.IsGunMode = value;
        } 
    }
    // Start is called before the first frame update

    void Start()
    {
        IsGunMode = false;
        DataManager.Instance.LoadData();
        DataManager.Instance.LoadSettings();
        bestScoreText.text = "Best score: " + DataManager.Instance.BestScore;
    }
    public void Play()
    {
        SceneManager.LoadSceneAsync("main");
    }
    public void HighScore()
    {
        SceneManager.LoadSceneAsync("highscore");
    }
    public void Settings(GameObject settingsMenu)
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void Quit()
    {
        DataManager.Instance.SaveData();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
