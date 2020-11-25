using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : BaseGameMenuController
{
    [SerializeField] private Button _chooseLevel;
    [SerializeField] private Button _reset;

    [SerializeField] private GameObject _levelMenu;
    [SerializeField] private Button _closeLevelMenu;

    int level = 1;

    protected override void Start()
    {
        base.Start();
        _chooseLevel.onClick.AddListener(OnLevelMenuClicked);
        _closeLevelMenu.onClick.AddListener(OnLevelMenuClicked);
        if (PlayerPrefs.HasKey(GamePrefs.LastPlayedLevel.ToString()))
        {
            _play.GetComponentInChildren<TMP_Text>().text = "Resume";
            level = PlayerPrefs.GetInt(GamePrefs.LastPlayedLevel.ToString());
        }
        _play.onClick.AddListener(OnPlayClicked);
        _reset.onClick.AddListener(OnResetClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _chooseLevel.onClick.RemoveListener(OnLevelMenuClicked);
        _closeLevelMenu.onClick.RemoveListener(OnLevelMenuClicked);
        _play.onClick.RemoveListener(OnPlayClicked);
        _reset.onClick.RemoveListener(_serviceManager.ResetAll);
    }

    private void OnLevelMenuClicked()
    {
        _levelMenu.SetActive(!_levelMenu.activeInHierarchy);
        OnMenuClicked();
    }

    private void OnPlayClicked()
    {
        _serviceManager.ChangeLevel(level);
        _audioManager.Play(UIClipNames.Play);
    }

    private void OnResetClicked()
    {
        _play.GetComponentInChildren<TMP_Text>().text = "Play";
        _serviceManager.ResetAll();
    }
}
