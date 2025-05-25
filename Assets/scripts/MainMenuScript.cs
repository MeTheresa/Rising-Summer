using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject menuMain;
    [SerializeField] private GameObject menuPlay;
    [SerializeField] private GameObject menuControls;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject backButtonFromPlay;
    [SerializeField] private GameObject backButtonFromControls;

    private MenuControls input;
    private void Awake()
    {
        input = new MenuControls();

        input.Menu.Submit.performed += ctx => OnSubmit();
        input.Menu.GoBack.performed += ctx => OnGoBack();

        ShowMainMenu();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void OnSubmit()
    {
        var selectedGO = EventSystem.current.currentSelectedGameObject;
        if (selectedGO == null) return;

        if (selectedGO == playButton)
        {
            ShowPlayMenu();
            return;
        }
        else if (selectedGO == controlsButton)
        {
            ShowControlsMenu();
            return;
        }
        else if (selectedGO == backButtonFromPlay || selectedGO == backButtonFromControls)
        {
            ShowMainMenu();
            return;
        }

        var button = selectedGO.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }

    private void OnGoBack()
    {
        if (menuPlay.activeSelf || menuControls.activeSelf)
        {
            ShowMainMenu();
        }
    }

    private void ShowMainMenu()
    {
        menuMain.SetActive(true);
        menuPlay.SetActive(false);
        menuControls.SetActive(false);

        StartCoroutine(SetSelectedNextFrame(playButton));
    }

    private void ShowPlayMenu()
    {
        menuMain.SetActive(false);
        menuPlay.SetActive(true);
        menuControls.SetActive(false);

        StartCoroutine(SetSelectedNextFrame(startButton));
    }

    private void ShowControlsMenu()
    {
        menuMain.SetActive(false);
        menuPlay.SetActive(false);
        menuControls.SetActive(true);

        StartCoroutine(SetSelectedNextFrame(backButtonFromControls));
    }

    public void ChangeSceneToPlayScene()
    {
        SceneManager.LoadScene("SceneLevel_1");
    }

    private IEnumerator SetSelectedNextFrame(GameObject button)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
