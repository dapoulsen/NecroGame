using TMPro;
using UnityEngine;

/// <summary>
/// Læg på¨et TextMestPro UI-element (Canvas). Viser Deaths i level og i alt
/// </summary>
public class DeathCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    void Awake()
    {
        if (label == null) label =  GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        Refresh(); 
        var counter = DeathCounter.Instance;
        if (counter == null) return;

        counter.OnDeathCountChanged += Refresh;
        Refresh(); 
    }

    void OnDisable()
    {
        if (DeathCounter.Instance != null)
        {
            DeathCounter.Instance.OnDeathCountChanged -= Refresh;
        }
    }

    void Refresh()
    {
        var c = DeathCounter.Instance;
        label.text = $"Deaths This Level: {c.LevelDeaths}\nTotal Deaths: {c.TotalDeaths}";
    }
}
