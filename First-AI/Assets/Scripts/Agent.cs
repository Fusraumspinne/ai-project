using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private bool initilized = false;
    private Transform target; 

    private NeuralNetwork net;
    private Rigidbody2D rBody; 
    private Material[] mats;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        mats = new Material[transform.childCount];
        for (int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (initilized == true)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance > 20f)
                distance = 20f;
            for (int i = 0; i < mats.Length; i++)
                mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[6];

            float angle = transform.eulerAngles.z % 360f;
            if (angle < 0f)
                angle += 360f;

            Vector2 deltaVector = (target.position - transform.position).normalized;


            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
            rad *= Mathf.Rad2Deg;

            rad = rad % 360;
            if (rad < 0)
            {
                rad = 360 + rad;
            }

            rad = 90f - rad;
            if (rad < 0f)
            {
                rad += 360f;
            }
            rad = 360 - rad;
            rad -= angle;
            if (rad < 0)
                rad = 360 + rad;
            if (rad >= 180f)
            {
                rad = 360 - rad;
                rad *= -1f;
            }
            rad *= Mathf.Deg2Rad;

            inputs[0] = rad / Mathf.PI;
            inputs[1] = CastRay(transform.up);
            inputs[2] = CastRay(Quaternion.Euler(0, 0, 25) * transform.up);
            inputs[3] = CastRay(Quaternion.Euler(0, 0, -25) * transform.up);
            inputs[4] = CastRay(Quaternion.Euler(0, 0, 50) * transform.up);
            inputs[5] = CastRay(Quaternion.Euler(0, 0, -50) * transform.up);

            float[] output = net.FeedForward(inputs);

            rBody.velocity = 2.5f * transform.up;
            rBody.angularVelocity = 500f * output[0];

            net.AddFitness(1f - Mathf.Abs(inputs[0]));
        }
    }

    private float CastRay(Vector2 direction)
    {
        Debug.DrawRay(transform.position, direction * 3f, Color.red);

        int layerMask = ~LayerMask.GetMask("Agent");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3f, layerMask);

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            return hit.distance / 3f;
        }

        return 1f;
    }

    public void Init(NeuralNetwork net, Transform target)
    {
        this.target = target; 
        this.net = net;
        initilized = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            //net.AddFitness(-200f);
            Destroy(gameObject);
        }
    }
}