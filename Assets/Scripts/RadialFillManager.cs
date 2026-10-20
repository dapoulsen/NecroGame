using UnityEngine;
using UnityEngine.UI;

public class RadialFillManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject radialRoot;  // the "RadialFill" child
    [SerializeField] Image fillImage;        // the "Fill" grandchild

    [Header("Follow")]
    [SerializeField] Transform target;       // the player (auto-found if empty)
    [SerializeField] Vector3 offset = new Vector3(0f, 1f, 0f);

    void Awake()
    {
        if (target == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }

    public void Show()
    {
        radialRoot.SetActive(true);
        SetProgress(1f);
    }

    public void Hide() => radialRoot.SetActive(false);

    // 1 = full, 0 = empty
    public void SetProgress(float t)
    {
        fillImage.fillAmount = Mathf.Clamp01(t);
    }
}