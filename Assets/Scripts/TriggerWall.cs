using Unity.VisualScripting;
using UnityEngine;

public class TriggerWall : MonoBehaviour
{
    public GameObject objectToMove;
    public float objectMoveLenght;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
        {
            objectToMove.transform.Translate(Vector2.down * objectMoveLenght, Space.World);
        }
    }
}
