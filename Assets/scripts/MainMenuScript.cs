using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject _menuMain;
    void Awake()
    {
        if (_menuMain.activeSelf == false)
        {
            Debug.Log("Not active");
            _menuMain.SetActive(true);
            Debug.Log("Active");
        }
    }
    
    public void ChangeSceneToPlayScene()
    {
        SceneManager.LoadScene("SceneLevel_1");
    }
}
