using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingMenuCanvasGO;
    [SerializeField] private string LevelName;
    [Header("FIrst Selected Option")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingMenuFirst;
    [SerializeField] private GameObject UiCover;
    [SerializeField] private Animator[] animators;
    private bool isPaused;

    // Start is called before the first frame update    
    void Start()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingMenuCanvasGO.SetActive(false);
        UiCover.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.MenuOpenCloseInput)
        {
            if (!isPaused)
            {
                Paused();
            }
            else
            {
                UnPause();
            }
        }
    }

    public void Paused()
    {
        isPaused = true;
        Time.timeScale = 0f;
        OpenMainMenu();
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        CloseAllMenu();
    }

    public void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        UiCover.SetActive(true) ;
        foreach (var animator in animators)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        }
        _settingMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingMenuCanvasGO.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_settingMenuFirst);
    }

    public void CloseAllMenu()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var animator in animators)
        {
            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");
        }
        StartCoroutine(CLoseUiCover());
    }

    public void OnSettingPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        UnPause();
    }

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    public void OnRestartPress()
    {
        SceneManager.LoadScene(LevelName);
        UnPause();
    }

    private IEnumerator CLoseUiCover()
    {
        yield return new WaitForSecondsRealtime(1f);
        //UiCover.SetActive(false );
    }
}
