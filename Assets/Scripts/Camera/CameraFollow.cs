using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject target;
	public Camera cam;

	float defaultCameraFOV = 35f;
	float widestCameraFOV = 100f;

	Vector3 offset;

	void Start(){
		offset = transform.position - target.transform.position;
	}

	void Update(){
		//offset = new Vector3(0, verticalDistance, -horizontalDistance);
	}

	public Vector3 getOffset(){
		return this.offset;
	}

	public void updateOffset(Vector3 newOffset){
		this.offset = newOffset;
	}

	void LateUpdate() {
		float desiredAngle = target.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
		Vector3 normalPosition = target.transform.position + (rotation * offset);
		transform.position = normalPosition;
		transform.LookAt(target.transform);

		// make sure that there is nothing between the camera and the target
		float d = Vector3.Distance(transform.position, target.transform.position);
		var relativePos = transform.position - (target.transform.position);
		RaycastHit hit;
		bool clipping =  Physics.Raycast(target.transform.position, relativePos, out hit, d);
		if (clipping && hit.transform != target){ // make sure the hit is not with the target
			Debug.DrawLine(target.transform.position, hit.point);
			transform.position = hit.point;
			transform.LookAt(target.transform);
			float regularDistance = Vector3.Distance(normalPosition, target.transform.position);
			float currentDistance = Vector3.Distance(normalPosition, hit.point);
			cam.fieldOfView = Mathf.Lerp(defaultCameraFOV, widestCameraFOV, currentDistance/regularDistance);
		} else {
			cam.fieldOfView = defaultCameraFOV;
		}
	}
}
