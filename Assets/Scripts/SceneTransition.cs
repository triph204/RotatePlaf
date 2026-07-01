using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    public static event System.Action OnTransitionComplete;

    [SerializeField] private GameObject canvasRoot;
    [SerializeField] private Image overlayImage;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    // True khi LoadScene đang xử lý fade-out — PlayerMovement.Start() không cần PlayIntro
    public bool IsTransitioning { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (canvasRoot != null) DontDestroyOnLoad(canvasRoot);
        SetAlpha(0f);
    }

    // ── Gọi từ DoorPass ───────────────────────────────────────────────────────
    public void LoadScene(string sceneName)
    {
        if (IsTransitioning) return;
        StartCoroutine(LoadRoutine(sceneName));
    }

    // ── Gọi từ PlayerMovement.Start() — chỉ khi KHÔNG có transition đang chạy
    public void PlayIntro()
    {
        StartCoroutine(IntroRoutine());
    }

    // ── Routines ──────────────────────────────────────────────────────────────

    private IEnumerator LoadRoutine(string sceneName)
    {
        IsTransitioning = true;

        // Fade tối
        yield return Fade(0f, 1f, fadeInDuration);

        // Load scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) yield return null;
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        // Chờ 1 frame để scene render
        yield return null;

        // Fade sáng — FadeRoutine tự xử lý, PlayerMovement.Start() sẽ bỏ qua PlayIntro
        yield return Fade(1f, 0f, fadeOutDuration);

        IsTransitioning = false;
        OnTransitionComplete?.Invoke();
    }

    // Chỉ dùng cho lần đầu mở game và respawn
    private IEnumerator IntroRoutine()
    {
        SetAlpha(1f);
        yield return null;
        yield return Fade(1f, 0f, fadeOutDuration);
        OnTransitionComplete?.Invoke();
    }

    // ── Fade ──────────────────────────────────────────────────────────────────

    private IEnumerator Fade(float from, float to, float duration)
    {
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
        {
            SetAlpha(Mathf.Lerp(from, to, EaseInOut(Mathf.Clamp01(t))));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (overlayImage == null) return;
        overlayImage.color = new Color(0f, 0f, 0f, a);
    }

    private static float EaseInOut(float t)
        => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
}