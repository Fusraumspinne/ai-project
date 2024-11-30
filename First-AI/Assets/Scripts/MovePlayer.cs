using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public float speed;

    public float randomTimeMin;
    public float randomTimeMax;

    private float timeToChangeDirection;
    private Vector2 direction;

    public bool autoMove;

    void Start()
    {
        direction = GetRandomDirection();
        timeToChangeDirection = GetRandomTime();
    }

    void Update()
    {
        if (autoMove)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            timeToChangeDirection -= Time.deltaTime;
            if (timeToChangeDirection <= 0)
            {
                direction = GetRandomDirection();
                timeToChangeDirection = GetRandomTime();
            }

            CheckBoundsAndClamp();
        }
    }

    Vector2 GetRandomDirection()
    {
        Vector2 randomDirection;
        do
        {
            randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        } while (randomDirection == Vector2.zero);

        return randomDirection;
    }

    float GetRandomTime()
    {
        return Random.Range(randomTimeMin, randomTimeMax);
    }

    void CheckBoundsAndClamp()
    {
        Vector3 position = transform.position;

        if (position.x < xMin)
        {
            position.x = xMin;
            direction.x = Mathf.Abs(direction.x);
        }
        else if (position.x > xMax)
        {
            position.x = xMax;
            direction.x = -Mathf.Abs(direction.x);
        }

        if (position.y < yMin)
        {
            position.y = yMin;
            direction.y = Mathf.Abs(direction.y);
        }
        else if (position.y > yMax)
        {
            position.y = yMax;
            direction.y = -Mathf.Abs(direction.y);
        }

        transform.position = position;
    }
}