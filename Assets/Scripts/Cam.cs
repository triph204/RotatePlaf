using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Rotate with gravity")]
    [SerializeField] private float rotateSpeed = 360f;

    [Header("Boundary (vùng camera được phép di chuyển)")]
    [SerializeField] private bool useBoundary = false;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    private static readonly float[] GravAngles = { 0f, -90f, 180f, 90f };
    private PlayerMovement pm;

    private void Awake()
    {
        if (target != null)
            pm = target.GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Follow
        float x = target.position.x + offset.x;
        float y = target.position.y + offset.y;

        if (useBoundary)
        {
            x = Mathf.Clamp(x, minX, maxX);
            y = Mathf.Clamp(y, minY, maxY);
        }

        Vector3 desired = new Vector3(x, y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Rotate theo gravity
        if (pm == null) return;
        float targetAngle = GravAngles[pm.GravDir];
        float currentAngle = transform.eulerAngles.z;
        float delta = Mathf.DeltaAngle(currentAngle, targetAngle);
        float step = rotateSpeed * Time.deltaTime;
        float newAngle = Mathf.Abs(delta) <= step
            ? targetAngle
            : currentAngle + Mathf.Sign(delta) * step;
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    private void OnDrawGizmosSelected()
    {
        if (!useBoundary) return;
        Gizmos.color = Color.yellow;
        float cx = (minX + maxX) / 2f;
        float cy = (minY + maxY) / 2f;
        float w = maxX - minX;
        float h = maxY - minY;
        Gizmos.DrawWireCube(new Vector3(cx, cy, 0), new Vector3(w, h, 0));
    }
}