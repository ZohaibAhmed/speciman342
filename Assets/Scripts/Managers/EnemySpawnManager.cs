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

	public static float lastTankMirror;
	public int tankMirrorSpawnInterval;
	public int tankMirrorCount;

	public float initialDelay = 15f; // initially delay the spawn by 60 seconds

	//public string[] names = {"Down", "Right", "Up"};

	// Use this for initialization
	void Start () {
		lastCopter = 0.0f;
		lastTank = 0.0f;
		lastTankMirror = 0.0f;
		tankCount = 0;
		tankMirrorCount = 0;
		spawnInterval = Random.Range (5, 20);
		tankSpawnInterval = Random.Range (10, 20);
		tankMirrorSpawnInterval = Random.Range (10, 20);
	}
	
	// Update is called once per frame
	void Update () {

//		if (initialDelay > 0){
//			initialDelay -= Time.deltaTime;
//			return;
//		}
//	
		if (Time.time - lastCopter > spawnInterval){
			int spawn = Random.Range(0, 7);
			copter = Instantiate(helicopter, SpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			helicopterEnemy enemy = copter.GetComponent(typeof(helicopterEnemy)) as helicopterEnemy;

			// set the direction
			enemy.spawnPoint = spawn;

			lastCopter = Time.time;
		}

		if (Time.time - lastTank > tankSpawnInterval && tankCount < 4) {
			int spawn = Random.Range(0, 8);

			tankObject = Instantiate(tank, tankSpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			tankEnemy enemy = tankObject.GetComponent(typeof(tankEnemy)) as tankEnemy;

			enemy.spawnPoint = spawn;

			if (spawn == 2 || spawn == 3) {
				enemy.transform.Rotate (0, 180, 0);
			} 
			tankCount++;

			lastTank = Time.time;
		}

		// TODO: I know this isn't efficient, but we're just going to spawn tanks on the other side here..
		if (Time.time - lastTankMirror > tankMirrorSpawnInterval && tankMirrorCount < 4) {
			int spawn = Random.Range(4, 8);

			tankObject = Instantiate(tank, tankSpawnPoints[spawn].transform.position, Quaternion.identity) as GameObject;
			tankEnemy enemy = tankObject.GetComponent(typeof(tankEnemy)) as tankEnemy;
			
			enemy.spawnPoint = spawn;
			
			if (spawn == 6 || spawn == 7) {
				enemy.transform.Rotate (0, 180, 0);
			} 

			tankMirrorCount++;
			lastTankMirror = Time.time;
		}

	}

	public void decrementTankCount (int spawnPoint) {
		if (spawnPoint < 4) {
			tankCount--;
		} else {
			tankMirrorCount--;
		}
	}
}
