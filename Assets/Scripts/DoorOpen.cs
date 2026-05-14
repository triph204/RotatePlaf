using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorOpen : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";

    public bool IsOpened { get; private set; } = false;
    private bool isPlaying = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (doorAnimator != null)
            doorAnimator.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsOpened || isPlaying) return;
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null || pm.GravDir != 0) return;

        StartCoroutine(PlayOpenAnimation());
    }

    private IEnumerator PlayOpenAnimation()
    {
        isPlaying = true;

        doorAnimator.enabled = true;
        doorAnimator.SetTrigger(openTriggerName);

        // Chờ 1 frame để animator cập nhật state mới
        yield return null;

        // Chờ hết độ dài animation
        float length = doorAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length);

        IsOpened = true; // chỉ set true sau khi animation xong
    }
}