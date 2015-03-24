using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour {

	public GameObject helicopter;
	public GameObject copter;
	public GameObject tank;
	public GameObject tankObject;

	public GameObject[] SpawnPoints;
	public GameObject[] tankSpawnPoints; // this handles only tanks
	public int tankCount;

	public static float lastCopter;
	public static float lastTank;
	public int spawnInterval;
	public int tankSpawnInterval;

	public float initialDelay = 60f; // initially delay the spawn by 60 seconds

	//public string[] names = {"Down", "Right", "Up"};

	// Use this for initialization
	void Start () {
		lastCopter = 0.0f;
		lastTank = 0.0f;
		tankCount = 0;
		spawnInterval = Random.Range (5, 20);
		tankSpawnInterval = Random.Range (10, 20);
	}
	
	// Update is called once per frame
	void Update () {

		if (initialDelay > 0){
			initialDelay -= Time.deltaTime;
			return;
		}
	
		if (Time.time - lastCopter > spawnInterval){
			int spawn = Random.Range(0, 7);
			copter = Instantiate(helicopter, SpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			helicopterEnemy enemy = copter.GetComponent(typeof(helicopterEnemy)) as helicopterEnemy;

			// set the direction
			enemy.spawnPoint = spawn;

			lastCopter = Time.time;
		}

		if (Time.time - lastTank > tankSpawnInterval && tankCount < 4) {
			int spawn = Random.Range(0, 5);
			Debug.Log ("Spawning Tank");

			tankObject = Instantiate(tank, tankSpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			tankEnemy enemy = tankObject.GetComponent(typeof(tankEnemy)) as tankEnemy;

			enemy.spawnPoint = spawn;

			if (spawn == 1) {
				enemy.transform.Rotate (0, 90, 0);
			} else if (spawn == 2) {
				enemy.transform.Rotate (0, 180, 0);
			} else if (spawn == 3) {
				enemy.transform.Rotate(0, -90, 0);
			}

			tankCount++;

			lastTank = Time.time;
		}
	}

	public void decrementTankCount () {
		tankCount = tankCount - 1;
	}
}
