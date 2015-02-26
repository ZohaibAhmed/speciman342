﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float movementSpeed = 1.0f;
	public float turningSpeed = 1.0f;
	public float attackRange = 1.5f;

	CharacterController controller;
	Rigidbody playerRigidbody;
	
	private bool growing = false; // true if the character is growing
	private float maxGrowLerpTime = 1f;
	private float currentGrowLerpTime = 0.0f;
	
	private Vector3 currentScale;
	private Vector3 nextScale;

	float oldCameraHorizontalDistance;
	float newCameraHorizontalDistance;
	float oldCameraVerticalDistance;
	float newCameraVerticalDistance;

	int destructableMask;

	public ChemicalSpawnManager chemicalSpawnManager;
	public CameraFollow cameraFollow;

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

		
		if (Input.GetAxis("Fire1") > 0){
			Attack ();
		}
		if (growing == true){
			Grow ();
		} else {
			currentGrowLerpTime = 0.0f;
		}
	}

	void Move(){
		this.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0) * this.turningSpeed);
		controller.Move(this.transform.forward * Input.GetAxis("Vertical") * this.movementSpeed);
	}


	void Attack(){
		RaycastHit hit;
		Vector3 direction = new Vector3(0, 0, 0);
		if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0){
			direction = transform.forward;
		} else {
			direction = direction + Input.GetAxis("Vertical") * new Vector3(0, 0, 1) + Input.GetAxis("Horizontal") * new Vector3(1, 0, 0);
		}
		
		Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
		if (Physics.Raycast(position, direction, out hit, attackRange, destructableMask)){
			Debug.Log(hit.transform.lossyScale.y);
			if (transform.lossyScale.y >= hit.transform.lossyScale.y){
				chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
				Destroy(hit.transform.gameObject);
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
		nextScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y * 2, this.transform.localScale.z);
		oldCameraHorizontalDistance = cameraFollow.horizontalDistance;
		newCameraHorizontalDistance = oldCameraHorizontalDistance * 2;
		oldCameraVerticalDistance = cameraFollow.verticalDistance;
		newCameraVerticalDistance = oldCameraVerticalDistance * 2;
		movementSpeed = movementSpeed + 0.5f;
		turningSpeed = turningSpeed + 0.5f;
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

		cameraFollow.horizontalDistance = Mathf.Lerp(this.oldCameraHorizontalDistance, this.newCameraHorizontalDistance, perc);
		cameraFollow.verticalDistance = Mathf.Lerp(this.oldCameraVerticalDistance, this.newCameraVerticalDistance, perc);

	}
}
