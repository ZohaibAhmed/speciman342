using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float movementSpeed = 2.0f;
	public float turningSpeed = 2.0f;
	public float growthFactor = 1.25f;
	public float maxSize = 40f;
	public Transform[] enemies;

	float currentHealth;
	float timer;

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

	public string horizontalInput;
	public string verticalInput;
	public string mouseXInput;
	public string lockOnInput;

	float speedShoeDuration = 0f;
	float speedShoeSpeed;

	Vector3 movement;
	Animator anim;
	PlayerAttack playerAttack;
	PlayerHealth playerHealth;

	// Use this for initialization
	void Start () {
	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		playerRigidbody = GetComponent<Rigidbody>();
		anim = GetComponent<Animator> ();
		playerAttack = GetComponent<PlayerAttack> ();
		playerHealth = GetComponent<PlayerHealth> ();
	}

	void FixedUpdate(){
		float h = Input.GetAxis(horizontalInput);
		float v = Input.GetAxis(verticalInput);
		float y = Input.GetAxis(mouseXInput);
		Move (h, v);
		if (Input.GetAxis(lockOnInput) > 0){
			LockOn();
		} else {
			Rotate (0, y, 0);
		}
		Animating(h, v, y);
	}

	// Update is called once per frame
	void Update () {
		//Move ();
		if (growing == true){
			updateScales ();
		} else {
			currentGrowLerpTime = 0.0f;
		}

		if (speedShoeDuration > 0){
			speedShoeDuration -= Time.deltaTime;
		}
	}

	void Move(float h, float v){
		movement = playerRigidbody.transform.forward * v + playerRigidbody.transform.right * h;

		float speed;
		if (speedShoeDuration > 0){
			speed = speedShoeSpeed;
		} else {
			speed = movementSpeed;
		}

		movement = movement.normalized * speed * Time.deltaTime;
		playerRigidbody.MovePosition(transform.position + movement);
	}

	void Rotate(float x, float y, float z){
		Vector3 rotation = new Vector3(x, y, z) * this.turningSpeed * 0.02f;
		rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(rotation));
	}

	// makes the gecko face the closest enemy
	void LockOn(){
		float closestEnemy = 0f;
		Transform target = null;
		foreach (Transform t in enemies){
			float d = Vector3.Distance(this.transform.position, t.position);
			if (closestEnemy == 0){
				target = t;
				closestEnemy = d;
			} else if (closestEnemy > 0 && d < closestEnemy){
				closestEnemy = d;
				target = t;
			}
		}
		if (target != null){
			Vector3 lockOnPosition = new Vector3(target.position.x,
			                                     this.transform.position.y,
			                                     target.position.z);
			transform.LookAt(lockOnPosition);
		}
	}

	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Chemical"){
			if (this.transform.lossyScale.y <= maxSize){ 
				Grow(this.growthFactor);
				currentGrowLerpTime = 0;
				Destroy(other.gameObject);
			}
		} else if (other.gameObject.tag == "Speed Shoe"){
			speedShoeSpeed = movementSpeed * 2;
			speedShoeDuration = 30f;
			Destroy (other.gameObject);
		} else if (other.gameObject.tag == "Big Hands"){
			playerAttack.activateBigHands();
			Destroy (other.gameObject);
		} 
	}

	void OnCollisionEnter(Collision collision){
		int layerMask = 1 << collision.gameObject.layer;
		if ((layerMask & destructableMask) > 0){
			if (transform.localScale.y >= collision.transform.lossyScale.y * 4){
				Destructable destructable = collision.gameObject.GetComponent<Destructable>();

				if (collision.gameObject.tag == "RadioactiveTruck"){
					Grow(1.05f);
				}
				destructable.destruct();
			}
		}
	}
	
	public void Grow(float growthAmount = 1.05f){
		if (this.transform.lossyScale.y * growthAmount >= maxSize){
			if (this.transform.lossyScale.y < maxSize){
				growing = true;
				currentScale = this.transform.localScale;
				oldCameraDistance = cameraControl.distance;
				nextScale = new Vector3(maxSize, maxSize, maxSize);
				newCameraDistance = 6 * maxSize;
				movementSpeed = 4 * maxSize;
				playerAttack.updateAttackDamage(5f);
			}
			return;
			
		}

		growing = true;
		currentScale = this.transform.localScale;
		nextScale = new Vector3(this.transform.localScale.x + growthAmount,
		                        this.transform.localScale.y + growthAmount, 
		                        this.transform.localScale.z + growthAmount);
		//nextScale = new Vector3(this.transform.localScale.x + growthFactor, this.transform.localScale.y + growthFactor, this.transform.localScale.z + growthFactor);


		oldCameraDistance = cameraControl.distance;
		//newCameraDistance = oldCameraDistance + (growthAmount + (transform.localScale.y / 2));
		newCameraDistance = 6 * nextScale.y;

		movementSpeed = movementSpeed + growthAmount;
		//movementSpeed = 5 * this.transform.localScale.x;
		//turningSpeed = turningSpeed * growthFactor;

		playerAttack.updateAttackDamage(0.75f * growthAmount);
		playerAttack.updateAttackRange(0.25f * growthAmount);


	}


	
	void updateScales(){
		currentGrowLerpTime += Time.deltaTime;
		if (currentGrowLerpTime > maxGrowLerpTime) {
			currentGrowLerpTime = maxGrowLerpTime;
			growing = false;
			return;
		}
		
		float perc = currentGrowLerpTime / maxGrowLerpTime;
		
		this.transform.localScale = Vector3.Lerp(currentScale, nextScale, perc * 2);

		//float newYPosition = this.transform.localScale.y / 2;
		float newYPosition = 0f;
		Vector3 position = new Vector3(this.transform.position.x, newYPosition, this.transform.position.z);
		playerRigidbody.transform.position = position;
		cameraControl.distance = Mathf.Lerp(oldCameraDistance, newCameraDistance, perc);
	}

	/* trigger the animator
	 * h is the horizontal movement
	 * v is the vertical movement
	 * y is the rotatation about the y axis (gecko is turning if y is not 0)
	 */
	void Animating(float h, float v, float y){
		bool walking = h != 0f || v != 0f;
		bool rotating  = y != 0f;
		if (walking){
			bool shuffling = Mathf.Abs(h) > Mathf.Abs(v);
			bool shufflingLeft;
			if (shuffling){
				shufflingLeft = h < 0;
				anim.SetBool("IsShufflingLeft", shufflingLeft);
				anim.SetBool("IsShufflingRight", !shufflingLeft);
			} else {
				anim.SetBool("IsShufflingLeft", false);
				anim.SetBool("IsShufflingRight", false);
			}
			anim.SetBool("IsWalking", !shuffling);
			anim.SetBool("IsTurningLeft", false);
			anim.SetBool("IsTurningRight", false);
		} else if (rotating){
			anim.SetBool("IsWalking", false);
			anim.SetBool("IsShufflingLeft", false);
			anim.SetBool("IsShufflingRight", false);
			anim.SetBool("IsTurningLeft",  y < 0);
			anim.SetBool("IsTurningRight", y > 0);
		} else {
			anim.SetBool("IsWalking", false);
			anim.SetBool("IsShufflingLeft", false);
			anim.SetBool("IsShufflingRight", false);
			anim.SetBool("IsTurningLeft", false);
			anim.SetBool("IsTurningRight", false);
		}
	}
}
