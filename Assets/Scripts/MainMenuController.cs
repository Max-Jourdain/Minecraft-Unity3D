using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }
    public string gameSceneName = "Game";
    private bool isInitialized = false;

    [SerializeField] private GameObject loadingScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!isInitialized)
        {
            isInitialized = true;
        }
    }

    public void StartGame()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(gameSceneName);
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
