using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 2f;
    SpriteRenderer fadeScreen;

    // Start is called before the first frame update
    void Start()
    {
        fadeScreen = GetComponent<SpriteRenderer>();
        fadeScreen.color = new Color(0f, 0f, 0f, 1f);
        StartCoroutine(FadeInScene());
    }

    IEnumerator FadeInScene()
    {
        Time.timeScale = 1;

        while (fadeScreen.color.a > 0f)
        {
            var newAlpha = fadeScreen.color.a - Time.deltaTime / fadeSpeed;
            fadeScreen.color = new Color(0f, 0f, 0f, newAlpha);
            yield return null;
        }

        DoorScript.OnChangingScenes.AddListener(StartFadeOutScene);
        yield return null;
    }

    private void StartFadeOutScene()
    {
        StartCoroutine(FadeOutScene());
    }

    IEnumerator FadeOutScene()
    {
        while (fadeScreen.color.a < 1f)
        {
            var newAlpha = fadeScreen.color.a + Time.deltaTime / fadeSpeed;
            fadeScreen.color = new Color(0f, 0f, 0f, newAlpha);
            yield return null;
        }

        DoorScript.OnChangingScenes.RemoveListener(StartFadeOutScene);
        yield return null;
    }
}
