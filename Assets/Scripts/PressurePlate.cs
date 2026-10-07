using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("What to move")]
    public Transform objectToMove;
    public Vector2 moveOffset = new Vector2(0f, -3f);
    public float moveSpeed = 4f;

    [Header("The plate itself")]
    public Transform plateVisual;
    public float pressDepth = 0.1f;
    public float plateSpeed = 2f;

    [Header("Who can activate the plate")]
    public string[] activatorTags = { "Player", "Body" };

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Vector3 _plateUpPosition;
    private Vector3 _plateDownPosition;
    private int _objectsOnPlate;

    void Start()
    {
        _closedPosition = objectToMove.position;
        _openPosition = _closedPosition + (Vector3)moveOffset;

        _plateUpPosition = plateVisual.localPosition;
        _plateDownPosition = _plateUpPosition + Vector3.down * pressDepth;
    }

    void Update()
    {
        bool pressed = _objectsOnPlate > 0;

        // The wall/door
        objectToMove.position = Vector3.MoveTowards(
            objectToMove.position,
            pressed ? _openPosition : _closedPosition,
            moveSpeed * Time.deltaTime);

        // The plate
        plateVisual.localPosition = Vector3.MoveTowards(
            plateVisual.localPosition,
            pressed ? _plateDownPosition : _plateUpPosition,
            plateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActivator(other))
            _objectsOnPlate++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsActivator(other))
            _objectsOnPlate = Mathf.Max(0, _objectsOnPlate - 1);
    }

    bool IsActivator(Collider2D other)
    {
        foreach (string tag in activatorTags)
        {
            if (other.CompareTag(tag))
                return true;
        }
        return false;
    }
}