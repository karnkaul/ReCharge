using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelLoadDelay = 1;
    public int initEnergyCount = 25, startLevel = 0;
    public GameObject popupPrefab;
    [Header("UI References")]
    public Text energyText, levelText, restartText, gameOverText;
    public CanvasGroup gameOverCanvas;

    public static bool gameOver = false, paused = false;
    public static Statics.Void EnablePC, DisablePC, UpdateUI, SetGameOver;
    public static Statics.VoidInt AddEnergy;

    private static int level = 0;
    public static int Level
    {
        get
        {
            return level;
        }
    }
    private static int energyCount;
    private float elapsed, inputDelay = 0.5f;
    private string _restartText = "Press any key to restart\nPress Esc to exit";
    private string _pauseText = "Press Space to resume\nPress Esc to exit";

    // Singleton
    private static GameManager instance = null;

    private IBoardGenerator boardGenerator;
    private IBoardManager boardManager;

    void OnEnable ()
    {
        EnablePC += EnablePlayerController;
        DisablePC += DisablePlayerController;
        UpdateUI += _UpdateUI;
        AddEnergy += _AddEnergy;
        SetGameOver += GameOver;
    }
    void OnDisable()
    {
        EnablePC -= EnablePlayerController;
        DisablePC -= DisablePlayerController;
        UpdateUI -= _UpdateUI;
        AddEnergy -= _AddEnergy;
        SetGameOver -= GameOver;
    }

    void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        if (this != instance)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);

#if !UNITY_ANDROID
        Destroy(GameObject.Find("Touch Canvas"));
#endif

        if (!energyText)
            Debug.Log("Reference to Energy Text missing.");

        boardManager = GameObject.Find("Board").GetComponent<IBoardManager>();
        boardGenerator = GameObject.Find("Board").GetComponent<IBoardGenerator>();

        energyCount = initEnergyCount;
        energyText.text = energyCount.ToString();
        gameOverCanvas.alpha = 0;
        level = startLevel;

#if UNITY_ANDROID
        _restartText = "Tap anywhere to restart\nPress back to exit";
#endif
    }

    void _AddEnergy (int count)
    {
        if (!gameOver)
        {
            energyCount += count;
            int offset = (count > 0) ? -1 : 0;
            if (popupPrefab && count != -1)
                PopupText(count + offset);
            UpdateUI();
        }
    }

    void _UpdateUI ()
    {
        StartCoroutine(DelayedUIUpdate());
    }

    void PopupText (int count)
    {
        string text = (count > 0) ? "+" : "";
        text += count.ToString();
        float r = (count > 0) ? 0 : 0.8f, g = (count > 0) ? 0.8f : 0;
        Color color = new Color(r, g, 0.6f, 1);

        GameObject popup = Instantiate(popupPrefab, GameObject.Find("Player").transform.position, Quaternion.identity) as GameObject;
        Text popupText = popup.transform.Find("Delta Count").GetComponent<Text>();
        popupText.color = color;
        popupText.text = text;
    }

    IEnumerator DelayedUIUpdate ()
    {
        yield return new WaitForSeconds(0.1f);
        energyText.text = energyCount.ToString();
        levelText.text = (level + 1).ToString();
    }

    public void LoadNext ()
    {
        if (!gameOver)
        {
            boardGenerator.StopEnemies();
            DisablePlayerController();
            StartCoroutine(Load());
        }
    }

	IEnumerator Load ()
    {
        yield return new WaitForSeconds(levelLoadDelay);
        ++level;
        boardManager.ResetPlayer();
        boardGenerator.InitBoard(true);
        UpdateUI();
    }

	void DisablePlayerController ()
    {
        PlayerController.disabled = true;
    }

    void EnablePlayerController ()
    {
        PlayerController.disabled = false;
    }

    void GameOver()
    {
        DisablePlayerController();
        gameOver = true;
        gameOverText.text = "Game Over";
        restartText.text = _restartText;
        gameOverCanvas.alpha = 1;
    }

    void TogglePause (bool pause)
    {
        gameOverText.text = "Paused";
        restartText.text = _pauseText;
        gameOverCanvas.alpha = (pause) ? 1 : 0;
        Time.timeScale = (pause) ? 0 : 1;
        if (paused)
            EnablePC();
        else
            DisablePC();
        paused = pause;
    }

    void Restart ()
    {
        gameOver = false;
        level = startLevel;
        energyCount = initEnergyCount;
        gameOverCanvas.alpha = 0;
        boardGenerator.InitBoard(true);
        boardManager.ResetPlayer();
    }

    void Update ()
    {
        elapsed += Time.deltaTime;
        if (inputDelay > 0)
            inputDelay -= Time.unscaledDeltaTime;

        if (!gameOver)
        {
            if (energyCount <= 0)
            {
                boardGenerator.StopEnemies();
                energyCount = 0;
                UpdateUI();
                GameOver();
                elapsed = 0;
                restartText.text = "";
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                inputDelay = 0.5f;
                TogglePause(!paused);
            }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
            if (paused)
#endif
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
        }

        else
        {
            if (elapsed >= levelLoadDelay)
            {
                restartText.text = _restartText;
                UpdateUI();

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }

                else if (Input.anyKeyDown || (inputDelay <= 0 && Input.touchCount > 0))
                {
                    inputDelay = 0.5f;
                    elapsed = 0;
                    Restart();
                }
            }
        }
    }
}
