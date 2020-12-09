using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// val  = sig(w1 * a1 + w2 * a2 ... + bias)

public class Node
{
	private double value;
	private double bias;
	private List<Node> prevousLayer = null;
	private List<double> weight = new List<double>();

	public Node()
	{

	}
	public Node(Node newNode)
	{
		this.value = newNode.value;
		this.bias = newNode.bias;
		this.weight = new List<double>(newNode.weight);
	}
	public Node(List<Node> newPrevousLayer)
	{
		this.prevousLayer = newPrevousLayer;
	}

	public double Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public double Bias
	{
		get
		{
			return this.bias;
		}
		set
		{
			this.bias = value;
		}
	}

	public List<double> Weight
	{
		get
		{
			return new List<double>(this.weight);
		}
		set
		{
			this.weight = value;
		}
	}

	public List<Node> PrevousLayer
	{
		set
		{
			this.prevousLayer = value;
		}
	}


	public void CalcValue()
	{
		int looper;

		this.value = 0;
		for (looper = 0; looper < prevousLayer.Count; looper++)
		{
			this.value += prevousLayer[looper].value * weight[looper];
		}
		this.value += this.bias;
	}
}

