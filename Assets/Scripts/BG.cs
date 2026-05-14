using UnityEngine;

public class BG : MonoBehaviour
{
    [SerializeField] private Transform bg1;
    [SerializeField] private Transform bg2;
    [SerializeField] private float speed = 1f;

    private float height;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Dùng bounds tính đúng kích thước thực sau scale
        SpriteRenderer sr = bg1.GetComponent<SpriteRenderer>();
        height = sr.bounds.size.y;

        // Tự động đặt bg2 ngay phía trên bg1
        bg2.position = new Vector3(bg1.position.x, bg1.position.y + height, bg1.position.z);
    }

    void Update()
    {
        bg1.position += Vector3.down * speed * Time.deltaTime;
        bg2.position += Vector3.down * speed * Time.deltaTime;

        CheckLoop(bg1, bg2);
        CheckLoop(bg2, bg1);
    }

    void CheckLoop(Transform self, Transform other)
    {
        if (self.position.y < cam.transform.position.y - height)
            self.position = new Vector3(self.position.x, other.position.y + height, self.position.z);
    }
}