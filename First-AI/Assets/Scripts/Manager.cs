using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse Manager, die MonoBehaviour erbt und für das Management des Trainingsprozesses zuständig ist
public class Manager : MonoBehaviour
{
    public GameObject agentPrefab; // Referenz zum Agenten-Prefab
    public GameObject target; // Referenz zum Ziel-Objekt

    private bool isTraning = false; // Flag, das angibt, ob das Training läuft
    public int populationSize; // Größe der Population (Anzahl der Agenten)
    private int generationNumber = 0; // Zähler für die Anzahl der Generationen
    private int[] layers = new int[] { 1, 10, 10, 1 }; // Array für die Layer-Struktur des neuronalen Netzwerks
    private List<NeuralNetwork> nets; // Liste für die NeuralNetwork-Objekte
    private bool leftMouseDown = false; // Flag, das angibt, ob die linke Maustaste gedrückt ist
    private List<Agent> agentList = null; // Liste der Agenten

    // Timer-Methode, die isTraning auf false setzt
    void Timer()
    {
        isTraning = false; // Setzt das Training-Flag zurück
    }

    // Update-Methode, die in jedem Frame aufgerufen wird
    void Update()
    {
        if (isTraning == false) // Überprüft, ob das Training nicht läuft
        {
            if (generationNumber == 0) // Überprüft, ob es die erste Generation ist
            {
                InitAgentNeuralNetworks(); // Initialisiert die Neural Networks der Agenten
            }
            else // Wenn es nicht die erste Generation ist
            {
                Debug.Log("Gespeichert"); // Gibt eine Debug-Nachricht aus

                nets.Sort(); // Sortiert die Neural Networks basierend auf der Fitness
                for (int i = 0; i < populationSize / 2; i++) // Iteriert über die Hälfte der Population
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]); // Kopiert das zweite Netz in die erste Hälfte
                    nets[i].Mutate(); // Mutiert das kopierte Netzwerk

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); // Kopiert das zweite Netz unverändert
                }

                for (int i = 0; i < populationSize; i++) // Iteriert über die gesamte Population
                {
                    nets[i].SetFitness(0f); // Setzt die Fitness für jedes Netzwerk auf 0
                }
            }

            generationNumber++; // Erhöht die Generationsnummer um 1

            isTraning = true; // Setzt das Training-Flag auf true
            Invoke("Timer", 15f); // Ruft die Timer-Methode nach 15 Sekunden auf
            CreateAgentBodies(); // Erstellt die Agenten-Objekte
        }

        if (Input.GetMouseButtonDown(0)) // Überprüft, ob die linke Maustaste gedrückt wurde
        {
            leftMouseDown = true; // Setzt das Flag auf true
        }
        else if (Input.GetMouseButtonUp(0)) // Überprüft, ob die linke Maustaste losgelassen wurde
        {
            leftMouseDown = false; // Setzt das Flag auf false
        }

        if (leftMouseDown == true) // Überprüft, ob die linke Maustaste gedrückt ist
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Konvertiert die Mausposition in Weltkoordinaten
            target.transform.position = mousePosition; // Setzt die Position des Ziel-Objekts auf die Mausposition
        }
    }

    // Methode zur Erstellung der Agenten-Objekte
    private void CreateAgentBodies()
    {
        if (agentList != null) // Überprüft, ob die Agenten-Liste nicht null ist
        {
            for (int i = 0; i < agentList.Count; i++) // Iteriert über die vorhandenen Agenten
            {
                GameObject.Destroy(agentList[i].gameObject); // Zerstört das Agenten-Objekt
            }
        }

        agentList = new List<Agent>(); // Erstellt eine neue Liste für Agenten

        for (int i = 0; i < populationSize; i++) // Iteriert über die gesamte Population
        {
            Agent agnet = ((GameObject)Instantiate(agentPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), agentPrefab.transform.rotation)).GetComponent<Agent>(); // Erstellt einen neuen Agenten mit zufälliger Position
            agnet.Init(nets[i], target.transform); // Initialisiert den Agenten mit dem zugehörigen NeuralNetwork und Ziel
            agentList.Add(agnet); // Fügt den Agenten zur Agenten-Liste hinzu
        }
    }

    // Methode zur Initialisierung der Neural Networks der Agenten
    void InitAgentNeuralNetworks()
    {
        if (populationSize % 2 != 0) // Überprüft, ob die Population ungerade ist
        {
            populationSize = 20; // Setzt die Population auf 20, wenn sie ungerade ist
        }

        nets = new List<NeuralNetwork>(); // Erstellt eine neue Liste für die Neural Networks

        for (int i = 0; i < populationSize; i++) // Iteriert über die gesamte Population
        {
            NeuralNetwork net = new NeuralNetwork(layers); // Erstellt ein neues NeuralNetwork mit der angegebenen Layer-Struktur
            net.Mutate(); // Mutiert das Netzwerk
            nets.Add(net); // Fügt das Netzwerk zur Liste hinzu
        }
    }
}