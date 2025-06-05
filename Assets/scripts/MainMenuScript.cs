using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject menuMain;
    [SerializeField] private GameObject menuPlay;
    [SerializeField] private GameObject menuControls;
    [SerializeField] private GameObject menuSelection;

    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject backButtonFromPlay;
    [SerializeField] private GameObject backButtonFromControls;

    public static int ClassP1;
    public static int ClassP2;
    [SerializeField] private Image _p1Idle;
    [SerializeField] private TMP_Text _p1SizeText;
    private Vector2 _originalP1Size;
    [SerializeField] private TMP_Text _p1StatsText;
    [SerializeField] private Image _p2Idle;
    [SerializeField] private TMP_Text _p2SizeText;
    [SerializeField] private TMP_Text _p2StatsText;
    private Vector2 _originalP2Size;

    private MenuControls input;
    private void Awake()
    {
        input = new MenuControls();

        input.Menu.Submit.performed += ctx => OnSubmit();
        input.Menu.GoBack.performed += ctx => OnGoBack();

        ShowMainMenu();
        _originalP1Size = _p1Idle.rectTransform.sizeDelta;
        _p1SizeText.text = "Normal";
        _p1StatsText.text = "1x\n1x\n1x";
        _originalP2Size = _p2Idle.rectTransform.sizeDelta;
        _p2SizeText.text = "Normal";
        _p2StatsText.text = "1x\n1x\n1x";
        ClassP1 = 0;
        ClassP2 = 0;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    #region Menu Navigation
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
    #endregion
    #region Menu UI
    #region Menu enabling/disabling
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
        SceneManager.LoadScene(1);
    }
    #endregion
    #region Character Selection
    public void CheckP1Class(string Choice) // 0 = normal, 1 = bigger, 2 = smaller
    {
        if (Choice == "Left")
        {
            switch (ClassP1)
            {
                case 0: //smaller <-- normal || class = 0 -> 2
                    {
                        ClassP1 = 2;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size / 1.2f;
                        _p1SizeText.text = "Smaller";
                        _p1StatsText.text = "0.8x\n0.8x\n1.2x"; //Size, strength, speed
                        break;
                    }
                case 1: //normal <-- bigger || class = 1 -> 0
                    {
                        ClassP1--;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size;
                        _p1SizeText.text = "Normal";
                        _p1StatsText.text = "1x\n1x\n1x"; //Size, strength, speed
                        break;
                    }
                case 2: //bigger <-- smaller || class = 2 -> 1
                    {
                        ClassP1--;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size * 1.2f;
                        _p1SizeText.text = "Bigger";
                        _p1StatsText.text = "1.2x\n1.2x\n0.8x"; //Size, strength, speed
                        break;
                    }
            }
        }
        if (Choice == "Right")
        {
            switch (ClassP1)
            {
                case 0: //normal --> bigger || class = 0 -> 1
                    {
                        ClassP1++;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size * 1.2f;
                        _p1SizeText.text = "Bigger";
                        _p1StatsText.text = "1.2x\n1.2x\n0.8x"; //Size, strength, speed
                        break;
                    }
                case 1: //bigger --> smaller || class = 1 -> 2
                    {
                        ClassP1++;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size / 1.2f;
                        _p1SizeText.text = "Smaller";
                        _p1StatsText.text = "0.8x\n0.8x\n1.2x"; //Size, strength, speed
                        break;
                    }
                case 2: //smaller --> normal || class = 2 -> 0
                    {
                        ClassP1 = 0;
                        _p1Idle.rectTransform.sizeDelta = _originalP1Size;
                        _p1SizeText.text = "Normal";
                        _p1StatsText.text = "1x\n1x\n1x"; //Size, strength, speed
                        break;
                    }
            }
        }
    }
    public void CheckP2Class(string Choice) // 0 = normal, 1 = bigger, 2 = smaller
    {
        if (Choice == "Left")
        {
            switch (ClassP2)
            {
                case 0: //smaller <-- normal || class = 0 -> 2
                    {
                        ClassP2 = 2;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size / 1.2f;
                        _p2SizeText.text = "Smaller";
                        _p2StatsText.text = "0.8x\n0.8x\n1.2x"; //Size, strength, speed
                        break;
                    }
                case 1: //normal <-- bigger || class = 1 -> 0
                    {
                        ClassP2--;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size;
                        _p2SizeText.text = "Normal";
                        _p2StatsText.text = "1x\n1x\n1x"; //Size, strength, speed
                        break;
                    }
                case 2: //bigger <-- smaller || class = 2 -> 1
                    {
                        ClassP2--;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size * 1.2f;
                        _p2SizeText.text = "Bigger";
                        _p2StatsText.text = "1.2x\n1.2x\n0.8x"; //Size, strength, speed
                        break;
                    }
            }
        }
        if (Choice == "Right")
        {
            switch (ClassP2)
            {
                case 0: //normal --> bigger || class = 0 -> 1
                    {
                        ClassP2++;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size * 1.2f;
                        _p2SizeText.text = "Bigger";
                        _p2StatsText.text = "1.2x\n1.2x\n0.8x"; //Size, strength, speed
                        break;
                    }
                case 1: //bigger --> smaller || class = 1 -> 2
                    {
                        ClassP2++;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size / 1.2f;
                        _p2SizeText.text = "Smaller";
                        _p2StatsText.text = "0.8x\n0.8x\n1.2x"; //Size, strength, speed
                        break;
                    }
                case 2: //smaller --> normal || class = 2 -> 0
                    {
                        ClassP2 = 0;
                        _p2Idle.rectTransform.sizeDelta = _originalP2Size;
                        _p2SizeText.text = "Normal";
                        _p2StatsText.text = "1x\n1x\n1x"; //Size, strength, speed
                        break;
                    }
            }
        }
    }
    #endregion
    #endregion
    private IEnumerator SetSelectedNextFrame(GameObject button)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
