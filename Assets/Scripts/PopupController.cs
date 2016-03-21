using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupController : MonoBehaviour
{
    [SerializeField][Range(0.5f, 2.5f)]
    private float speed = 1;

    Text text;
    RectTransform rect;

    void Start ()
    {
        StartCoroutine(FadeToDeath());
        rect = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();

        StartCoroutine(FadeToDeath());
    }

    IEnumerator FadeToDeath ()
    {
        if (!text)
            text = GetComponentInChildren<Text>();
        if (!rect)
            rect = GetComponent<RectTransform>();
        float alphaDelta = -0.25f, yDelta = 0.0f;
        while (text.color.a > 0.1f)
        {
            if (text.color.a < 0.6f)
            {
                yDelta = 0.05f;
                alphaDelta = -0.75f;
            }
            rect.position += new Vector3(-(0.02f + Random.Range(-0.02f, 0.04f)) * speed, (0.1f + yDelta) * speed, 0);
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + alphaDelta * speed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

}
