using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : BaseGameMenuController
{
    
    [SerializeField] private Button _restart;
    [SerializeField] private Button _toMenu;

    protected override void Start()
    {
        base.Start();
        _play.onClick.AddListener(OnMenuClicked);
        _restart.onClick.AddListener(_serviceManager.Restart);
        _toMenu.onClick.AddListener(OnMainMenuClicked);        
    }

    protected override void OnDestroy()
    {
        _play.onClick.RemoveListener(OnMenuClicked);
        _restart.onClick.RemoveListener(_serviceManager.Restart);
        _toMenu.onClick.RemoveListener(OnMainMenuClicked);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyUp(KeyCode.Escape))
            OnMenuClicked();
    }

    protected override void OnMenuClicked()
    {
        base.OnMenuClicked();
        Time.timeScale = _menu.activeInHierarchy ? 0 : 1;
    }

    public void OnMainMenuClicked()
    {
        ServiceManager.Instanse.ChangeLevel((int)Scenes.MainMenu);
    }
}
