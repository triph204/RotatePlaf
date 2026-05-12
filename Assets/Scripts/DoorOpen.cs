using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorOpen : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private bool isOpened = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (doorAnimator != null)
            doorAnimator.enabled = false; // tắt animator lúc đầu
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;
        if (!other.CompareTag("Player")) return;

        isOpened = true;
        doorAnimator.enabled = true;
  
    }
}