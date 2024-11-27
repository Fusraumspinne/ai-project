using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int rayCount; // Anzahl der Raycasts
    public float rayLength; // Länge der Raycasts
    public float angleSpread; // Winkelverteilung der Raycasts

    void Update()
    {
        FireRays();
    }

    void FireRays()
    {
        float angleStep = angleSpread / (rayCount - 1);
        float startAngle = -angleSpread / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength);

            // Zeichne den Ray im Editor
            Debug.DrawRay(transform.position, direction * rayLength, Color.green);

            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                Debug.Log($"Ray {i} trifft eine Wand: {hit.collider.name}");
            }
        }
    }
}
