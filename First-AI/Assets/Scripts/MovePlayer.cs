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
            // Bewegt den Spieler in der aktuellen Richtung
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Zählt die Zeit für die Richtungsänderung herunter
            timeToChangeDirection -= Time.deltaTime;
            if (timeToChangeDirection <= 0)
            {
                direction = GetRandomDirection();
                timeToChangeDirection = GetRandomTime();
            }

            // Prüft auf Kollision mit den Begrenzungen
            CheckBoundsAndClamp();
        }
    }

    Vector2 GetRandomDirection()
    {
        // Generiert eine zufällige Richtung und stellt sicher, dass sie nicht null ist
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

        // Überprüft, ob der Spieler die Begrenzung erreicht hat
        if (position.x < xMin)
        {
            position.x = xMin;
            direction.x = Mathf.Abs(direction.x); // Bewegt sich nach rechts
        }
        else if (position.x > xMax)
        {
            position.x = xMax;
            direction.x = -Mathf.Abs(direction.x); // Bewegt sich nach links
        }

        if (position.y < yMin)
        {
            position.y = yMin;
            direction.y = Mathf.Abs(direction.y); // Bewegt sich nach oben
        }
        else if (position.y > yMax)
        {
            position.y = yMax;
            direction.y = -Mathf.Abs(direction.y); // Bewegt sich nach unten
        }

        transform.position = position; // Aktualisiert die Position, um sicherzustellen, dass sie im Bereich bleibt
    }
}