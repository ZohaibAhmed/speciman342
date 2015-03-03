using UnityEngine;
using System.Collections;

public class FirstPersonCamera : MonoBehaviour {


	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	float y = 0.0f;

	// Use this for initialization
	void Start () {
	}

	
	// Update is called once per frame
	void Update () {
		y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

		y = ClampAngle(y, yMinLimit, yMaxLimit);
		Quaternion rotation = Quaternion.Euler(y, transform.parent.eulerAngles.y, 0);
		transform.rotation = rotation;
	}

	float ClampAngle(float angle, float min, float max){
		if (angle < -360){
			angle += 360;
		} if (angle > 360){
			angle -= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public float getYRotation(){
		return this.y;
	}
}
