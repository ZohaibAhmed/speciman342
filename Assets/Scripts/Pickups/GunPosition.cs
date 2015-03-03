using UnityEngine;
using System.Collections;

public class GunPosition : MonoBehaviour {

	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public Transform holder;
	
	float y = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	
	// Update is called once per frame
	void Update () {
		y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		Quaternion rotation = Quaternion.Euler(y, holder.eulerAngles.y, 0);
		transform.rotation = rotation;

		Vector3 position = (rotation * new Vector3(holder.lossyScale.x ,0,holder.lossyScale.z)) 
						+ new Vector3(holder.position.x, holder.lossyScale.y - transform.lossyScale.y, holder.position.z);
		transform.position = position;
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
