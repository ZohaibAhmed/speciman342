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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
