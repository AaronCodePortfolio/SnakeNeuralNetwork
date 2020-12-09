using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
	public GameObject foodPrefab;

	private List<GameObject> food = new List<GameObject>();

	public Transform borderTop;
	public Transform borderBottom;
	public Transform borderLeft;
	public Transform borderRight;

	// Use this for initialization
	void Start ()
	{
		InvokeRepeating("Spawn", 3, 2);
		food.Add((GameObject)Instantiate(foodPrefab, new Vector2(14, 0), Quaternion.identity));
	}

	public void Reset()
	{
		GameObject temp;
		while (food.Count > 0)
		{
			temp = food[0];
			food.RemoveAt(0);
			Destroy(temp);
		}

		food.Add((GameObject)Instantiate(foodPrefab, new Vector2(14, 0), Quaternion.identity));
	}

	void Spawn()
	{
		int x = (int)Random.Range(borderLeft.position.x, borderRight.position.x);
		int y = (int)Random.Range(borderBottom.position.y, borderTop.position.y);

		food.Add((GameObject)Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity));
	}
	
}
