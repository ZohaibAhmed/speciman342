using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public float horizontalDistance = 8f;
	public float verticalDistance = 10f;

	public GameObject target;

	Vector3 offset;

	void Start(){
		offset = new Vector3(0, verticalDistance, -horizontalDistance);
	}

	void Update(){
		offset = new Vector3(0, verticalDistance, -horizontalDistance);
	}

	void LateUpdate() {
		float desiredAngle = target.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
		transform.position = target.transform.position + (rotation * offset);
		transform.LookAt(target.transform);
	}
}
