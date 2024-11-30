using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class Manager : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject target;

    private bool isTraning = false;
    public int populationSize;
    private int generationNumber = 0;
    private int[] layers = new int[] { 6, 20, 20, 1 };
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

                Save();

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
                if (agentList[i] != null)
                    Destroy(agentList[i].gameObject);
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

    private void Save()
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
            SavedNetwork savedNetwork = new SavedNetwork();
            savedNetwork.fitness = bestFitness;

            List<LayerWeightsArray> layerArrays = new List<LayerWeightsArray>();

            for (int i = 0; i < bestNetwork.GetWeights().Length; i++)
            {
                LayerWeightsArray layerArray = new LayerWeightsArray();
                List<WeightsArray> weightsArrays = new List<WeightsArray>();

                for (int j = 0; j < bestNetwork.GetWeights()[i].Length; j++)
                {
                    WeightsArray weightsArray = new WeightsArray();
                    weightsArray.weights = bestNetwork.GetWeights()[i][j]; 

                    weightsArrays.Add(weightsArray);
                }

                layerArray.weightsArrays = weightsArrays.ToArray();
                layerArrays.Add(layerArray);
            }

            savedNetwork.layerArrays = layerArrays.ToArray();

            string json = JsonUtility.ToJson(savedNetwork, true);
            string filePath = Path.Combine(Application.persistentDataPath, "bestNeuralNetwork.json");
            File.WriteAllText(filePath, json);
        }
    }

}

[System.Serializable]
public class SavedNetwork
{
    public float fitness;
    public LayerWeightsArray[] layerArrays;
}

[System.Serializable]
public class WeightsArray
{
    public float[] weights; 
}

[System.Serializable]
public class LayerWeightsArray
{
    public WeightsArray[] weightsArrays;
}