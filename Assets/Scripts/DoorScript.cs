using AssemblyCSharp.Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    [SerializeField] SceneNumbers SceneToLoad;
    [SerializeField] float LevelLoadDelay = 1f;
    [SerializeField] AudioClip audioClipDoorOpen;
    public static UnityEvent OnChangingScenes;

    private void Awake()
    {
        if (OnChangingScenes == null) OnChangingScenes = new UnityEvent();
    }

    internal void EnterDoor()
    {
        if (SceneToLoad >= 0)
            StartCoroutine(LoadNextScene());
        else
            StartCoroutine(QuitApplication());
    }

    IEnumerator LoadNextScene()
    {
        var nextScene = gameObject.name == "Door_Retry" ? UtilityHelper.previousScene : SceneToLoad;
        var camera = FindObjectOfType<Camera>();
        OnChangingScenes.Invoke();
        AudioSource.PlayClipAtPoint(audioClipDoorOpen, camera.transform.position);
        yield return new WaitForSecondsRealtime(LevelLoadDelay);
        SceneManager.LoadScene((int)nextScene, LoadSceneMode.Single);
    }

    IEnumerator QuitApplication()
    {
        OnChangingScenes.Invoke();
        var camera = FindObjectOfType<Camera>();
        AudioSource.PlayClipAtPoint(audioClipDoorOpen, camera.transform.position);
        yield return new WaitForSecondsRealtime(LevelLoadDelay);
        Application.Quit();
    }
}
