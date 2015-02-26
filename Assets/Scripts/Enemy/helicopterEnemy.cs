using UnityEngine;
using System.Collections;

public class helicopterEnemy : MonoBehaviour {
	public float Speed = 5;
	public int spawnPoint;
	public PlayerMovement player;

	public GameObject ground;


	public GameObject bomb;
	private static float lastBomb;
	private int bombInterval; // this is the interval in which bombs are going to be created 

	CharacterController controller;

	// Use this for initialization
	void Start () {
		controller = this.GetComponent<CharacterController> ();
		player = FindObjectOfType<PlayerMovement> ();
		lastBomb = 0.0f;
		bombInterval = 2;
	}
	
	// Update is called once per frame
	void Update () {

		if (spawnPoint >= 0 && spawnPoint <= 2) {
			// this should move from left to right
			controller.Move (Vector3.right * Speed * Time.deltaTime);
		} else if (spawnPoint >= 3 && spawnPoint <= 5) {
			controller.Move (Vector3.right * -Speed * Time.deltaTime);
		} else if (spawnPoint >= 6) {
			// this helicopter will point towards the player and move there
			// note: will only rotate with respect to y-axis
			Vector3 targetPostition = new Vector3( player.transform.position.x, 
			                                      this.transform.position.y, 
			                                      player.transform.position.z ) ;

			this.transform.LookAt( targetPostition );

			controller.Move (this.transform.forward * Speed * Time.deltaTime);
		}

		// everywhere we go, we need to drop a bomb there...
		if (Time.time - lastBomb > bombInterval) {
			Debug.Log ("DROPPED BOMB");

			float distance = 2;
			Instantiate(bomb, this.transform.position + (-this.transform.up * distance), Quaternion.identity);

			lastBomb = Time.time;
		}

		// check to see if we are outside the stage, so we can destroy ourselves
		RaycastHit hit;
		if (!Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			Destroy(this.gameObject);
		}



	}
}
