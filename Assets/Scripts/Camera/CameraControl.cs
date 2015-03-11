using UnityEngine;
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

	float y = 0.0f;

	Vector3 offset;

	void Start(){
		Vector3 angles = transform.eulerAngles;
		y = angles.x;

	}

	void LateUpdate() {

		if (target){
			y += Input.GetAxis(mouseYInput) * ySpeed * 0.02f;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			float targetRotation = target.transform.eulerAngles.y;

			Quaternion rotation = Quaternion.Euler(y, targetRotation, 0);
			Vector3 normalPosition = (rotation * new Vector3(0, 0, -distance)) + target.transform.position;

			transform.rotation = rotation;
			transform.position = normalPosition;

			// make sure that there is nothing between the camera and the target
			float d = Vector3.Distance(transform.position, target.transform.position);
			var relativePos = transform.position - (target.transform.position);
			RaycastHit hit;
			Debug.DrawRay(target.transform.position, relativePos.normalized * d, Color.green);
			bool clipping =  Physics.Raycast(target.transform.position, relativePos, out hit, d);
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
}
