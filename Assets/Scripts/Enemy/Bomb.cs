using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	public float speed = 20;
	public GameObject dropZone;
	public GameObject dropZoneClone;
	//CharacterController controller;

	// Use this for initialization
	void Start () {
		// create a new dropZone
		Vector3 targetPosition = new Vector3( this.transform.position.x, 
		                                      2.0f, 
		                                      this.transform.position.z ) ;

		dropZoneClone = Instantiate (dropZone, targetPosition, Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//controller.Move (-this.transform.up * speed * Time.deltaTime);
		//transform.Translate (-Vector3.up * speed * Time.deltaTime);
	}


	void OnCollisionEnter (Collision col) {
		if(col.gameObject.name == "Ground")
		{
			Destroy(this.gameObject);
			Destroy(dropZoneClone.gameObject);
		}
	}
	
}
