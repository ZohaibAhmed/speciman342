﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public GameObject target;
	public Camera cam;

	public float distance;

	float defaultCameraFOV = 35f;
	float widestCameraFOV = 100f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float ySpeed = 120.0f;

	public string mouseYInput;

	public bool playerWon = false;

	float y = 0.0f;

	float shake_decay;
	float shake_intensity;
	float shake_delay;


	Vector3 offset;
//	Animator anim;
	
		

	void Start(){
		Vector3 angles = transform.eulerAngles;
		y = angles.x;
//		anim = GetComponent<Animator>();
	}

	void Update() {

	}

	void LateUpdate() {
		y += Input.GetAxis(mouseYInput) * ySpeed * 0.02f;
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);

		if (target){

			float targetRotation = target.transform.eulerAngles.y;

			Quaternion rotation;
			Vector3 normalPosition;
			if (!playerWon){
				rotation = Quaternion.Euler(y, targetRotation, 0);
				normalPosition = (rotation * new Vector3(0, 0, -distance)) + target.transform.position;
				transform.rotation = rotation;
			} else {
				//TODO
				rotation = Quaternion.Euler(y, targetRotation + 180f, 0);
				normalPosition = (rotation * new Vector3(0, 0, -distance)) + target.transform.position;
				transform.rotation = rotation;
				//transform.LookAt(target.transform.position);
			}

			Quaternion originRotation = transform.rotation;

			if (shake_delay > 0){
				shake_delay -= Time.deltaTime;
			}

			if (shake_intensity > 0 && shake_delay <= 0){

				transform.position = normalPosition + Random.insideUnitSphere * shake_intensity;
				transform.rotation = new Quaternion(
					originRotation.x + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.y + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.z + Random.Range (-shake_intensity,shake_intensity) * .2f,
					originRotation.w + Random.Range (-shake_intensity,shake_intensity) * .2f);
				shake_intensity -= shake_decay;
			} else {
				transform.position = normalPosition;
			}

			Vector3 targetRightBound = target.transform.position + (target.transform.right.normalized * target.transform.lossyScale.x);
			Vector3 targetLeftBound = target.transform.position - target.transform.right.normalized * target.transform.lossyScale.x;

			// make sure that there is nothing between the camera and the target
			float d = Vector3.Distance(transform.position, target.transform.position);
			float dR = Vector3.Distance(transform.position, targetRightBound);
			float dL = Vector3.Distance(transform.position, targetLeftBound);

			var relativePos = transform.position - (target.transform.position);
			var relativePosR = transform.position - targetRightBound;
			var relativePosL = transform.position - targetLeftBound;

			RaycastHit hit;
			RaycastHit hitRight;
			RaycastHit hitLeft;
			Debug.DrawRay(target.transform.position, relativePos.normalized * d, Color.green);
			Debug.DrawRay(targetRightBound, relativePosR.normalized * dR, Color.red);
			Debug.DrawRay(targetLeftBound, relativePosL.normalized * dL, Color.red);
			bool clipping =  Physics.Raycast(target.transform.position, relativePos, out hit, d) &&
							 Physics.Raycast(targetRightBound, relativePosR, dR) &&
							 Physics.Raycast(targetLeftBound, relativePosL, dL);
			if (clipping && hit.transform != target && hit.collider.tag != "Player"){ // make sure the hit is not with the target

				Debug.DrawLine(target.transform.position, hit.point, Color.red);
				if (hit.collider.tag == "Bomb"){ // ignore bombs
					return;
				} 
				transform.position = hit.point;
				//transform.LookAt(target.transform);
				float regularDistance = Vector3.Distance(normalPosition, target.transform.position);
				float currentDistance = Vector3.Distance(normalPosition, hit.point);
				cam.fieldOfView = Mathf.Lerp(defaultCameraFOV, widestCameraFOV, currentDistance/regularDistance);
			} else {
				cam.fieldOfView = defaultCameraFOV;
			}
		}

	}


	float ClampAngle(float angle, float min, float max){
		if (angle < -360){
			angle += 360;
		} if (angle > 360){
			angle -= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public void Shake(float intensity, float decay, float delay){
		shake_intensity = intensity;
		shake_decay = decay;
		shake_delay = delay;
	}
}
