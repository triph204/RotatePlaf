using UnityEngine;

/// <summary>
/// Cuộn background vô hạn theo chiều dọc.
/// Không thay đổi pattern, đã đúng Single Responsibility.
/// </summary>
public class BG : MonoBehaviour
{
    [SerializeField] private Transform bg1;
    [SerializeField] private Transform bg2;
    [SerializeField] private float speed = 1f;

    private float _height;
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        _height = bg1.GetComponent<SpriteRenderer>().bounds.size.y;
        bg2.position = new Vector3(bg1.position.x, bg1.position.y + _height, bg1.position.z);
    }

    private void Update()
    {
        bg1.position += Vector3.down * speed * Time.deltaTime;
        bg2.position += Vector3.down * speed * Time.deltaTime;
        CheckLoop(bg1, bg2);
        CheckLoop(bg2, bg1);
    }

    private void CheckLoop(Transform self, Transform other)
    {
        if (self.position.y < _cam.transform.position.y - _height)
            self.position = new Vector3(self.position.x, other.position.y + _height, self.position.z);
    }
}