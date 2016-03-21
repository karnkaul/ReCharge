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
    private float elapsed;
    
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

        if (!energyText)
            Debug.Log("Reference to Energy Text missing.");

        boardManager = GameObject.Find("Board").GetComponent<IBoardManager>();
        boardGenerator = GameObject.Find("Board").GetComponent<IBoardGenerator>();

        energyCount = initEnergyCount;
        energyText.text = energyCount.ToString();
        gameOverCanvas.alpha = 0;
        level = startLevel;
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
        restartText.text = "Press any key to restart";
        gameOverCanvas.alpha = 1;
    }

    void TogglePause (bool pause)
    {
        gameOverText.text = "Paused";
        restartText.text = "Press Space/Esc to continue/exit.";
        gameOverCanvas.alpha = (pause) ? 1 : 0;
        Time.timeScale = (pause) ? 0 : 1;
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
                TogglePause(!paused);
            }

            if (paused)
                if (Input.GetKeyDown(KeyCode.Escape))
                    Application.Quit();
        }

        else
        {
            if (elapsed >= levelLoadDelay)
            {
                restartText.text = "Press any key to Restart";
                UpdateUI();

                if (Input.anyKeyDown)
                {
                    elapsed = 0;
                    Restart();
                }
            }
        }
    }
}
