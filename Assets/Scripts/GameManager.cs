using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelLoadDelay = 1;
    public Text energyText, levelText, restartText, gameOverText;
    public CanvasGroup gameOverCanvas;
    public int initEnergyCount = 25, startLevel = 0;

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
    private static GameManager instance = null;
    private PlayerController playerController;
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
        playerController = FindObjectOfType<PlayerController>();
        //playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        energyText.text = energyCount.ToString();
        gameOverCanvas.alpha = 0;
        level = startLevel;
    }

    void _AddEnergy (int count)
    {
        if (!gameOver)
        {
            energyCount += count;
            UpdateUI();
        }
    }

    void _UpdateUI ()
    {
        StartCoroutine(DelayedUIUpdate());
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        boardGenerator.InitBoard();
        boardManager.ResetPlayer();
        UpdateUI();
    }

	void DisablePlayerController ()
    {
        //PlayerController.DisableMovement();
        if (playerController)
            playerController.enabled = false;
    }

    void EnablePlayerController ()
    {
        //PlayerController.EnableMovement();
        //playerController.SubscribeDelegates();
        if(playerController)
            playerController.enabled = true;
    }

    public void GameOver()
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
        restartText.text = "Press Space to continue";
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
        boardGenerator.InitBoard();
        boardManager.ResetPlayer();
    }

    void Update ()
    {
        if (!gameOver)
        {
            if (energyCount <= 0)
            {
                GameOver();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //if (!paused)
                //{
                //    Time.timeScale = 0;
                //    paused = true;
                //    Debug.Log("paused?");
                //}
                //else
                //{
                //    Time.timeScale = 1;
                //    paused = false;
                //}
                TogglePause(!paused);
            }
        }

        else
        {
            if (Input.anyKeyDown)
            {
                Restart();
            }
        }
    }
}
