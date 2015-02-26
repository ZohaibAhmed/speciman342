using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour {

	public GameObject helicopter;
	public GameObject copter;
	public GameObject[] SpawnPoints;

	public static float lastCopter;
	public int spawnInterval;

	//public string[] names = {"Down", "Right", "Up"};

	// Use this for initialization
	void Start () {
		lastCopter = 0.0f;
		spawnInterval = Random.Range (3, 5);
	}
	
	// Update is called once per frame
	void Update () {
	
		// spawn a helicopter
		if (Time.time - lastCopter > spawnInterval){
			int spawn = Random.Range(0, 5);
			copter = Instantiate(helicopter, SpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			helicopterEnemy enemy = copter.GetComponent(typeof(helicopterEnemy)) as helicopterEnemy;

			// set the direction
			enemy.spawnPoint = spawn;

			lastCopter = Time.time;
		}
	}
}
