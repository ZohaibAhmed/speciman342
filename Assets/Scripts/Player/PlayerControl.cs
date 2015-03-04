using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public float movementSpeed = 2.0f;
	public float turningSpeed = 2.0f;
	public float attackRange = 1.5f;
	public float growthFactor = 1.25f;
	public float attackDamage = 1f;
	public GameObject gun;

	Rigidbody playerRigidbody;

	private bool growing = false; // true if the character is growing
	private float maxGrowLerpTime = 1f;
	private float currentGrowLerpTime = 0.0f;
	
	private Vector3 currentScale;
	private Vector3 nextScale;

	float oldCameraDistance;
	float newCameraDistance;

	int destructableMask;

	public ChemicalSpawnManager chemicalSpawnManager;
	public CameraControl cameraControl;

	float speedShoeDuration = 0f;
	float speedShoeSpeed;

	float bigHandsDuration = 0f;
	float bigHandsDamage;
	float bigHandsAttackRadiusMultiplier = 100;

	float gunDuration = 0f;
	
	Vector3 movement;

	// Use this for initialization
	void Start () {
	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		playerRigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate(){
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		Move (h, v);
		Rotate (0, Input.GetAxis("Mouse X"), 0);
	}

	// Update is called once per frame
	void Update () {
		//Move ();

		
		if (Input.GetButtonDown("Fire1")){
			if (!gun || gunDuration <= 0){
				Attack ();
			}
		}
		if (growing == true){
			Grow ();
		} else {
			currentGrowLerpTime = 0.0f;
		}

		if (speedShoeDuration > 0){
			speedShoeDuration -= Time.deltaTime;
		}

		if (bigHandsDuration > 0){
			bigHandsDuration -= Time.deltaTime;
		}

		if (gunDuration > 0 && gun != null){
			gunDuration -= Time.deltaTime;
			if (gunDuration <= 0){
				gun.SetActive(false);
			}
		}
	}

	void Move(float h, float v){
		//this.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * this.turningSpeed * 0.02f);
		//movement.Set(h, 0f, v);
		movement = playerRigidbody.transform.forward * v + playerRigidbody.transform.right * h;



		float speed;
		if (speedShoeDuration > 0){
			speed = speedShoeSpeed;
		} else {
			speed = movementSpeed;
		}

		movement = movement.normalized * speed * Time.deltaTime;
		playerRigidbody.MovePosition(transform.position + movement);

		//rigidbody.MovePosition(this.transform.right * Input.GetAxis("Horizontal") * speed) ;
		//rigidbody.MovePosition(this.transform.forward * Input.GetAxis("Vertical") * speed);
	}

	void Rotate(float x, float y, float z){
		Vector3 rotation = new Vector3(x, y, z) * this.turningSpeed * 0.02f;
		rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotation));
	}


	void Attack(){
		RaycastHit[] hits;
		Vector3 direction = transform.forward;
		float damageDealt = attackDamage;
		float attackRadius = transform.lossyScale.x / 2;

		if (bigHandsDuration > 0){
			attackRadius =  attackRadius * bigHandsAttackRadiusMultiplier;
			damageDealt = bigHandsDamage;
		} 
		
		hits = Physics.CapsuleCastAll(transform.position - Vector3.up * transform.localScale.y / 2, 
		                    transform.position + Vector3.up * transform.localScale.y / 2,
		                    transform.lossyScale.x / 2, 
		                    direction,
		                    attackRange,
		                    destructableMask);
		int i = 0;
		while (i < hits.Length){
			RaycastHit hit = hits[i];

			if (bigHandsDuration > 0 || transform.lossyScale.y >= hit.transform.lossyScale.y){
				//chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
				Destructable other = hit.collider.GetComponent<Destructable>();
				other.takeDamage(damageDealt);
			}
			i++;
		}

	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Chemical"){
			updateScales();
			growing = true;
			Destroy(other.gameObject);
		} else if (other.gameObject.tag == "Speed Shoe"){
			speedShoeSpeed = movementSpeed * 2;
			speedShoeDuration = 30f;
			Destroy (other.gameObject);
		} else if (other.gameObject.tag == "Big Hands"){
			bigHandsDamage = attackDamage * 2;
			bigHandsDuration = 30f;
			Destroy (other.gameObject);
		} else if (other.gameObject.tag == "Gun"){
			if (gun){
				gun.SetActive(true);
				gunDuration = 30f;
			}
			Destroy(other.gameObject);
		}
	}

	void OnCollisionEnter(Collision collision){
		int layerMask = 1 << collision.gameObject.layer;
		if ((layerMask & destructableMask) > 0){
			if (transform.localScale.y >= collision.transform.lossyScale.y * 4){
				Destructable destructable = collision.gameObject.GetComponent<Destructable>();
				destructable.destruct();
			}
		}
	}
	
	void updateScales(){
		currentScale = this.transform.localScale;
		nextScale = new Vector3(this.transform.localScale.x * (growthFactor - 0.1f), this.transform.localScale.y * growthFactor, this.transform.localScale.z * (growthFactor - 0.1f));

		oldCameraDistance = cameraControl.distance;
		newCameraDistance = oldCameraDistance + (growthFactor + (transform.localScale.y / 2));

		movementSpeed = movementSpeed + (0.5f * growthFactor);
		//turningSpeed = turningSpeed * growthFactor;

		attackDamage += 1;
	}
	
	void Grow(){
		currentGrowLerpTime += Time.deltaTime;
		if (currentGrowLerpTime > maxGrowLerpTime) {
			currentGrowLerpTime = maxGrowLerpTime;
			growing = false;
			return;
		}
		
		float perc = currentGrowLerpTime / maxGrowLerpTime;
		
		this.transform.localScale = Vector3.Lerp(currentScale, nextScale, perc);

		var newYPosition = Mathf.Lerp (this.currentScale.y, this.nextScale.y / 2, perc);
		Vector3 position = new Vector3(this.transform.position.x, newYPosition, this.transform.position.z);
		playerRigidbody.transform.position = position;

		cameraControl.distance = Mathf.Lerp(oldCameraDistance, newCameraDistance, perc);
	}
}
