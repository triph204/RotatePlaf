using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Mỗi lần scene mới load xong, tự fade từ đen -> trong suốt
        StartCoroutine(Fade(1f, 0f));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f)); // fade sang đen
        SceneManager.LoadScene(sceneName);
        // fade-in sẽ tự chạy qua OnSceneLoaded ở trên
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float t = 0f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.alpha = startAlpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
        fadeCanvasGroup.blocksRaycasts = (endAlpha > 0.99f);
    }
}