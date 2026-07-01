using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        int unlocked = PlayerPrefs.GetInt("unlocked_level", 1);
        Debug.Log("Unlocked level: " + unlocked);

        for (int i = 0; i < buttons.Length; i++)
        {
            int level = i + 1;
            bool isUnlocked = level <= unlocked;

            Debug.Log("Button " + level + " - interactable: " + isUnlocked);

            Transform lockIcon = buttons[i].transform.Find("Lock");
            if (lockIcon != null)
                lockIcon.gameObject.SetActive(!isUnlocked);

            buttons[i].interactable = isUnlocked;
        }
    }
}