using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float movementSpeed = 2.0f;
	public float turningSpeed = 2.0f;
	public float attackRange = 1.5f;
	public float growthFactor = 1.25f;
	public float attackDamage = 1f;

	CharacterController controller;
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

	// Use this for initialization
	void Start () {
		this.controller = this.GetComponent<CharacterController>();
	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
	}
	
	// Update is called once per frame
	void Update () {
		Move ();

		
		if (Input.GetButtonDown("Fire1")){
			Attack ();
		}
		if (growing == true){
			Grow ();
		} else {
			currentGrowLerpTime = 0.0f;
		}
	}

	void Move(){
		//this.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0) * this.turningSpeed);
		this.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * this.turningSpeed * 0.02f);
		controller.Move(this.transform.right * Input.GetAxis("Horizontal") * this.movementSpeed) ;
		controller.Move(this.transform.forward * Input.GetAxis("Vertical") * this.movementSpeed);
	}


	void Attack(){
		RaycastHit hit;
		Vector3 direction = new Vector3(0, 0, 0);
		direction = transform.forward;
		
		Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
		if (Physics.Raycast(position, direction, out hit, attackRange, destructableMask)){
			if (transform.lossyScale.y >= hit.transform.lossyScale.y){
				//chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
				Destructable other = hit.collider.GetComponent<Destructable>();
				other.takeDamage(attackDamage);
			}
		}
	}
	
	void OnTriggerEnter(Collider other){
		Debug.Log ("am in here?");
		if (other.gameObject.tag == "Chemical"){
			updateScales();
			growing = true;
			Destroy(other.gameObject);
		}
	}
	
	void updateScales(){
		currentScale = this.transform.localScale;
		nextScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * growthFactor, this.transform.localScale.z);

		oldCameraDistance = cameraControl.distance;
		newCameraDistance = oldCameraDistance * growthFactor;

		movementSpeed = movementSpeed * growthFactor;
		turningSpeed = turningSpeed * growthFactor;
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
		Debug.Log(this.transform.localScale);
		
		var newYPosition = Mathf.Lerp (this.currentScale.y, this.nextScale.y / 2, perc);
		Vector3 position = new Vector3(this.transform.position.x, newYPosition, this.transform.position.z);
		this.transform.position = position;

		cameraControl.distance = Mathf.Lerp(oldCameraDistance, newCameraDistance, perc);
	}
}
