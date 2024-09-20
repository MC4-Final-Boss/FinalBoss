using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 1.0f;

    private Vector3 initialPosition;
    private bool movingToTarget = true;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (movingToTarget)
        {
            MoveTowards(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                movingToTarget = false;
            }
        }
        else
        {
            MoveTowards(initialPosition);

            if (Vector3.Distance(transform.position, initialPosition) < 0.1f)
            {
                movingToTarget = true;
            }
        }
    }

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        other.transform.SetParent(transform);

    }

    private void OnCollisionExit2D(Collision2D other) {
        other.transform.SetParent(null);
    }

    
}