using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Holder styr på dødsfald i hele spillet og pr. level.
/// Opretter sig selv automatisk, overlever scene-skift og gemmer til disk
/// </summary>
public class DeathCounter : MonoBehaviour
{
    public static DeathCounter _instance;
    public static DeathCounter Instance
    {
        get
        {
            if (_instance == null)
            _instance = new GameObject("DeathCounter").AddComponent<DeathCounter>();
            return _instance;
        }
    }

    public int TotalDeaths {get; private set;}
    public string CurrentLevelId {get; private set;}
    public int LevelDeaths => GetLevelDeaths(CurrentLevelId);

    // UI'et lytter på denne, så den kun opdaterer når noget ændrer sig
    public event Action OnDeathCountChanged;

    private readonly Dictionary<string, int> _levelDeaths = new Dictionary<string, int>();
    private const string SaveKey = "DeathCounterSave";

    [Serializable]
    private class SaveData
    {
        public int total;
        public List<string> levelIds = new List<string>();
        public List<int> levelCounts = new List<int>();
    }

    // Kører automatisk når spillet starter - ingen GameObject nødvendig i scenen
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null)
        {
            var go = new GameObject("Deathcounter");
            go.AddComponent<DeathCounter>();
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Load();

        CurrentLevelId = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Standard: ét level = én scene. Brug SetLevel() hvis et level består af flere scener.
        CurrentLevelId = scene.name;
        OnDeathCountChanged?.Invoke();
    }

    /// <summary>Kald denne hvid dit level-ID ikke er det samme som scenenavnet.</summary>
    public void SetLevel(string levelID)
    {
        CurrentLevelId = levelID;
        OnDeathCountChanged?.Invoke();
    }

    /// <summary> Kaldes fra PlayerController.Die() </summary>
    public void RegisterDeath()
    {
        TotalDeaths++;
        _levelDeaths[CurrentLevelId] = GetLevelDeaths(CurrentLevelId) + 1;

        Save();
        OnDeathCountChanged?.Invoke();
    }

    public int GetLevelDeaths(string levelId)
    {
        return _levelDeaths.TryGetValue(levelId, out int count) ? count : 0;
    }

    /// <summary>Nulstil kun det nuværende level (fx hvis spilleren genstarter det).</summary>
    public void ResetCurrentLevel()
    {
        TotalDeaths -= LevelDeaths;
        _levelDeaths[CurrentLevelId] = 0;
        Save();
        OnDeathCountChanged?.Invoke();
    }

    public void ResetAll()
    {
        TotalDeaths = 0;
        _levelDeaths.Clear();
        Save();
        OnDeathCountChanged?.Invoke();
    }

    void Save()
    {
        var data = new SaveData { total = TotalDeaths };
        foreach (var pair in _levelDeaths)
        {
            data.levelIds.Add(pair.Key);
            data.levelCounts.Add(pair.Value);
        }

        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        var data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey));
        TotalDeaths = data.total;
        _levelDeaths.Clear();
        for (int i = 0; i < data.levelIds.Count; i++)
        {
            _levelDeaths[data.levelIds[i]] = data.levelCounts[i];
        }
    }

}
