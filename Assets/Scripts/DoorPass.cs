using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DoorPass : MonoBehaviour
{
    [SerializeField] private DoorOpen doorOpen;

    public UnityEvent onPlayerPass;

    private bool hasPassed = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (hasPassed) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || pm.GravDir != 0) return;

        // Chờ cửa mở xong mới cho qua
        if (doorOpen == null || !doorOpen.IsOpened) return;

        hasPassed = true;
        onPlayerPass?.Invoke();
    }
}