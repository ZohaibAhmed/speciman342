using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HudHandler : MonoBehaviour {

	public Slider healthBarSlider;
	public Text points;
	public Text timeText;

	private float totalTime = 300f; // this is 300 seconds (5 mins)

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		// subtract Time.deltaTime from totalTime
		totalTime = totalTime - Time.deltaTime;

		// convert totalTime to minutes and seconds
		System.TimeSpan t = System.TimeSpan.FromSeconds( totalTime );

		string answer = string.Format("{0:D2}:{1:D2}", 
		                              t.Minutes, 
		                              t.Seconds
		                              );
		
		timeText.text = answer;

	}

	public void changeHealth(float change) {
		healthBarSlider.value = healthBarSlider.value + change;
	}

	public void incrementPoints(int addPoints) {
		int totalPoints = int.Parse(points.text);
		int finalPoints = totalPoints + addPoints;

		points.text = finalPoints.ToString ();
	}
}
