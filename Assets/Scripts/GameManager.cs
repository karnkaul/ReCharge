using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelLoadDelay = 1;
    public Text energyText, levelText;
    public int initEnergyCount = 25;

    private static bool exitReached = false;
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
        playerController = FindObjectOfType<PlayerController>();
        energyText.text = energyCount.ToString();
	}

    public void AddEnergy (int count)
    {
        energyCount += count;
        Invoke("UpdateUI", 0.05f);
    }

    public void UpdateUI ()
    {
        energyText.text = energyCount.ToString();
        levelText.text = "Level " + (level + 1);
    }

    public void LoadNext ()
    {
        DisablePlayerController();
        StartCoroutine(Load());
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

	public void DisablePlayerController ()
    {
        //playerController.UnsubscirbeDelegates();
        playerController.enabled = false;
    }

    public void EnablePlayerController ()
    {
        //playerController.SubscribeDelegates();
        playerController.enabled = true;
    }

    void GameOver()
    {
        DisablePlayerController();
        Debug.Log("Game Over.");
    }

    void Update ()
    {
        if (energyCount <= 0)
        {
            GameOver();
        }
    }
}
