using UnityEngine;
using System.Collections;

public class TrapBall: MonoBehaviour
{
    [Header("Danh sách điểm di chuyển (theo thứ tự)")]
    public Transform[] points;      // các điểm bẫy sẽ đi qua
    public float speed = 10f;       // tốc độ di chuyển
    public float pauseTime = 0.3f;  // thời gian dừng tại mỗi điểm

    private int currentIndex = 0;

    void Start()
    {
        if (points.Length > 0)
        {
            transform.position = points[0].position; // đặt tại điểm đầu tiên
            StartCoroutine(MoveLoop());
        }
      
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            // điểm kế tiếp
            int nextIndex = (currentIndex + 1) % points.Length;
            Vector3 target = points[nextIndex].position;

            // di chuyển về điểm kế tiếp
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            // chờ 1 chút trước khi di chuyển tiếp
            yield return new WaitForSeconds(pauseTime);

            currentIndex = nextIndex;
        }
    }
}
