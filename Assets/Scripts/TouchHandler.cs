using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchHandler : MonoBehaviour
{
    [Range(0.25f, 0.85f)]
    public float targetAlpha = 0.5f;
    public float fadeDuration = 5.0f;
    
    void Start ()
    {
        StartCoroutine(FadeButtons());
    }

    IEnumerator FadeButtons ()
    {
        CanvasGroup image = GetComponent<CanvasGroup>();
        image.alpha = 1;
        float elapsed = 0;
        float delta = (1 - targetAlpha) / fadeDuration;

        while (elapsed <= fadeDuration)
        {
            elapsed += Time.deltaTime;
            image.alpha -= (delta * Time.deltaTime);
            yield return null;  
        }
        image.alpha = targetAlpha;
    }

    public void Left()
    {
        EventHandler.Instance.HandleTaps(-1, 0);
    }

    public void Right()
    {
        EventHandler.Instance.HandleTaps(1, 0);
    }

    public void Up()
    {
        EventHandler.Instance.HandleTaps(0, 1);
    }

    public void Down()
    {
        EventHandler.Instance.HandleTaps(0, -1);
    }
}
