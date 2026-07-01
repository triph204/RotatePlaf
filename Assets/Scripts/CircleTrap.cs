using UnityEngine;

public class CircleTrap : MonoBehaviour
{

    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 3f;
    int i = 0;
    void Start()
    {
        transform.position = points[0].position;
    }

    // Update is called once per frame
    void Update()
    {

        // Đi tới target
        Vector3 target = points[i].position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Quay mặt theo hướng di chuyển

        // Đổi điểm khi tới nơi
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            i = (i + 1) % points.Length;
        }
    }
}
