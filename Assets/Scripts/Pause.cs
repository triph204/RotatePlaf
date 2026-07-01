using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused = false;

    void Awake()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pauses();
        }
    }

    public void Pauses()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }


}