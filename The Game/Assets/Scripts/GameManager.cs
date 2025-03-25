using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool lockCursor = false;

    private void Start()
    {
        Time.timeScale = 1;

        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void LoadAScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadAScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }

    public void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public void ChangeScreenMode(TMP_Dropdown dropdown)
    {
        int screenMode = dropdown.value;
        FullScreenMode mode = new();

        switch (screenMode)
        {
            case 0:
                mode = FullScreenMode.ExclusiveFullScreen;
                break;

            case 1:
                mode = FullScreenMode.FullScreenWindow;
                break;

            case 2:
                mode = FullScreenMode.MaximizedWindow;
                break;

            case 3:
                mode = FullScreenMode.Windowed;
                break;

            default:
                break;
        }

        Debug.Log(mode);
        Screen.fullScreenMode = mode;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(audioClip);
    }

    public void ChangeBrightness(Slider brightness)
    {
        Screen.brightness = brightness.value;
    }

    /*
    public void SavePlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Save();
    }

    public void LoadPlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Load();
    }

    public void DeletePlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Delete();
    }
    */
}
