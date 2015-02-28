using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	public float speed = 20;
	public GameObject dropZone;
	public GameObject dropZoneClone;
	//CharacterController controller;

	public HudHandler hud;
	public PlayerMovement player;

	// Use this for initialization
	void Start () {
		// create a new dropZone
		Vector3 targetPosition = new Vector3( this.transform.position.x, 
		                                      2.0f, 
		                                      this.transform.position.z ) ;

		dropZoneClone = Instantiate (dropZone, targetPosition, Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;

		player = FindObjectOfType<PlayerMovement> ();
		hud = FindObjectOfType<HudHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
		//controller.Move (-this.transform.up * speed * Time.deltaTime);
		//transform.Translate (-Vector3.up * speed * Time.deltaTime);
	}

	float Euclidean(float x1, float x2, float y1, float y2) {
		return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
	}


	void OnCollisionEnter (Collision col) {
		if(col.gameObject.name == "Ground")
		{
			Destroy(this.gameObject);
			Destroy(dropZoneClone.gameObject);
			// can we find how far it is from the player? 
//			this.transform.position.x
			float damage = Euclidean(player.transform.position.x, this.transform.position.x, player.transform.position.y, this.transform.position.y);
			Debug.Log (damage);
			if (damage < 5.0f) {
				hud.changeHealth(-damage);
			}
		}

		if (col.gameObject.name == "Player") {
			Destroy(this.gameObject);
			Destroy(dropZoneClone.gameObject);

			hud.changeHealth(-10.0f);
		}

	}
	
}
