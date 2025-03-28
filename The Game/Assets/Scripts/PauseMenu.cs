using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private bool muteMusic = false;
    [SerializeField] private AudioSource[] audiosToMute;
    [SerializeField] private Animator animator;

    private FirstPersonController controller;

    public bool AutoLockCursor { get; set; } = true;

    private AudioSource audioSource;

    private void Awake()
    {
        if (muteMusic)
        {
            audioSource = Camera.main.GetComponent<AudioSource>();
        }

        controller = FindFirstObjectByType<FirstPersonController>();
    }

    private void Start()
    {
        pauseMenu.enabled = false;
        animator.enabled = true;
    }

    private void Update()
    {
        animator.SetBool("isPaused", pauseMenu.enabled);

        if (Input.GetKeyDown(KeyCode.Escape) && !pauseMenu.enabled)
        {
            EnablePauseMenu();
        }

        if (AutoLockCursor)
        {
            Cursor.lockState = pauseMenu.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void EnablePauseMenu()
    {
        pauseMenu.enabled = !pauseMenu.enabled;
        //Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        //Cursor.lockState = Time.timeScale == 1 ? CursorLockMode.Locked : CursorLockMode.None;
        controller.ChangeRotationSpeed(true);
        controller.ChangeMoveSpeed(true);
        if (muteMusic)
        {
            audioSource.Play();
            PlayOtherAudios(true);
            if (!pauseMenu.enabled)
            {
                audioSource.Stop();
                PlayOtherAudios(false);
            }
        }
    }

    private void PlayOtherAudios(bool shouldPlay)
    {
        for (int i = 0; i < audiosToMute.Length; i++)
        {
            if (shouldPlay)
            {
                audiosToMute[i].Pause();
            }
            else
            {
                audiosToMute[i].Play();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.enabled = false;
        //Time.timeScale = 1;
        //Cursor.lockState = CursorLockMode.Locked;
        controller.ChangeRotationSpeed(false);
        controller.ChangeMoveSpeed(false);
        if (muteMusic)
        {
            audioSource.Stop();
            PlayOtherAudios(false);
        }
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
