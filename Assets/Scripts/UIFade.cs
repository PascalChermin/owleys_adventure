using AssemblyCSharp.Assets.Scripts;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] bool onlyFadeIn = false;
    [SerializeField] bool showAfterDeath = false;
    TextMeshProUGUI textFade;
    bool PlayerInCollision = false;

    // Start is called before the first frame update
    void Start()
    {
        textFade = GetComponent<TextMeshProUGUI>();
        textFade.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (showAfterDeath && UtilityHelper.previousScene != SceneNumbers.Menu_GameOver) return;
        PlayerInCollision = true;
        StartCoroutine(FadeInText());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlyFadeIn == false)
        {
            PlayerInCollision = false;
            StartCoroutine(FadeOutText());
        }
    }

    private IEnumerator FadeInText()
    {
        while (textFade.alpha < 1f && PlayerInCollision)
        {
            textFade.alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
    }

    private IEnumerator FadeOutText()
    {
        while (textFade.alpha > 0f && !PlayerInCollision)
        {
            textFade.alpha -= Time.deltaTime / fadeSpeed;
            yield return null;
        }
    }
}
