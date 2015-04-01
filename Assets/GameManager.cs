using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public GameObject canvasP1;
	public GameObject canvasP2;
	public GameObject cameraP1;
	public GameObject cameraP2;

	public GameObject city2;

	public GameObject FlyByTargets;
	public GameObject startCamera;
	public GameObject startScreenTarget;
	public GameObject startScreenGUI;

	public string startInput;

	private float volLowRange = .5f;
	private float volHighRange = 1.0f;

	private float totalTime = 300f; // this is 300 seconds (5 mins)

	PlayerHealth p1Health;
	PlayerHealth p2Health;

	PlayerScoreCounter p1Score;
	PlayerScoreCounter p2Score;

	HudHandler p1Hud;
	HudHandler p2Hud;

	bool gameOver;

	void Awake() {
	}

	// Use this for initialization
	void Start () {

		p1Health = player1.GetComponent<PlayerHealth>();
		p2Health = player2.GetComponent<PlayerHealth>();

		p1Score = player1.GetComponent<PlayerScoreCounter>();
		p2Score = player2.GetComponent<PlayerScoreCounter>();

		p1Hud = canvasP1.GetComponent<HudHandler>();
		p2Hud = canvasP2.GetComponent<HudHandler>();
	}

	void Update(){
		totalTime -= Time.deltaTime;
	}

	void LateUpdate () {

		if (p1Health.getHealth() <= 0){
			p1Hud.gameOver();
			p2Hud.win();
			gameOver = true;
		} else if (p2Health.getHealth() <= 0){
			p1Hud.win();
			p2Hud.gameOver();
			gameOver = true;
		} 

		if (totalTime <= 0){
			if (p1Score.getScore() > p2Score.getScore()){
				p1Hud.win();
				p2Hud.gameOver();
				gameOver = true;
			} else {
				p1Hud.gameOver();
				p2Hud.win();
				gameOver = true;
			}
		}

		if (gameOver){
			if (Input.GetButtonDown(startInput)){
				Application.LoadLevel(Application.loadedLevel);
			}
		}

	}

	public void StartGame(){

		player1.SetActive(true);
		player2.SetActive(true);
		canvasP1.SetActive(true);
		canvasP2.SetActive(true);

		startCamera.SetActive(false);
		startScreenTarget.SetActive(false);
		FlyByTargets.SetActive(false);
		startScreenGUI.SetActive(false);
		LoadCity2();
		Destroy(startCamera);
		Destroy(startScreenTarget);
		Destroy(FlyByTargets);
		Destroy(startScreenGUI);
	}

	public void UnloadCity2(){
		city2.SetActive(false);
	}

	public void LoadCity2(){
		city2.SetActive(true);
	}


}
