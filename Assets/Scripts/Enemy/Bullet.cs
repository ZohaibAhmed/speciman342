using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float speed = 0.00000001f;
	public Transform explosion;
	
	public HudHandler hud;
	public PlayerControl player;

	public float SecondsUntilDestroy = 100.0f;
	public float startTime;

	// Use this for initialization
	void Start () {
		hud = FindObjectOfType<HudHandler> ();
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time - startTime >= SecondsUntilDestroy) {
			Destroy(this.gameObject);
		} 

	}

	void OnCollisionEnter (Collision col) {
		Debug.Log ("collided with " + col.gameObject.name);
		if (col.gameObject.name == "Player") {
			Destroy (this.gameObject);
			PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
			playerHealth.takeDamage(10.0f);
		} else if (col.gameObject.name != "Bullet(Clone)") {
			// we collided with something else... just explode.
			Destroy(this.gameObject);
			Instantiate(explosion, this.transform.position, Quaternion.identity);
		}

	}
}
