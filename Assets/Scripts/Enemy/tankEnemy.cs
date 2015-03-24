using UnityEngine;
using System.Collections;

public class tankEnemy : MonoBehaviour {
	public float Speed = 5;
	public int spawnPoint;
	public PlayerControl player;

	public EnemySpawnManager enemySpawnManager;

	CharacterController controller;

	public GameObject bullet;
	private static float lastBullet;
	private float bulletInterval; // this is the interval in which bombs are going to be created 

	public AudioClip shootSound;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		controller = this.GetComponent<CharacterController> ();
		player = FindObjectOfType<PlayerControl> ();

		lastBullet = 0.0f;
		bulletInterval = (Random.value + 0.5f) * 2f;

		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnPoint == 0 || spawnPoint == 1) {
			// this should move from left to right
			controller.Move (Vector3.right * Speed * Time.deltaTime);
		} else if (spawnPoint == 2 || spawnPoint == 3) {
			controller.Move (Vector3.right * -Speed * Time.deltaTime);
		} 

		if (spawnPoint == 4 || spawnPoint == 5) {
			// this should move from left to right
			controller.Move (Vector3.right * Speed * Time.deltaTime);
		} else if (spawnPoint == 6 || spawnPoint == 7) {
			controller.Move (Vector3.right * -Speed * Time.deltaTime);
		} 

		// TODO: follow a path.

		// Find if a player is near me so I can point and shoot

		Vector3 pos = new Vector3(player.transform.position.x, 
                                  transform.Find ("Tower").gameObject.transform.position.y, 
                                  player.transform.position.z) ;

		transform.Find ("Tower").transform.LookAt(pos);

		// The model for this tank is weird, it not oriented correctly. This fixes the orientation.
		transform.Find ("Tower").transform.eulerAngles = new Vector3(
			transform.Find ("Tower").transform.eulerAngles.x + 270,
			transform.Find ("Tower").transform.eulerAngles.y + 180,
			transform.Find ("Tower").transform.eulerAngles.z
		);

		// TODO: shoot bullets if close enough to player
		float dist = Vector3.Distance(player.transform.position, this.transform.position);

		if (dist <= 50.0f && Time.time - lastBullet > bulletInterval) {
			// we're in firing range

			float distance = 5;

			GameObject b = (GameObject)Instantiate(bullet, transform.Find("Tower").transform.position + (transform.Find("Tower").transform.up * distance), Quaternion.identity);
			b.GetComponent<Rigidbody>().AddForce(transform.Find("Tower").transform.up * 500);

			// play shoot sound
			source.PlayOneShot(shootSound, 0.7f);
			
			lastBullet = Time.time;
		}
	
		// If Im outside the scene for any reason, kill me...
		RaycastHit hit;
		if (!Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			enemySpawnManager.decrementTankCount(spawnPoint);
			Destroy(this.gameObject);
		}

	}
}
