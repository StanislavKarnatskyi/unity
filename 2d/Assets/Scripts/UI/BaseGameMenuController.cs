using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseGameMenuController : MonoBehaviour
{
    protected ServiceManager _serviceManager;
    protected UIAudioManager _audioManager;

    [SerializeField] protected GameObject _menu;

    [Header("MainButtons")]
    [SerializeField] protected Button _play;
    [SerializeField] protected Button _setting;
    [SerializeField] protected Button _quit;

    [Header("Settings")]
    [SerializeField] protected GameObject _settingsMenu;
    [SerializeField] protected Button _closeSettings;

    protected virtual void Start()
    {
        _serviceManager = ServiceManager.Instanse;
        _audioManager = UIAudioManager.Instance;
        _quit.onClick.AddListener(OnQuitClicked);
        _setting.onClick.AddListener(OnSettingsClicked);
        _closeSettings.onClick.AddListener(OnSettingsClicked);
    }

    protected virtual void OnDestroy()
    {
        _quit.onClick.RemoveListener(OnQuitClicked);
        _setting.onClick.RemoveListener(OnSettingsClicked);
        _closeSettings.onClick.RemoveListener(OnSettingsClicked);
    }

    protected virtual void Update()    {     }

    protected virtual void OnMenuClicked()
    {
        _menu.SetActive(!_menu.activeInHierarchy);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit");
        _audioManager.Play(UIClipNames.Quit);
    }

    private void OnSettingsClicked()
    {
        _audioManager.Play(UIClipNames.Settings);
        _settingsMenu.SetActive(!_settingsMenu.activeInHierarchy);
    }
}