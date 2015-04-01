using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float movementSpeed = 2.0f;
	public float turningSpeed = 2.0f;
	public float growthFactor = 1.25f;
	public float maxSize = 40f;
	public Transform enemy;
	public float radioactiveTruckGrowth = 2f;
	public float wasteDisposalFacilityGrowth = 30f;
	public float nuclearPowerPlantGrowth = 80f;
	public GameObject ExclamationMark;
	public float FranticThresholdDistance = 750f;

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

	AudioSource audioSource;
	public AudioClip jump;
	public AudioClip land;

	float hardAttackDuration = 0f;

	float speedShoeDuration = 0f;
	float speedShoeSpeed;

	bool knockingback;
	Vector3 knockBackDirection;
	float knockbackDamage;

	Vector3 movement;
	Animator anim;
	PlayerAttack playerAttack;
	PlayerHealth playerHealth;

	bool isFrantic;

	// Use this for initialization
	void Start () {
	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		playerRigidbody = GetComponent<Rigidbody>();
		anim = GetComponent<Animator> ();
		playerAttack = GetComponent<PlayerAttack> ();
		playerHealth = GetComponent<PlayerHealth> ();
		currentScale = this.transform.localScale;
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	void FixedUpdate(){
		float h = Input.GetAxis(horizontalInput);
		float v = Input.GetAxis(verticalInput);
		float y = Input.GetAxis(mouseXInput);

		if (knockingback){
			Debug.Log("knockback!");
			playerRigidbody.MovePosition (transform.position + knockBackDirection.normalized * knockbackDamage);

			knockingback = false;
			return;
		}

		if (hardAttackDuration <= 0){
			Move (h, v);
			if (Input.GetAxis(lockOnInput) > 0){
				LockOn();
			} else {
				Rotate (0, y, 0);
			}
		} else {
			hardAttackDuration -= Time.deltaTime;
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

		if (!isFrantic && checkFrantic())
		{
			movementSpeed = movementSpeed * 8f;
			Vector3 lockOnPosition = new Vector3(enemy.position.x,
			                                     this.transform.position.y,
			                                     enemy.position.z);
			transform.LookAt(lockOnPosition);
			anim.SetTrigger("Surprise");
			anim.SetBool("Frantic", true);
			if (ExclamationMark){
				ExclamationMark.SetActive(true);
			}
			isFrantic = true;
		} else if (isFrantic){
			if (enemy.lossyScale.y <= 2f* transform.lossyScale.y){
				movementSpeed = movementSpeed / 8f;
				anim.SetBool("Frantic", false);
				if (ExclamationMark){
					ExclamationMark.SetActive(false);
				}
				isFrantic = false;
			}
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
		GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler(rotation));
	}

	// makes the gecko face the closest enemy
	void LockOn(){
		if (enemy != null){
			Vector3 lockOnPosition = new Vector3(enemy.position.x,
			                                     this.transform.position.y,
			                                     enemy.position.z);
			transform.LookAt(lockOnPosition);
		}
	}

	bool checkFrantic(){
		if (enemy){
			return Vector3.Distance(enemy.position, transform.position) <= FranticThresholdDistance &&
				   enemy.lossyScale.y >= 2f * transform.lossyScale.x;
		}

		return false;
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Chemical"){
			if (this.transform.lossyScale.y <= maxSize){ 
				Grow(this.growthFactor);
				currentGrowLerpTime = 0;
				Destroy(other.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision collision){
		int layerMask = 1 << collision.gameObject.layer;
		if ((layerMask & destructableMask) > 0){
			if (transform.localScale.y >= collision.transform.lossyScale.y * 4){
				Destructable destructable = collision.gameObject.GetComponent<Destructable>();

				if (collision.gameObject.tag == "RadioactiveTruck"){
					Grow(this.radioactiveTruckGrowth);
				}
				destructable.destruct();
			}
		}
	}

	public void faceCamera(){
		//cameraControl.enabled = false;
		cameraControl.playerWon = true;
		transform.LookAt(new Vector3(cameraControl.transform.position.x,
		                             0,
		                             cameraControl.transform.position.z));
	}

	// set hardattackduration so movement is disabled while its happening!
	public void hardAttack(float duration){
		// TODO: jump?
		AudioSource.PlayClipAtPoint(jump, transform.position);

		hardAttackDuration = duration;
		cameraControl.Shake(0.06f, 0.006f, 0.7f); // shake the camera
	}
	
	public void Grow(float growthAmount = 1.05f){
//		i (this.transform.lossyScale.y + growthAmount >= maxSize){
//			if (this.transform.lossyScale.y < maxSize){
//				growing = true;
//				currentScale = this.transform.localScale;
//				oldCameraDistance = cameraControl.distance;
//				nextScale = new Vector3(maxSize, maxSize, maxSize);
//				newCameraDistance = 6 * maxSize;
//				movementSpeed = 4 * maxSize;
//				playerAttack.updateAttackDamage(5f);
//			}
//			return;
//			
//		}

		growing = true;
		nextScale = new Vector3(currentScale.x + growthAmount,
		                        currentScale.y + growthAmount, 
		                        currentScale.z + growthAmount);
		currentScale = nextScale;

		//nextScale = new Vector3(this.transform.localScale.x + growthFactor, this.transform.localScale.y + growthFactor, this.transform.localScale.z + growthFactor);


		oldCameraDistance = cameraControl.distance;
		//newCameraDistance = oldCameraDistance + (growthAmount + (transform.localScale.y / 2));
		newCameraDistance = 6 * nextScale.y;

		if (isFrantic){
			movementSpeed = movementSpeed + growthAmount * 6f;
		} else {
			movementSpeed = movementSpeed + growthAmount;
		}
		//movementSpeed = 5 * this.transform.localScale.x;
		//turningSpeed = turningSpeed * growthFactor;

		playerAttack.updateAttackDamage(0.5f * growthAmount);
		playerAttack.updateAttackRange(0.20f * growthAmount);

		playerHealth.incrementHealth(growthAmount * 2f);


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
		float newYPosition = -0.005f * transform.localScale.y;
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
				anim.SetBool("IsWalkingForward", false);
				anim.SetBool("IsWalkingBackward", false);
			} else {
				anim.SetBool("IsShufflingLeft", false);
				anim.SetBool("IsShufflingRight", false);
				anim.SetBool("IsWalkingForward", v > 0);
				anim.SetBool("IsWalkingBackward", v < 0);
			}

			anim.SetBool("IsTurningLeft", false);
			anim.SetBool("IsTurningRight", false);
		} else if (rotating){
			anim.SetBool("IsWalkingForward", false);
			anim.SetBool("IsWalkingBackward", false);
			anim.SetBool("IsShufflingLeft", false);
			anim.SetBool("IsShufflingRight", false);
			anim.SetBool("IsTurningLeft",  y < 0);
			anim.SetBool("IsTurningRight", y > 0);
		} else {
			anim.SetBool("IsWalkingForward", false);
			anim.SetBool("IsWalkingBackward", false);
			anim.SetBool("IsShufflingLeft", false);
			anim.SetBool("IsShufflingRight", false);
			anim.SetBool("IsTurningLeft", false);
			anim.SetBool("IsTurningRight", false);
		}
	}

	public void knockBack(Vector3 direction, float damage) {
		knockingback = true;
		knockBackDirection = direction;
		knockbackDamage = damage;
	}
}
