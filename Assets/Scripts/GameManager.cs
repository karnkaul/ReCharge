using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelLoadDelay = 1;

    private static int level = 0;
    // Singleton
    private static GameManager instance = null;
    private PlayerController playerController;

	void Awake ()
    {
        if (!instance)
            instance = this;

        else if (this != instance)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
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
}
