using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private static PauseMenu _instance;

    private GameObject _menuRoot;
    private float _previousTimeScale = 1f;
    private bool _isOpen;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance != null) return;

        GameObject pauseMenu = new GameObject("PauseMenu");
        pauseMenu.AddComponent<PauseMenu>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        BuildMenu();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_isOpen)
            {
                Time.timeScale = _previousTimeScale;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    public void Continue()
    {
        SetMenuOpen(false);
    }

    public void RestartLevel()
    {
        SetMenuOpen(false);

        if (DeathCounter._instance != null)
        {
            DeathCounter.Instance.ResetCurrentLevel();
        }

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name, LoadSceneMode.Single);
    }

    public void RestartGame()
    {
        SetMenuOpen(false);
        DeathCounter.Instance.ResetAll();
        SceneManager.LoadScene("Level0", LoadSceneMode.Single);
    }

    private void ToggleMenu()
    {
        SetMenuOpen(!_isOpen);
    }

    private void SetMenuOpen(bool open)
    {
        if (_isOpen == open) return;

        _isOpen = open;
        _menuRoot.SetActive(open);

        if (open)
        {
            EnsureEventSystem();
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = _previousTimeScale;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_isOpen)
        {
            SetMenuOpen(false);
        }
    }

    private void BuildMenu()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        _menuRoot = CreateRect("EscapeMenu", transform);
        StretchToParent(_menuRoot.GetComponent<RectTransform>());
        _menuRoot.SetActive(false);

        Image overlay = _menuRoot.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.72f);

        GameObject panel = CreateRect("MenuPanel", _menuRoot.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(420f, 430f);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.08f, 0.1f, 0.96f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(36, 36, 32, 36);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateLabel("Paused", panel.transform, 46f, 70f);
        CreateButton("Continue", panel.transform, Continue, true);
        CreateButton("Restart Level", panel.transform, RestartLevel, true);
        CreateButton("Settings", panel.transform, null, false);
        CreateButton("Restart Game", panel.transform, RestartGame, true);
    }

    private static GameObject CreateRect(string name, Transform parent)
    {
        GameObject rectObject = new GameObject(name, typeof(RectTransform));
        rectObject.transform.SetParent(parent, false);
        return rectObject;
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static void CreateLabel(string text, Transform parent, float fontSize, float height)
    {
        GameObject labelObject = CreateRect(text, parent);
        LayoutElement layoutElement = labelObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
    }

    private static void CreateButton(string text, Transform parent, UnityEngine.Events.UnityAction action, bool interactable)
    {
        GameObject buttonObject = CreateRect(text, parent);
        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 58f;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = interactable
            ? new Color(0.28f, 0.12f, 0.14f, 1f)
            : new Color(0.16f, 0.16f, 0.18f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.interactable = interactable;
        button.targetGraphic = buttonImage;

        ColorBlock colors = button.colors;
        colors.normalColor = buttonImage.color;
        colors.highlightedColor = new Color(0.42f, 0.18f, 0.2f, 1f);
        colors.pressedColor = new Color(0.2f, 0.08f, 0.1f, 1f);
        colors.disabledColor = new Color(0.16f, 0.16f, 0.18f, 0.72f);
        button.colors = colors;

        if (action != null)
        {
            button.onClick.AddListener(action);
        }

        GameObject textObject = CreateRect("Text", buttonObject.transform);
        StretchToParent(textObject.GetComponent<RectTransform>());

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 28f;
        label.color = interactable ? Color.white : new Color(0.62f, 0.62f, 0.66f, 1f);
        label.alignment = TextAlignmentOptions.Center;
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }
}
