using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Canvas loadingScreen;
    [SerializeField] private Slider loadingBar;

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.enabled = true;

        while (!async.isDone)
        {
            float loadProgess = Mathf.Clamp01(async.progress / 0.9f);
            loadingBar.value = loadProgess;
            Debug.Log(loadProgess);
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.enabled = true;

        while (!async.isDone)
        {
            float loadProgess = Mathf.Clamp01(async.progress / 0.9f);
            loadingBar.value = loadProgess;
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.enabled = true;

        while (!async.isDone)
        {
            float loadProgess = Mathf.Clamp01(async.progress / 0.9f);
            loadingBar.value = loadProgess;
            Debug.Log(loadProgess);
            yield return null;
        }
    }
}
