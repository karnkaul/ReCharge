using UnityEngine;
using System.Collections;

public class Statics : MonoBehaviour
{
    public static float fadeDuration = 1.0f;

    public static IEnumerator FadeIn(Renderer rend)
    {
        rend.material.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(Random.Range(0.0f, fadeDuration));
        float alpha = 0;
        while (alpha < 1)
        {
            rend.material.color = new Color(1, 1, 1, alpha += 0.1f);
            yield return null;
        }
    }
}
