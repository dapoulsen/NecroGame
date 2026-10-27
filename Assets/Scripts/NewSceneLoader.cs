using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneLoader : MonoBehaviour
{
    public string sceneName;
    public AudioClip levelCompleteSound;
    private AudioSource _sceneLoaderSound;


    public void Start()
    {
        _sceneLoaderSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(LoadSceneAfterSound());
    }

    private IEnumerator LoadSceneAfterSound()
    {
        _sceneLoaderSound.PlayOneShot(levelCompleteSound, 1f);

        //Wait for sound to finish
        yield return new WaitForSeconds(levelCompleteSound.length - 1.5f);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
