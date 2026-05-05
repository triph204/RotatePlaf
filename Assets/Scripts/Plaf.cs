using UnityEngine;
using UnityEngine.InputSystem;

public class Plaf : MonoBehaviour
{
    public float rotationDuration = 0.5f;
    public Vector3 centerPoint;

    private bool isSpinning = false;

    void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame && !isSpinning)
        {
            StartCoroutine(SpinOnce(90f)); // Q quay thuận
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && !isSpinning)
        {
            StartCoroutine(SpinOnce(-90f)); // E quay ngược
        }
    }

    private System.Collections.IEnumerator SpinOnce(float targetAngle)
    {
        isSpinning = true;
        float elapsed = 0f;
        float previousAngle = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;
            float currentAngle = Mathf.SmoothStep(0f, targetAngle, t);
            float delta = currentAngle - previousAngle;

            transform.RotateAround(centerPoint, Vector3.forward, delta);
            previousAngle = currentAngle;
            yield return null;
        }

        isSpinning = false;
    }
}