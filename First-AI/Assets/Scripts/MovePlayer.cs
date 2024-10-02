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

    void Start()
    {
        direction = GetRandomDirection();

        timeToChangeDirection = GetRandomTime();
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            direction = GetRandomDirection();
            timeToChangeDirection = GetRandomTime();
        }

        CheckBoundsAndChangeDirection();
    }

    Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    float GetRandomTime()
    {
        return Random.Range(randomTimeMin, randomTimeMax);
    }

    void CheckBoundsAndChangeDirection()
    {
        Vector3 position = transform.position;

        if (position.x < xMin || position.x > xMax)
        {
            direction.x *= -1;
        }

        if (position.y < yMin || position.y > yMax)
        {
            direction.y *= -1;
        }
    }
}
