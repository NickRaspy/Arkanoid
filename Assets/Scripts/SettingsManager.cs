using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public struct Resolution
{
    public int x;
    public int y;
    public Resolution(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [Header("Dropdowns")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown windowModeDropdown;
    [SerializeField] private Dropdown gameSpeedDropdown;
    [SerializeField] private Slider volumeSlider;

    [Space]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private Button saveButton;
    [SerializeField] private List<Resolution> resolutions = new();

    private int x, y;
    private FullScreenMode fsm; 
    private float gameSpeed;
    private float volume;
    void Start()
    {
        DataManager.Instance.LoadSettings();
        SettingsData settingsData = DataManager.Instance.SettingsData;
        GetData(settingsData);
        List<Dropdown.OptionData> optionDatas = new();
        for (int i = 0; i < resolutions.Count; i++)
        {
            optionDatas.Add(new Dropdown.OptionData($"{resolutions[i].x}X{resolutions[i].y}"));
        }
        resolutionDropdown.AddOptions(optionDatas);
        int r = resolutions.FindIndex(r => r.y == settingsData.y && r.x == settingsData.x);
        Debug.Log(r);
        resolutionDropdown.value = r;
        switch (settingsData.fsm)
        {
            case FullScreenMode.Windowed:
                windowModeDropdown.value = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                windowModeDropdown.value = 1;
                break;
            case FullScreenMode.ExclusiveFullScreen:
                windowModeDropdown.value = 2;
                break;
        }
        switch (settingsData.gameSpeed)
        {
            case 1f:
                gameSpeedDropdown.value = 0;
                break;
            case 1.25f:
                gameSpeedDropdown.value = 1;
                break;
            case 1.5f:
                gameSpeedDropdown.value = 2;
                break;
        }
        VolumeChange(settingsData.volume);
        volumeSlider.value = volume;
        saveButton.gameObject.SetActive(false);
    }
    public void ResolutionChange(int dropdownValue)
    {
        x = resolutions[dropdownValue].x;
        y = resolutions[dropdownValue].y;
    }
    public void FSMChange(int dropdownValue)
    {
        switch (dropdownValue)
        {
            case 0:
                fsm = FullScreenMode.Windowed;
                break;
            case 1:
                fsm = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                fsm = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }
    public void GameSpeedChange(int dropdownValue)
    {
        switch (dropdownValue)
        {
            case 0:
                gameSpeed = 1f;
                break;
            case 1:
                gameSpeed = 1.25f;
                break;
            case 2:
                gameSpeed = 1.5f;
                break;
        }
    }
    public void VolumeChange(float volume)
    {
        this.volume = volume;
        masterMixer.SetFloat("Volume", volume);
    }
    public void Menu(GameObject mainMenu)
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void Save()
    {
        DataManager.Instance.SaveSettings(x, y, fsm, gameSpeed, volume);
        DataManager.Instance.LoadSettings();
        saveButton.gameObject.SetActive(false);
    }
    public void ShowSave()
    {
        saveButton.gameObject.SetActive(true);
    }
    public void Wipe()
    {
        DataManager.Instance.WipeData();
    }
    public void Default()
    {
        DataManager.Instance.DefaultSettings();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void GetData(SettingsData settingsData)
    {
        x = settingsData.x;
        y = settingsData.y;
        fsm = settingsData.fsm;
        gameSpeed = settingsData.gameSpeed;
        volume = settingsData.volume;
    }
}

