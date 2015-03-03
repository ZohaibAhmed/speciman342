using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	public Transform firstPerson; // take the transform from the first person camera

	public float timeBetweenShots = 0.15f;
	public float range = 100f;
	public float damagePerShot = 1f;
	public GameObject player;

	float timer;
	Ray shootRay;
	RaycastHit shootHit;
	int destructableMask;
	LineRenderer shootLine;
	float effectsDisplayTime = 0.2f;

	Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position;
	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		shootLine = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		//Quaternion rotation = Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0);
		//Vector3 position = rotation * offset;
		
		//transform.position = position;
		//transform.rotation = rotation;

		Debug.DrawRay(transform.position, transform.forward);
		timer += Time.deltaTime;
		if (Input.GetButtonDown("Fire1") && timer >= timeBetweenShots && Time.timeScale != 0){
			Shoot ();
		}

		if (timer >= timeBetweenShots * effectsDisplayTime){
			DisableEffects();
		}
	}

	public void DisableEffects(){
		shootLine.enabled = false;
	}

	void Shoot(){
		timer = 0f;

		shootLine.enabled = true;
		shootLine.SetPosition(0, transform.position);



		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;

		if (Physics.Raycast(shootRay, out shootHit, range, destructableMask)){
			Destructable destructable = shootHit.collider.GetComponent<Destructable>();
			if (destructable != null){
				destructable.takeDamage(damagePerShot);
			}
			shootLine.SetPosition(1, shootHit.point);
		} else {
			shootLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
		}
	}
}
