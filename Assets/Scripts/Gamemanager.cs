using UnityEngine;
using UnityEngine.SceneManagement;


public class Gamemanager : MonoBehaviour
{
    public void ChuyenScene(string tenScene)
    {
        SceneManager.LoadScene(tenScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}