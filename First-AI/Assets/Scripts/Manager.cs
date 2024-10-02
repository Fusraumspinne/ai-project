using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse Manager, die MonoBehaviour erbt und f�r das Management des Trainingsprozesses zust�ndig ist
public class Manager : MonoBehaviour
{
    public GameObject agentPrefab; // Referenz zum Agenten-Prefab
    public GameObject target; // Referenz zum Ziel-Objekt

    private bool isTraning = false; // Flag, das angibt, ob das Training l�uft
    public int populationSize; // Gr��e der Population (Anzahl der Agenten)
    private int generationNumber = 0; // Z�hler f�r die Anzahl der Generationen
    private int[] layers = new int[] { 1, 10, 10, 1 }; // Array f�r die Layer-Struktur des neuronalen Netzwerks
    private List<NeuralNetwork> nets; // Liste f�r die NeuralNetwork-Objekte
    private bool leftMouseDown = false; // Flag, das angibt, ob die linke Maustaste gedr�ckt ist
    private List<Agent> agentList = null; // Liste der Agenten

    // Timer-Methode, die isTraning auf false setzt
    void Timer()
    {
        isTraning = false; // Setzt das Training-Flag zur�ck
    }

    // Update-Methode, die in jedem Frame aufgerufen wird
    void Update()
    {
        if (isTraning == false) // �berpr�ft, ob das Training nicht l�uft
        {
            if (generationNumber == 0) // �berpr�ft, ob es die erste Generation ist
            {
                InitAgentNeuralNetworks(); // Initialisiert die Neural Networks der Agenten
            }
            else // Wenn es nicht die erste Generation ist
            {
                Debug.Log("Gespeichert"); // Gibt eine Debug-Nachricht aus

                nets.Sort(); // Sortiert die Neural Networks basierend auf der Fitness
                for (int i = 0; i < populationSize / 2; i++) // Iteriert �ber die H�lfte der Population
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]); // Kopiert das zweite Netz in die erste H�lfte
                    nets[i].Mutate(); // Mutiert das kopierte Netzwerk

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); // Kopiert das zweite Netz unver�ndert
                }

                for (int i = 0; i < populationSize; i++) // Iteriert �ber die gesamte Population
                {
                    nets[i].SetFitness(0f); // Setzt die Fitness f�r jedes Netzwerk auf 0
                }
            }

            generationNumber++; // Erh�ht die Generationsnummer um 1

            isTraning = true; // Setzt das Training-Flag auf true
            Invoke("Timer", 15f); // Ruft die Timer-Methode nach 15 Sekunden auf
            CreateAgentBodies(); // Erstellt die Agenten-Objekte
        }

        if (Input.GetMouseButtonDown(0)) // �berpr�ft, ob die linke Maustaste gedr�ckt wurde
        {
            leftMouseDown = true; // Setzt das Flag auf true
        }
        else if (Input.GetMouseButtonUp(0)) // �berpr�ft, ob die linke Maustaste losgelassen wurde
        {
            leftMouseDown = false; // Setzt das Flag auf false
        }

        if (leftMouseDown == true) // �berpr�ft, ob die linke Maustaste gedr�ckt ist
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Konvertiert die Mausposition in Weltkoordinaten
            target.transform.position = mousePosition; // Setzt die Position des Ziel-Objekts auf die Mausposition
        }
    }

    // Methode zur Erstellung der Agenten-Objekte
    private void CreateAgentBodies()
    {
        if (agentList != null) // �berpr�ft, ob die Agenten-Liste nicht null ist
        {
            for (int i = 0; i < agentList.Count; i++) // Iteriert �ber die vorhandenen Agenten
            {
                GameObject.Destroy(agentList[i].gameObject); // Zerst�rt das Agenten-Objekt
            }
        }

        agentList = new List<Agent>(); // Erstellt eine neue Liste f�r Agenten

        for (int i = 0; i < populationSize; i++) // Iteriert �ber die gesamte Population
        {
            Agent agnet = ((GameObject)Instantiate(agentPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), agentPrefab.transform.rotation)).GetComponent<Agent>(); // Erstellt einen neuen Agenten mit zuf�lliger Position
            agnet.Init(nets[i], target.transform); // Initialisiert den Agenten mit dem zugeh�rigen NeuralNetwork und Ziel
            agentList.Add(agnet); // F�gt den Agenten zur Agenten-Liste hinzu
        }
    }

    // Methode zur Initialisierung der Neural Networks der Agenten
    void InitAgentNeuralNetworks()
    {
        if (populationSize % 2 != 0) // �berpr�ft, ob die Population ungerade ist
        {
            populationSize = 20; // Setzt die Population auf 20, wenn sie ungerade ist
        }

        nets = new List<NeuralNetwork>(); // Erstellt eine neue Liste f�r die Neural Networks

        for (int i = 0; i < populationSize; i++) // Iteriert �ber die gesamte Population
        {
            NeuralNetwork net = new NeuralNetwork(layers); // Erstellt ein neues NeuralNetwork mit der angegebenen Layer-Struktur
            net.Mutate(); // Mutiert das Netzwerk
            nets.Add(net); // F�gt das Netzwerk zur Liste hinzu
        }
    }
}