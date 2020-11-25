using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
    private Button _button;  
    [SerializeField] private Scenes scene;

    void Start()
    {
        _button = GetComponent<Button>();
        if (!PlayerPrefs.HasKey(GamePrefs.LevelPlayed.ToString() + ((int)scene).ToString()))
        {
            _button.interactable = false;
            return;
        }

        _button.onClick.AddListener(OnChangeLevelClicked);

        GetComponentInChildren<TMP_Text>().text = ((int)scene).ToString();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    void Update()
    {
        
    }

    void OnChangeLevelClicked()
    {
        ServiceManager.Instanse.ChangeLevel((int)scene);
    }
}
