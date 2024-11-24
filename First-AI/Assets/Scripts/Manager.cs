using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject target; 

    private bool isTraning = false; 
    public int populationSize;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 };
    private List<NeuralNetwork> nets; 
    private bool leftMouseDown = false; 
    private List<Agent> agentList = null; 

    void Timer()
    {
        isTraning = false; 
    }

    void Update()
    {
        if (isTraning == false)
        {
            if (generationNumber == 0) 
            {
                InitAgentNeuralNetworks();
            }
            else 
            {
                nets.Sort();

                SaveNeuralNetworksToFile();

                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate(); 

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++; 

            isTraning = true; 
            Invoke("Timer", 15f); 
            CreateAgentBodies(); 
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            leftMouseDown = true; 
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

        if (leftMouseDown == true) 
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            target.transform.position = mousePosition; 
        }
    }

    private void CreateAgentBodies()
    {
        if (agentList != null) 
        {
            for (int i = 0; i < agentList.Count; i++)
            {
                GameObject.Destroy(agentList[i].gameObject); 
            }
        }

        agentList = new List<Agent>(); 

        for (int i = 0; i < populationSize; i++)
        {
            Agent agnet = ((GameObject)Instantiate(agentPrefab, new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0), agentPrefab.transform.rotation)).GetComponent<Agent>();
            agnet.Init(nets[i], target.transform); 
            agentList.Add(agnet);
        }
    }

    void InitAgentNeuralNetworks()
    {
        if (populationSize % 2 != 0)
        {
            populationSize = 20; 
        }

        nets = new List<NeuralNetwork>(); 

        for (int i = 0; i < populationSize; i++) 
        {
            NeuralNetwork net = new NeuralNetwork(layers); 
            net.Mutate(); 
            nets.Add(net);
        }
    }

    private void SaveNeuralNetworksToFile()
    {
        NeuralNetwork bestNetwork = null;
        float bestFitness = float.MinValue;

        foreach (var net in nets)
        {
            if (net.GetFitness() > bestFitness)
            {
                bestFitness = net.GetFitness();
                bestNetwork = net;
            }
        }

        if (bestNetwork != null)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "bestNeuralNetwork.txt");

            List<string> neuralNetworkData = new List<string>();

            string networkString = "Fitness: " + bestNetwork.GetFitness() + "\n";

            for (int i = 0; i < bestNetwork.GetWeights().Length; i++)
            {
                networkString += $"Layer {i + 1} weights:\n";

                for (int j = 0; j < bestNetwork.GetWeights()[i].Length; j++)
                {
                    networkString += $"Neuron {j + 1}: " + string.Join(", ", bestNetwork.GetWeights()[i][j]) + "\n";
                }
            }

            neuralNetworkData.Add(networkString);

            File.WriteAllLines(filePath, neuralNetworkData);
            Debug.Log("Best neural network saved to " + filePath);
        }
        else
        {
            Debug.LogError("No networks available to save.");
        }
    }
}