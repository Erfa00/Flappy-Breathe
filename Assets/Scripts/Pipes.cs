using UnityEngine;

public class Pipes : MonoBehaviour
{
    private float speed = 1f;
    private float leftEdge;

    void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1;
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if(transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
