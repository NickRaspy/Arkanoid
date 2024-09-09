using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct SettingsData
{
    public int x; public int y; public FullScreenMode fsm; [Min(1)] public float gameSpeed; [Range(-80f, 0f)]public float volume;
    public SettingsData(int x, int y, FullScreenMode fsm, float gameSpeed, float volume)
    {
        this.x = x; this.y = y; this.fsm = fsm; this.gameSpeed = gameSpeed; this.volume = volume;
    }
}
[Serializable]
public class Data
{
    public int Score;
}
[Serializable]
public class Settings
{
    public int ScreenX;
    public int ScreenY;
    public FullScreenMode FullScreenMode;
    [Min(1)]public float gameSpeed;
}
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public int Score { get; set; } //to share between levels

    public int BestScore {get; set;}
    public bool IsGunMode { get; set;}

    public Data Data { get; set; }

    public SettingsData SettingsData { get; set; }

    [SerializeField] private SettingsObject defaultSettings;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveData()
    {
        UpdateData(BestScore);

        string json = JsonUtility.ToJson(Data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (JsonUtility.FromJson<Data>(json) != null)
            {
                Data = JsonUtility.FromJson<Data>(json);
                BestScore = Data.Score;
            }
        }
    }
    public void SaveSettings(int x, int y, FullScreenMode fsm, float gameSpeed, float volume)
    {
        SettingsData settings = new()
        {
            x = x,
            y = y,
            fsm = fsm,
            gameSpeed = gameSpeed,
            volume = volume
        };
        string json = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.persistentDataPath + "/settings.json", json);
    }
    public void LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SettingsData = JsonUtility.FromJson<SettingsData>(json);
            Screen.SetResolution(SettingsData.x, SettingsData.y, SettingsData.fsm);
        }
        else
        {
            DefaultSettings();
        }
    }
    public void WipeData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            BestScore = 0;
        }
    }
    public void DefaultSettings()
    {
        SettingsData = defaultSettings.settings;
        string json = JsonUtility.ToJson(SettingsData);
        File.WriteAllText(Application.persistentDataPath + "/settings.json", json);
    }
    private void UpdateData(int score)
    {
        if(Data != null) Data.Score = score;
        else 
            Data = new()
            {
                Score = score
            };
    }
}
