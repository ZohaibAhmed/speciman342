using UnityEngine;
using System.Collections;

public class ChemicalSpawnManager : MonoBehaviour {


	public GameObject chemical;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnChemical(Vector3 position){
		if (Random.value > 0.3){ // spawn chemical drop 70% of the time.
			Instantiate(chemical, position, Quaternion.identity);
		}
	}
}
