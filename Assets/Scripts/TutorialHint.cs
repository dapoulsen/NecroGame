using Unity.VisualScripting;
using UnityEngine;

public class TutorialHint : MonoBehaviour
{
    public GameObject hintObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hintObject != null)
        {
            hintObject.SetActive(false);
        }   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintObject.SetActive(false);
        }
    }
}
