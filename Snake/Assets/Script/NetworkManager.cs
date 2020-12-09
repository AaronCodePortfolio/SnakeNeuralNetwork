using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

	private List<Network> family = new List<Network>();
	private List<Network> topFamily = new List<Network>();
	public Text currentOutput;
	public Text lastOutput;
	public Text highscoreOutput;
	private int brain = 0;
	private int generation = 0;
	private int familyNum = 0;
	private int highscore = 0;
	public int numOfHiddenLayers;
	public int nodesInLayer;
	bool finalFamily = false;

	// Use this for initialization
	void Start ()
	{
		int generate;
		for(generate = 0; generate < 100; generate++)
		{
			family.Add(new Network());
			family[generate].GenerateNetwork(numOfHiddenLayers, nodesInLayer);
		}
		
	}

	public double[] Think(List<int> startValues)
	{
		return family[brain].Think(startValues);
	}

	public void NextNetwork(int fitness)
	{

		//Debug.Log("Family:" + familyNum + " Generation: " + generation + " Snake Brain: " + brain + " score: " + fitness);

		lastOutput.text = "Last snake: Family:" + familyNum + " Generation: " + generation + " Snake Brain: " + brain + " score: " + fitness;

		

		gameObject.GetComponent<SpawnFood>().Reset();

		family[brain].Fitness = fitness;

		if (family[brain].Fitness > highscore)
		{
			highscore = fitness;
			highscoreOutput.text = "High Score: Family:" + familyNum + " Generation: " + generation + " Snake Brain: " + brain + " score: " + fitness;
		}

		if (brain < 99)
		{
			brain++;
		}
		else
		{
			brain = 0;
			NextGeneration();
			generation++;
			if(generation > 10 && !finalFamily)
			{
				generation = 0;
				familyNum++;
				NextFamily();
				if(familyNum >= 100)
				{
					family.Clear();
					family = topFamily;
					finalFamily = true;
				}
			}
		}

		currentOutput.text = "Current Snake: Family:" + familyNum + " Generation: " + generation + " Snake Brain: " + brain;
	}

	private void NextGeneration()
	{
		int looper;
		int halfCount = family.Count / 2;

		SortFamily();

		family.RemoveRange(halfCount, halfCount);

		//Debug.Log("Half removed start breading");

		for(looper = 0; looper < halfCount; looper += 2)
		{
			Breed(looper, looper + 1);
			//Debug.Log("Child " + (looper - 1));
			Breed(looper + 1, looper);
			//Debug.Log("Child " + (looper));
		}

		//Debug.Log("Breeding finished");
	}

	private void SortFamily()
	{
		int looper1;
		int looper2;

		Network temp;

		for(looper1 = 1; looper1 < family.Count; looper1++)
		{
			for(looper2 = looper1; looper2 > 0 && family[looper2].Fitness > family[looper2 - 1].Fitness; looper2--)
			{
				temp = family[looper2];
				family[looper2] = family[looper2 - 1];
				family[looper2 - 1] = temp;
			}
		}
	}

	private void Breed(int parent1, int parent2)
	{
		int looper1;
		Network child = new Network();
		List<List<Node>> parent1Brain = family[parent1].GetLayers();
		List<List<Node>> parent2Brain = family[parent2].GetLayers();

		for (looper1 = 0; looper1 < family[parent1].GetLayers().Count; looper1++)
		{
			parent1Brain[looper1].RemoveRange(parent1Brain[looper1].Count / 2, parent1Brain[looper1].Count / 2);
			parent2Brain[looper1].RemoveRange(0, parent2Brain[looper1].Count / 2);
		}

		child.SetNodes(parent1Brain, parent2Brain);
		family.Add(child);
	}

	private void NextFamily()
	{
		topFamily.Add(family[0]);

		family.Clear();
		
		int generate;
		for (generate = 0; generate < 100; generate++)
		{
			family.Add(new Network());
			family[generate].GenerateNetwork(numOfHiddenLayers, nodesInLayer);
		}
	}
}
