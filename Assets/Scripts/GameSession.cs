using AssemblyCSharp.Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 1f;
    [SerializeField] AudioClip bgmAudio_Menu;
    [SerializeField] AudioClip bgmAudio_Level;

    private Camera mainCamera;
    private AudioSource audioSource;
    private AudioClip bgmAudio_Active;
    private SceneNumbers currentScene;
    private bool finishedGame;
    private bool showingSecretDoor;

    private void Awake()
    {
        var sessionAmount = FindObjectsOfType<GameSession>().Length;

        if (sessionAmount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        audioSource = GetComponent<AudioSource>();
        bgmAudio_Active = bgmAudio_Menu;
        SceneManager.sceneLoaded += SceneChanged;
        currentScene = (SceneNumbers)SceneManager.GetActiveScene().buildIndex;
        finishedGame = false;
        showingSecretDoor = false;
        FadeInAudio();
    }

    private void PlayBGM(AudioClip bgmAudio)
    {
        audioSource.loop = true;
        audioSource.clip = bgmAudio;
        audioSource.transform.position = mainCamera.transform.position;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private void SceneChanged(Scene newScene, LoadSceneMode sceneMode)
    {
        // This sometimes gets triggered twice, so prevent duplication
        if (currentScene == (SceneNumbers)newScene.buildIndex) return;

        mainCamera = FindObjectOfType<Camera>();
        FadeInAudio();
        DoorScript.OnChangingScenes.AddListener(FadeOutAudio);

        if (currentScene == SceneNumbers.Level_10
            && newScene.buildIndex == (float)SceneNumbers.Menu_Main)
        {
            finishedGame = true;
        }

        if (showingSecretDoor)
        {
            StartCoroutine(ShowSecretDoor());
        }

        UtilityHelper.previousScene = currentScene;
        currentScene = (SceneNumbers)newScene.buildIndex;

    }

    private void FadeInAudio()
    {
        CheckBGMSource();
        PlayBGM(bgmAudio_Active);
        StartCoroutine(FadeAudio(1f));
    }

    private void FadeOutAudio()
    {
        StartCoroutine(FadeAudio(0f));
        DoorScript.OnChangingScenes.RemoveListener(FadeOutAudio);
    }

    public void CutBGMAudio()
    {
        if (audioSource == null) return;

        audioSource.Stop();
    }

    private IEnumerator FadeAudio(float endVolume)
    {
        if (endVolume > 0f)
        {
            while (audioSource.volume < endVolume)
            {
                audioSource.volume += Time.deltaTime / fadeSpeed;
                yield return null;
            }
        }
        else
        {
            while (audioSource.volume > endVolume)
            {
                audioSource.volume -= Time.deltaTime / fadeSpeed;
                yield return null;
            }
        }

        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera = FindObjectOfType<Camera>();
        audioSource.transform.position = mainCamera.transform.position;
        if (showingSecretDoor == false && finishedGame && currentScene == SceneNumbers.Menu_Main)
        {
            showingSecretDoor = true;
            StartCoroutine(ShowSecretDoor());
        }
    }

    private IEnumerator ShowSecretDoor()
    {
        var doors = GameObject.Find("Doors");
        if (doors == null) yield break;
        var children = doors.GetComponentInChildren<Transform>(true);
        if (children == null) yield break;
        foreach (Transform door in children)
        {
            if (door.name == "Door_Secret")
            {
                var gamehint = GameObject.Find("Text_DoubleJumpHint");
                gamehint.SetActive(false);
                var player = GameObject.Find("Player");
                player.GetComponent<Player>().PlayerCanGlide = true;
                player.GetComponent<Player>().PlayerCanFly = true;
                StartCoroutine(FadeInSecretDoor(door.gameObject));
            }
        }

        yield return null;
    }

    private IEnumerator FadeInSecretDoor(GameObject secretDoor)
    {
        var sprite = secretDoor.GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
        secretDoor.SetActive(true);

        while (sprite.color.a < 1f)
        {
            var newAlpha = sprite.color.a + (Time.deltaTime / fadeSpeed);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);
            yield return null;
        }

        yield return null;
    }

    private void CheckBGMSource()
    {
        if (SceneManager.GetActiveScene().name.Contains("Menu"))
            bgmAudio_Active = bgmAudio_Menu;
        else
            bgmAudio_Active = bgmAudio_Level;
    }
}
