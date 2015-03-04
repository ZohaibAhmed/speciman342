using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {
	
	public float timeBetweenShots = 0.15f;
	public float range = 100f;
	public float damagePerShot = 1f;
	public FirstPersonCamera firstPersonCamera;
	public GameObject fire;

	float timer;
	Ray shootRay;
	RaycastHit shootHit;
	int destructableMask;
	LineRenderer shootLine;
	float effectsDisplayTime = 0.2f;

	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	float y = 0.0f;

	Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position;

		if (firstPersonCamera){
			y = firstPersonCamera.getYRotation();
		}

	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		shootLine = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		//Quaternion rotation = Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0);
		//Vector3 position = rotation * offset;
		
		//transform.position = position;
		//transform.rotation = rotation;

		y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		Quaternion rotation = Quaternion.Euler(y, transform.parent.eulerAngles.y, 0);
		transform.rotation = rotation;

		Debug.DrawRay(transform.position, transform.forward);
		timer += Time.deltaTime;
		if (Input.GetButtonDown("Fire1") && timer >= timeBetweenShots && Time.timeScale != 0){
			Shoot ();
		}

		if (timer >= timeBetweenShots * effectsDisplayTime){
			DisableEffects();
		}
	}

	public void DisableEffects(){
		shootLine.enabled = false;
	}

	void Shoot(){
		timer = 0f;

		shootLine.enabled = true;
		shootLine.SetPosition(0, transform.position);



		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;

		if (Physics.Raycast(shootRay, out shootHit, range, destructableMask)){
			Destructable destructable = shootHit.collider.GetComponent<Destructable>();
			if (destructable != null){
				destructable.takeDamage(damagePerShot);
			}
			shootLine.SetPosition(1, shootHit.point);
			Instantiate(fire, shootHit.transform.position, Quaternion.identity);
		} else {
			shootLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
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
