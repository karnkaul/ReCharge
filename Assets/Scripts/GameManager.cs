using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelLoadDelay = 1;
    public Text energyText;
    public int initEnergyCount = 25;

    private static int level = 0;
    private static int energyCount;
    private static GameManager instance = null;
    private PlayerController playerController;

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
    }
	IEnumerator Load ()
    {
        yield return new WaitForSeconds(levelLoadDelay);
        ++level;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void DisablePlayerController ()
    {
        playerController.UnsubscirbeDelegates();
    }

    public void EnablePlayerController ()
    {
        playerController.SubscribeDelegates();
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
