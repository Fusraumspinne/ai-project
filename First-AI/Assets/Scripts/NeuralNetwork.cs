using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Klasse NeuralNetwork, implementiert das IComparable-Interface f�r Vergleichsfunktionen
public class NeuralNetwork : IComparable<NeuralNetwork> 
{
    private int[] layers; // Array zur Speicherung der Anzahl der Neuronen in jedem Layer
    private float[][] neurons; // Array f�r die Neuronenwerte in jedem Layer
    private float[][][] weights; // 3D-Array f�r die Gewichte zwischen den Neuronen
    private float fitness; // Fitnesswert des Netzwerks

    // Konstruktor f�r die Erstellung eines neuen NeuralNetwork mit einer angegebenen Layer-Konfiguration
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length]; // Initialisiert das layers-Array mit der gleichen L�nge wie das �bergebene Array
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i]; // Kopiert die Layer-Anzahl
        }

        InitNeurons(); // Initialisiert das Neuronen-Array
        InitWeights(); // Initialisiert das Gewichts-Array
    }

    // Konstruktor f�r das Kopieren eines bestehenden NeuralNetwork
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length]; // Initialisiert das layers-Array mit der L�nge des kopierten Netzwerks
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i]; // Kopiert die Layer-Anzahl
        }

        InitNeurons(); // Initialisiert das Neuronen-Array
        InitWeights(); // Initialisiert das Gewichts-Array
        CopyWeights(copyNetwork.weights); // Kopiert die Gewichte vom �bergebenen Netzwerk
    }

    // Methode zum Kopieren der Gewichte von einem anderen Netzwerk
    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++) // Iteriert �ber die Layer
        {
            for (int j = 0; j < weights[i].Length; j++) // Iteriert �ber die Neuronen im aktuellen Layer
            {
                for (int k = 0; k < weights[i][j].Length; k++) // Iteriert �ber die Gewichte der Neuronen
                {
                    weights[i][j][k] = copyWeights[i][j][k]; // Kopiert das Gewicht
                }
            }
        }
    }

    // Methode zur Initialisierung des Neuronen-Arrays
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>(); // Erstellt eine Liste f�r die Neuronen

        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]); // F�gt ein neues Array f�r die Neuronen des aktuellen Layers hinzu
        }

        neurons = neuronsList.ToArray(); // Konvertiert die Liste in ein Array
    }

    // Methode zur Initialisierung des Gewichts-Arrays
    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>(); // Erstellt eine Liste f�r die Gewichte

        for (int i = 1; i < layers.Length; i++) // Beginnt bei der zweiten Layer (i = 1)
        {
            List<float[]> layerWeightsList = new List<float[]>(); // Erstellt eine Liste f�r die Gewichte des aktuellen Layers

            int neuronsInPreviousLayer = layers[i - 1]; // Speichert die Anzahl der Neuronen im vorherigen Layer

            for (int j = 0; j < neurons[i].Length; j++) // Iteriert �ber die Neuronen im aktuellen Layer
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; // Erstellt ein Array f�r die Gewichte eines Neurons

                for (int k = 0; k < neuronsInPreviousLayer; k++) // Iteriert �ber die Neuronen im vorherigen Layer
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); // Initialisiert das Gewicht mit einem zuf�lligen Wert
                }

                layerWeightsList.Add(neuronWeights); // F�gt das Gewicht-Array der Liste hinzu
            }

            weightsList.Add(layerWeightsList.ToArray()); // Konvertiert die Liste in ein Array und f�gt es der Gewichtsliste hinzu
        }

        weights = weightsList.ToArray(); // Konvertiert die Gewichtsliste in ein Array
    }

    // Methode zur Durchf�hrung der Vorw�rtsausbreitung des Netzwerks
    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i]; // Setzt die Eingabewerte in den ersten Layer
        }

        for (int i = 1; i < layers.Length; i++) // Iteriert �ber die Layers, beginnend mit dem zweiten Layer
        {
            for (int j = 0; j < neurons[i].Length; j++) // Iteriert �ber die Neuronen im aktuellen Layer
            {
                float value = 0f; // Initialisiert den Wert des Neurons

                for (int k = 0; k < neurons[i - 1].Length; k++) // Iteriert �ber die Neuronen im vorherigen Layer
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; // Berechnet den gewichteten Wert
                }

                neurons[i][j] = (float)Math.Tanh(value); // Wendet die Tanh-Aktivierungsfunktion an
            }
        }

        return neurons[neurons.Length - 1]; // Gibt die Ausgaben des letzten Layers zur�ck
    }

    // Methode zur Mutation der Gewichte
    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++) // Iteriert �ber die Layers
        {
            for (int j = 0; j < weights[i].Length; j++) // Iteriert �ber die Neuronen im aktuellen Layer
            {
                for (int k = 0; k < weights[i][j].Length; k++) // Iteriert �ber die Gewichte der Neuronen
                {
                    float weight = weights[i][j][k]; // Speichert das aktuelle Gewicht

                    float randomNumber = UnityEngine.Random.Range(0f, 100f); // Generiert eine zuf�llige Zahl zwischen 0 und 100

                    if (randomNumber <= 2f) // 2% Wahrscheinlichkeit
                    {
                        weight *= -1f; // Negiert das Gewicht
                    }
                    else if (randomNumber <= 4f) // 2% Wahrscheinlichkeit
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f); // Setzt das Gewicht auf einen neuen zuf�lligen Wert
                    }
                    else if (randomNumber <= 6f) // 2% Wahrscheinlichkeit
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f; // Generiert einen zuf�lligen Faktor
                        weight *= factor; // Multipliziert das Gewicht mit dem Faktor
                    }
                    else if (randomNumber <= 8f) // 2% Wahrscheinlichkeit
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f); // Generiert einen zuf�lligen Faktor
                        weight *= factor; // Multipliziert das Gewicht mit dem Faktor
                    }

                    weights[i][j][k] = weight; // Setzt das mutierte Gewicht zur�ck
                }
            }
        }
    }

    // Methode zur Hinzuf�gung eines Fitnesswerts
    public void AddFitness(float fit)
    {
        fitness += fit; // Addiert den Fitnesswert zum aktuellen Fitnesswert
    }

    // Methode zum Setzen eines Fitnesswerts
    public void SetFitness(float fit)
    {
        fitness = fit; // Setzt den Fitnesswert auf den angegebenen Wert
    }

    // Methode zum Abrufen des Fitnesswerts
    public float GetFitness()
    {
        return fitness; // Gibt den Fitnesswert zur�ck
    }

    // Methode f�r den Vergleich von NeuralNetwork-Objekten basierend auf Fitnesswert
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1; // Wenn das andere Netzwerk null ist, ist dieses Netzwerk besser

        if (fitness > other.fitness) // Wenn der Fitnesswert gr��er ist
            return 1; // Dieses Netzwerk ist besser
        else if (fitness < other.fitness) // Wenn der Fitnesswert kleiner ist
            return -1; // Dieses Netzwerk ist schlechter
        else
            return 0; // Fitnesswerte sind gleich
    }
}