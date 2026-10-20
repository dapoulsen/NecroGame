using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneLoader : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
