using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorPass : MonoBehaviour
{
    [SerializeField] private DoorOpen doorOpen;
    [SerializeField] private int thisLevelNumber;
    public UnityEvent onPlayerPass;

    private bool hasPassed = false;

    private void Awake() => GetComponent<Collider2D>().isTrigger = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hasPassed) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || pm.GravDir != 0) return;
        if (doorOpen == null || !doorOpen.IsOpened) return;

        hasPassed = true;

        // Mở khóa màn tiếp theo
        int current = PlayerPrefs.GetInt("unlocked_level", 1);
        if (thisLevelNumber >= current)
        {
            PlayerPrefs.SetInt("unlocked_level", thisLevelNumber + 1);
            PlayerPrefs.Save();
        }

        StartCoroutine(DelayPass());
    }

    private IEnumerator DelayPass()
    {
        yield return new WaitForSeconds(2f);
        onPlayerPass?.Invoke();
    }
}