using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Network
{
	private List<List<Node>> layers = new List<List<Node>>();
	private int fitness;

	public int Fitness
	{
		get
		{
			return this.fitness;
		}
		set
		{
			this.fitness = value;
		}
	}

	public List<List<Node>> GetLayers()
	{
		int looper;

		List<List<Node>> sendLayers = new List<List<Node>>();

		for(looper = 0; looper < this.layers.Count; looper++)
		{
			sendLayers.Add(new List<Node>(this.layers[looper]));
		}

		return sendLayers;
	}

	public void GenerateNetwork(int numOfHiddenLayers, int nodesInLayer)
	{
		int looper1;
		int looper2;

		//Create the lists for the input, output, and hidden layers
		for(looper1 = 0; looper1 < numOfHiddenLayers + 2; looper1++)
		{
			this.layers.Add(new List<Node>());
		}
		
		//Generate the nodes for the input layer (fixed 24 nodes)
		for(looper1 = 0; looper1 < 24; looper1++)
		{
			this.layers[0].Add(new Node());
		}

		//Generate the nodes for each hidden layer each with their connections to the previous layer
		for(looper1 = 0; looper1 < numOfHiddenLayers; looper1++)
		{
			for (looper2 = 0; looper2 < nodesInLayer; looper2++)
			{
				this.layers[looper1 + 1].Add(new Node(this.layers[looper1]));
			}
		}

		//Generate the nodes for the output layer each with their connections to the previous layer
		for (looper1 = 0; looper1 < 4; looper1++)
		{
			this.layers[this.layers.Count - 1].Add(new Node(this.layers[this.layers.Count - 2]));
		}

		RandNodes();
	}


	public double[] Think(List<int> startValues)
	{
		int looper1;
		int looper2;
		

		for(looper1 = 0; looper1 < layers[0].Count; looper1++)
		{
			layers[0][looper1].Value = startValues[looper1];
		}

		//For each layer except the first
		for(looper1 = 1; looper1 < this.layers.Count; looper1++)
		{
			//For each node in that layer
			for(looper2 = 0; looper2 < this.layers[looper1].Count; looper2++)
			{
				//Calculate the value of that node
				this.layers[looper1][looper2].CalcValue();

				//Run that value through a sigmoid
				this.layers[looper1][looper2].Value = Sigmoid(this.layers[looper1][looper2].Value);
			}
		}

		return new double[] { this.layers[layers.Count - 1][0].Value, this.layers[layers.Count - 1][1].Value, this.layers[layers.Count - 1][2].Value, this.layers[layers.Count - 1][3].Value };
	}

	public void RandNodes()
	{
		int looper1;
		int looper2;
		int looper3;

		List<double> weight = null;

		//For each layer except the First
		for (looper1 = 1; looper1 < this.layers.Count; looper1++)
		{
			//For each node in that layer
			for (looper2 = 0; looper2 < this.layers[looper1].Count; looper2++)
			{
				weight = new List<double>();
				//For each node in the previous layer
				for(looper3 = 0; looper3 < this.layers[looper1 - 1].Count; looper3++)
				{
					weight.Add(Random.Range(-5, 5));
				}

				layers[looper1][looper2].Weight = weight;
				layers[looper1][looper2].Bias = Random.Range(-10, 10);
			}
		}
	}

	public void SetNodes(List<List<Node>> firstHalf, List<List<Node>> secondHalf)
	{
		List<List<Node>> fullNetwork = new List<List<Node>>();
		int looperLayers;
		int looperNodes;


		for (looperLayers = 0; looperLayers < firstHalf.Count; looperLayers++)
		{
			fullNetwork.Add(new List<Node>());

			for (looperNodes = 0; looperNodes < firstHalf[looperLayers].Count; looperNodes++)
			{
				fullNetwork[looperLayers].Add(new Node(firstHalf[looperLayers][looperNodes]));

				if (looperLayers != 0)
				{
					fullNetwork[looperLayers][looperNodes].PrevousLayer = fullNetwork[looperLayers - 1];
				}
			}

			for (looperNodes = 0; looperNodes < secondHalf[looperLayers].Count; looperNodes++)
			{
				fullNetwork[looperLayers].Add(new Node(secondHalf[looperLayers][looperNodes]));

				if (looperLayers != 0)
				{
					fullNetwork[looperLayers][looperNodes + firstHalf[looperLayers].Count].PrevousLayer = fullNetwork[looperLayers - 1];
				}
			}


		}


		SetNodes(fullNetwork);
	}
	public void SetNodes(List<List<Node>> newNetwork)
	{
		layers.Clear();
		layers.AddRange(newNetwork);
	}


	private double Sigmoid(double x)
	{
		return 1 / (1 + Mathf.Exp(-(float)x));
	}
}
