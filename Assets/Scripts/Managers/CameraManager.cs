using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public GameObject ThirdPersonCamera;
	public GameObject FirstPersonCamera;

	public string cameraToggleInput;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(Input.GetAxis("CameraToggle"));
		if (Input.GetAxis(cameraToggleInput) > 0){
			ThirdPersonCamera.SetActive(false);
			FirstPersonCamera.SetActive(true);
		} else {
			ThirdPersonCamera.SetActive(true);
			FirstPersonCamera.SetActive(false);
		}
	}
}
