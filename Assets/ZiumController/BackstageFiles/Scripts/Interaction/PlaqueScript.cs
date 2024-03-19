using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;

public class PlaqueScript : MonoBehaviour
{
    [TextArea(3, 10)]
    public string message; // The unique message for each plaque


    /*private IEnumerator FadeInText()
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);
        float rate = 1.0f / fadeInDuration;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = messageText.color;
            messageText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(0, 1, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FadeOutText()
    {
        float startAlpha = messageText.color.a;
        float rate = 1.0f / fadeOutDuration;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tmpColor = messageText.color;
            messageText.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        messageText.gameObject.SetActive(false);
    }*/
}
