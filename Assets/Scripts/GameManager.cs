using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelLoadDelay = 1;
    public Text energyText, levelText, restartText;
    public CanvasGroup gameOverCanvas;
    public int initEnergyCount = 25, startLevel = 0;

    public static bool gameOver = false;

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
        //playerController = FindObjectOfType<PlayerController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        Debug.Log(playerController.name);
        energyText.text = energyCount.ToString();
        gameOverCanvas.alpha = 0;
        level = startLevel;
    }

    public void AddEnergy (int count)
    {
        if (!gameOver)
        {
            energyCount += count;
            Invoke("UpdateUI", 0.1f);
        }
    }

    public void UpdateUI ()
    {
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
    }

	public void DisablePlayerController ()
    {
        //playerController.UnsubscirbeDelegates();
        if (playerController)
            playerController.enabled = false;
    }

    public void EnablePlayerController ()
    {
        //playerController.SubscribeDelegates();
        if(playerController)
            playerController.enabled = true;
    }

    public void GameOver()
    {
        DisablePlayerController();
        gameOver = true;
        gameOverCanvas.alpha = 1;
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
