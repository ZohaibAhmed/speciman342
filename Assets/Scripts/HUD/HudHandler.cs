using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	public Slider healthBarSlider;
	public Text pointsText;
	public Text timeText;
	public bool restart;
	public gameOverScreen screenOver;
	public PlayerHealth playerHealth;
	public PlayerScoreCounter playerScore;
	Animator anim;

	private float totalTime = 300f; // this is 300 seconds (5 mins)


	// Use this for initialization
	void Start () {
		restart = false;
		anim = GetComponent <Animator> ();
		screenOver = FindObjectOfType<gameOverScreen> ();
		healthBarSlider.maxValue = playerHealth.maxHealth;
	}
	
	// Update is called once per frame
	void Update () {

		// update the health
		healthBarSlider.value = playerHealth.getHealth();
		healthBarSlider.maxValue = playerHealth.maxHealth;

		pointsText.text = playerScore.getScore().ToString("D9");

		if (playerHealth.getHealth() <= 0){
			gameOver();
		}

		if (restart) {
			if (screenOver){
				screenOver.enabled = true;
				screenOver.FadeToBlack();
			}
			if (Input.GetKeyDown (KeyCode.R)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		} else {

			// subtract Time.deltaTime from totalTime
			totalTime = totalTime - Time.deltaTime;

			// convert totalTime to minutes and seconds
			System.TimeSpan t = System.TimeSpan.FromSeconds (totalTime);

			string answer = string.Format ("{0:D2}:{1:D2}", 
			                              t.Minutes, 
			                              t.Seconds
			);
			
			timeText.text = answer;

			// times up
			if (totalTime <= 0) {
				gameOver ();
			}
		}

	}

	public void changeHealth(float change) {
		healthBarSlider.value = healthBarSlider.value + change;

		if (healthBarSlider.value == 0.0f) {
			// you're dead
			gameOver();
		}
	}	

	public void incrementPoints(int addPoints) {
		int totalPoints = int.Parse(pointsText.text);
		int finalPoints = totalPoints + addPoints;

		pointsText.text = finalPoints.ToString ();
	}

	public void gameOver() {
		Debug.Log("Game over");

		anim.SetTrigger ("GameOver");
		// stop the game...
		//Time.timeScale = 0;
		// set flag to true
		//restart = true;


	}
}
