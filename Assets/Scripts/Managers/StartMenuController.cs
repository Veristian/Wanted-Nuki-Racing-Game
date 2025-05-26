using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingMenuCanvasGO;
    [SerializeField] private GameObject _creditMenuCanvasGO;
    [SerializeField] private GameObject _graphicMenuCanvasGO;
    [SerializeField] private GameObject _ControllMenuCanvasGO;
    [SerializeField] private string LevelName;
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingMenuFirst;
    [SerializeField] private GameObject _creditMenuFirst;
    [SerializeField] private GameObject _graphicMenuFirst;
    [SerializeField] private GameObject _ControllMenuFirst;
    [SerializeField] private Animator[] CarAnimators;
    [SerializeField] private Animator _creditMenuAnimator;
    [SerializeField] private Animator _graphicMenuAnimator;
    [SerializeField] private ParticleSystem Burning;
    // Start is called before the first frame update
    void Start()
    {
        _settingMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            OpenMainMenu();
        }
    }

    private void OpenSettingsMenuHandle()
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingMenuCanvasGO.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_settingMenuFirst);
    }

    private void OpenCreditMenu()
    {
        _mainMenuCanvasGO.SetActive(false);
        _creditMenuCanvasGO.SetActive(true);
        _creditMenuAnimator.SetTrigger("Open");
        _creditMenuAnimator.ResetTrigger("Close");
        EventSystem.current.SetSelectedGameObject(_creditMenuFirst);
    }

    public void OnSettingPress()
    {
        OpenSettingsMenuHandle();
    }
    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }
    public void OnCreditPress()
    {
        OpenCreditMenu();
    }

    public void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        _settingMenuCanvasGO.SetActive(false);
        _creditMenuAnimator.ResetTrigger("Open");
        _creditMenuAnimator.SetTrigger("Close");
        _graphicMenuAnimator.ResetTrigger("Open");
        _creditMenuCanvasGO.SetActive(false) ;
        _ControllMenuCanvasGO.SetActive(false);
        _graphicMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    public void OnStartPress()
    {
        foreach (var animator in CarAnimators)
        {
            animator.SetTrigger("Start");
            //StartCoroutine(StartLevel());
        }
        Burning.Play();
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(LevelName);  
    }

    public void OnGraphicPress()
    {
        _settingMenuCanvasGO.SetActive(false );
        _graphicMenuCanvasGO.SetActive(true);
        _graphicMenuAnimator.SetTrigger("Open");
        EventSystem.current.SetSelectedGameObject(_graphicMenuFirst);
    }

    public void OnGraphicBackPress()
    {
        _settingMenuCanvasGO.SetActive(true);
        _graphicMenuCanvasGO.SetActive(false);
        _graphicMenuAnimator.ResetTrigger("Open");
        EventSystem.current.SetSelectedGameObject(_settingMenuFirst);
    }

    public void OnControlPress()
    {
        _settingMenuCanvasGO.SetActive(false ) ;
        _ControllMenuCanvasGO.SetActive(true) ;
        EventSystem.current.SetSelectedGameObject(_ControllMenuFirst);
    }

    public void OnControlBackPress()
    {
        _settingMenuCanvasGO.SetActive(true);
        _ControllMenuCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_settingMenuFirst);
    }
}
