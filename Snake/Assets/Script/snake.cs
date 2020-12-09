using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class snake : MonoBehaviour
{

	Vector2 dir = Vector2.right;

	List<Transform> tail = new List<Transform>();

	private int count = 0;

	bool ate = false;
	bool moved = true;

	public GameObject tailPrefab;
	

	// Use this for initialization
	void Start () {
		InvokeRepeating("Move", 0.1f, 0.1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(moved)
		{
			if(Input.GetKey(KeyCode.RightArrow) && dir != -Vector2.right)
			{
				dir = Vector2.right;
			}
			else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
			{
				dir = -Vector2.up;
			}
			else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
			{
				dir = -Vector2.right;
			}
			else if(Input.GetKey(KeyCode.UpArrow) && dir != -Vector2.up)
			{
				dir = Vector2.up;
			}

			moved = false;
		}
		
	}

	void Move()
	{
		//Save current position (gap will go here)
		Vector2 v = transform.position;

		//Move head
		transform.Translate(dir);

		if(ate)
		{
			//Load prefab
			GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);

			//Add to list
			tail.Insert(0, g.transform);

			//reset flag
			ate = false;
		}

		//Is there a tail
		else if(tail.Count > 0)
		{
			//Move last tail Element to where the Head was
			tail[tail.Count - 1].position = v;

			//Add to fron of the list, remove from back
			tail.Insert(0, tail[tail.Count - 1]);
			tail.RemoveAt(tail.Count - 1);
		}

		moved = true;
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(coll.tag == "Food")
		{
			ate = true;

			count++;

			Destroy(coll.gameObject);
		}
		else
		{
			CancelInvoke();
		}
	}
}
