using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DoorPass : MonoBehaviour
{
    public UnityEvent onPlayerPass;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || pm.GravDir != 0) return; // chỉ qua khi gravity xuống

        onPlayerPass?.Invoke();
    }
}