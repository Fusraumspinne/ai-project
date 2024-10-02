using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse Agent, die MonoBehaviour erbt und für die Agenten-Logik zuständig ist
public class Agent : MonoBehaviour
{
    private bool initilized = false; // Flag, das angibt, ob der Agent initialisiert wurde
    private Transform target; // Referenz zum Ziel-Transform (target)

    private NeuralNetwork net; // Referenz zum zugehörigen NeuralNetwork
    private Rigidbody2D rBody; // Referenz zum Rigidbody2D des Agenten
    private Material[] mats; // Array für die Materialien der Kind-Objekte

    // Start-Methode, die beim Start des Spiels aufgerufen wird
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>(); // Holt den Rigidbody2D des Agenten
        mats = new Material[transform.childCount]; // Initialisiert das Material-Array mit der Anzahl der Kind-Objekte
        for (int i = 0; i < mats.Length; i++) // Iteriert über die Kind-Objekte
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material; // Holt das Material jedes Kind-Objekts
    }

    // FixedUpdate-Methode, die in jedem Fixed-Frame aufgerufen wird
    void FixedUpdate()
    {
        if (initilized == true) // Überprüft, ob der Agent initialisiert ist
        {
            float distance = Vector2.Distance(transform.position, target.position); // Berechnet die Distanz zum Ziel
            if (distance > 20f) // Überprüft, ob die Distanz größer als 20 ist
                distance = 20f; // Setzt die Distanz auf 20, wenn sie größer ist
            for (int i = 0; i < mats.Length; i++) // Iteriert über die Materialien
                mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f))); // Setzt die Farbe basierend auf der Distanz

            float[] inputs = new float[1]; // Initialisiert das Eingangs-Array mit einer Größe von 1


            float angle = transform.eulerAngles.z % 360f; // Holt den aktuellen Drehwinkel des Agenten in Z-Richtung
            if (angle < 0f) // Überprüft, ob der Winkel negativ ist
                angle += 360f; // Korrigiert den Winkel, um ihn positiv zu machen

            Vector2 deltaVector = (target.position - transform.position).normalized; // Berechnet den normalisierten Vektor zum Ziel


            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x); // Berechnet den Winkel in Bogenmaß
            rad *= Mathf.Rad2Deg; // Konvertiert den Winkel in Grad

            rad = rad % 360; // Stellt sicher, dass der Winkel im Bereich von 0 bis 360 bleibt
            if (rad < 0) // Überprüft, ob der Winkel negativ ist
            {
                rad = 360 + rad; // Korrigiert den Winkel, um ihn positiv zu machen
            }

            rad = 90f - rad; // Berechnet den Winkel, um die Richtung zu erhalten
            if (rad < 0f) // Überprüft, ob der Winkel negativ ist
            {
                rad += 360f; // Korrigiert den Winkel, um ihn positiv zu machen
            }
            rad = 360 - rad; // Invertiert den Winkel
            rad -= angle; // Subtrahiert den aktuellen Drehwinkel des Agenten
            if (rad < 0) // Überprüft, ob der Winkel negativ ist
                rad = 360 + rad; // Korrigiert den Winkel, um ihn positiv zu machen
            if (rad >= 180f) // Überprüft, ob der Winkel 180 Grad oder mehr beträgt
            {
                rad = 360 - rad; // Invertiert den Winkel
                rad *= -1f; // Macht den Winkel negativ
            }
            rad *= Mathf.Deg2Rad; // Konvertiert den Winkel zurück in Bogenmaß

            inputs[0] = rad / (Mathf.PI); // Setzt den Wert für die Eingaben basierend auf dem berechneten Winkel


            float[] output = net.FeedForward(inputs); // Holt die Ausgaben vom Neural Network

            rBody.velocity = 2.5f * transform.up; // Setzt die Geschwindigkeit des Agenten in die Vorwärtsrichtung
            rBody.angularVelocity = 500f * output[0]; // Setzt die Winkelgeschwindigkeit basierend auf der Ausgabe des Netzwerks

            net.AddFitness((1f - Mathf.Abs(inputs[0]))); // Aktualisiert die Fitness des Neural Networks basierend auf der Eingabe
        }
    }

    // Methode zur Initialisierung des Agenten mit dem Neural Network und dem Ziel
    public void Init(NeuralNetwork net, Transform target)
    {
        this.target = target; // Setzt die Referenz zum Ziel-Transform
        this.net = net; // Setzt die Referenz zum Neural Network
        initilized = true; // Setzt das Initialisierungs-Flag auf true
    }
}