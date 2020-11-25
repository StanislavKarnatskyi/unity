using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager Instanse;

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            PlayerPrefs.SetInt(GamePrefs.LastPlayedLevel.ToString(), SceneManager.GetActiveScene().buildIndex);
            PlayerPrefs.SetInt(GamePrefs.LevelPlayed.ToString() + SceneManager.GetActiveScene().buildIndex, 1);
        }
    }

    public void Restart()
    {
        ChangeLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevel()
    {
        ChangeLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ChangeLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}

public enum Scenes
{ 
    MainMenu,
    First,
    Second, 
    Third,
}

public enum GamePrefs
{
    LastPlayedLevel,
    LevelPlayed,
}