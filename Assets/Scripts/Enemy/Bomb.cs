using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	public float speed = 20;
	public GameObject dropZone;
	public GameObject dropZoneClone;
	//CharacterController controller;

	public Transform explosion;

	public HudHandler hud;
	public PlayerControl player;

	public AudioClip bombSound;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		// create a new dropZone
		Vector3 targetPosition = new Vector3( this.transform.position.x, 
		                                      2.0f, 
		                                      this.transform.position.z ) ;

		dropZoneClone = Instantiate (dropZone, targetPosition, Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;

		player = FindObjectOfType<PlayerControl> ();
		hud = FindObjectOfType<HudHandler> ();
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		//controller.Move (-this.transform.up * speed * Time.deltaTime);
		//transform.Translate (-Vector3.up * speed * Time.deltaTime);

		// Get rid of me
		RaycastHit hit;
		if (!Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			Destroy(this.gameObject);
		}
	}

	float Euclidean(float x1, float x2, float y1, float y2) {
		return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
	}


	void OnCollisionEnter (Collision col) {
		// play sound for bomb
		source.PlayOneShot(bombSound, 0.7f);
		Debug.Log ("collided with " + col.gameObject.name);
		if(col.gameObject.name == "Ground")
		{
			Destroy(this.gameObject);
			Destroy(dropZoneClone.gameObject);
			// can we find how far it is from the player? 
//			this.transform.position.x
			float damage = Euclidean(player.transform.position.x, this.transform.position.x, player.transform.position.y, this.transform.position.y);
			Debug.Log (damage);
			if (damage < 5.0f) {
				player.gameObject.GetComponent<PlayerHealth>().takeDamage((5.0f - damage));
			}

			// explode!
			Instantiate(explosion, this.transform.position, Quaternion.identity);
		}

		if (col.gameObject.name == "Player") {
			Destroy(this.gameObject);
			Destroy(dropZoneClone.gameObject);

			PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
			playerHealth.takeDamage(10.0f);
		}


	}
	
}
