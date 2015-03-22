using UnityEngine;
using System.Collections;

public class PlayerScoreCounter : MonoBehaviour {

	int score = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void incrementScore(int score){
		this.score += score;
	}

	public int getScore(){
		return this.score;
	}
}
