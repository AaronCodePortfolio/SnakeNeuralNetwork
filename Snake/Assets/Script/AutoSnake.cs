using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSnake : MonoBehaviour
{
	public GameObject networkController;

	private Vector2 dir = Vector2.right;

	private List<GameObject> tail = new List<GameObject>();

	//The node order is Wall, Body, Food, each starting in the up/left direction and moving clockwise
	private List<int> startingValues = new List<int>();

	private bool ate = false;
	private bool wait = true;
	private bool alive = true;

	private int length = 0;
	private int moveCount = 0;

	private int lifetime = 0;
	private int fitness = 0;
	private int lifeToLive = 200;

	private double[] direction = new double[4];

	public GameObject tailPrefab;

	private Vector2Int[] rayPosChange = new Vector2Int[8] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0) };

	// Use this for initialization
	void Start()
	{
		Reset();
	}

	private void Reset()
	{
		transform.position = new Vector3(0, 0, 0);
		//InvokeRepeating("Wait", .01f, .01f);
		InvokeRepeating("Wait", .001f, .001f);
		alive = true;
		fitness = 0;
		lifetime = 0;
		lifeToLive = 200;
		length = 0;
	}

	private void Wait()
	{
		this.wait = true;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		int looper;
		double largestVal = 0;
		int largestPos = 0;

		while(alive && wait)
		{
			Look();

			direction = networkController.GetComponent<NetworkManager>().Think(startingValues);

			for (looper = 0; looper < direction.Length; looper++)
			{
				if (direction[looper] > largestVal)
				{
					if((dir == Vector2.right && looper != 2)|| (dir == Vector2.up && looper != 1)|| (dir == -Vector2.right && looper != 0)|| (dir == -Vector2.up && looper != 3))
					{
						largestVal = direction[looper];
						largestPos = looper;
					}
					
				}
			}

			if (largestPos == 0)
			{
				dir = Vector2.right;
			}
			else if (largestPos == 1)
			{
				dir = -Vector2.up;
			}
			else if (largestPos == 2)
			{
				dir = -Vector2.right;
			}
			else if (largestPos == 3)
			{
				dir = Vector2.up;
			}
			
			moveCount++;

			Move();

			lifeToLive--;
			lifetime++;

			this.wait = false;

			if (lifeToLive <= 0)
			{
				Dead();
			}

			
		}
	}

	void Move()
	{
		//Save current position (gap will go here)
		Vector2 v = transform.position;

		//Move head
		transform.Translate(dir);

		if (ate)
		{

			//Load prefab
			GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

			//Add to list
			tail.Insert(0, g);

			//reset flag
			ate = false;
		}

		//Is there a tail
		else if (tail.Count > 0)
		{
			//Move last tail Element to where the Head was
			tail[tail.Count - 1].transform.position = v;

			//Add to from of the list, remove from back
			tail.Insert(0, tail[tail.Count - 1]);
			tail.RemoveAt(tail.Count - 1);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "Food")
		{
			ate = true;

			length++;

			lifeToLive += 100;

			Destroy(coll.gameObject);
		}
		else
		{
			Dead();
		}
	}

	private void Look()
	{


		int mainLooper;
		int hitLooper;

		List<int> wall = new List<int>();
		List<int> body = new List<int>();
		List<int> food = new List<int>();

		RaycastHit2D[] hits;

		bool bodyFound;
		bool foodFound;

		Ray2D[] rays = new Ray2D[8];

		for(mainLooper = 0; mainLooper < rays.Length; mainLooper++)
		{
			rays[mainLooper].origin = (Vector2)transform.position + rayPosChange[mainLooper];
			rays[mainLooper].direction = rayPosChange[mainLooper];
			hits = Physics2D.RaycastAll(rays[mainLooper].origin, rays[mainLooper].direction);

			bodyFound = false;
			foodFound = false;

			for(hitLooper = 0; hitLooper < hits.Length; hitLooper++)
			{
				if(hits[hitLooper].collider.gameObject.CompareTag("Player") && !bodyFound)
				{
					bodyFound = true;
					body.Add((int)Mathf.Ceil(hits[hitLooper].distance));
				}
				else if (hits[hitLooper].collider.gameObject.CompareTag("Food") && !foodFound)
				{
					foodFound = true;
					food.Add((int)Mathf.Ceil(hits[hitLooper].distance));
					//Debug.Log("Food Found Ray: " + mainLooper + " Distance: " + (int)Mathf.Ceil(hits[hitLooper].distance));
				}
				else if (hits[hitLooper].collider.gameObject.CompareTag("Wall"))
				{
					wall.Add((int)Mathf.Ceil(hits[hitLooper].distance));
					//Debug.Log("Wall Found Ray: " + mainLooper + " Distance: " + (int)Mathf.Ceil(hits[hitLooper].distance));
				}
			}

			if(!bodyFound)
			{
				body.Add(64);
			}

			if(!foodFound)
			{
				food.Add(64);
			}

			//Debug.Log("Ray " + mainLooper + "Body " + body[hitLooper]);
			//Debug.Log("Ray " + mainLooper + "Food " + food[hitLooper]);
			//Debug.Log("Ray " + mainLooper + "Wall " + wall[hitLooper]);
		}

		startingValues.Clear();

		startingValues.AddRange(wall);
		startingValues.AddRange(body);
		startingValues.AddRange(food);
	}

	void CalcFitness()
	{
		//Fitness is based on length and lifetime
		if(length < 10)
		{
			fitness = (int)Mathf.Floor(lifetime * lifetime * Mathf.Pow(2, (Mathf.Floor(length))));
		}
		else
		{
			fitness = (((lifetime * lifetime) * (int)Mathf.Pow(2,10)) * length - 9);
		}
	}

	void Dead()
	{
		//Debug.Log("Dead");
		GameObject temp;
		CalcFitness();
		
		CancelInvoke();

		wait = false;
		alive = false;

		networkController.GetComponent<NetworkManager>().NextNetwork(fitness);

		while(tail.Count > 0)
		{
			temp = tail[0];
			tail.RemoveAt(0);
			Destroy(temp);
		}

		Reset();
	}
}
