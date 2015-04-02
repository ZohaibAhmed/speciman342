using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartScreenCamera : MonoBehaviour {

	public class Transition{
		public float speed;
		public float startTime;
		public float journeyLength;
		public Transform startPosition;
		public Transform targetPosition;
		public Transform target;

		public Transition(float startTime, Transform startPos, Transform targetPos, Transform target, float s = 25.0f){
			this.startTime = startTime;
			startPosition = startPos;
			targetPosition = targetPos;
			speed = s;
			this.target = target;
			journeyLength = Vector3.Distance(startPos.position, targetPos.position);
		}

	}

	public Transform startScreenTarget;
	public Transform playerOne;
	public float rotateSpeed;
	public float startScreenTargetDistance = 600.0f;
	public string startInput;
	public GameObject startScreenGraphics;
	public Transform ChemPickupCamPos;
	public Transform WasteFacilCamPos;
	public Transform RadioTruckCamPos;
	public Transform NuclearPlantCamPos;
	public Transform CityCamPos;

	public Transform ChemPickupTarget;
	public Transform WasteFacilTarget;
	public Transform RadioTruckTarget;
	public Transform NuclearPlantTarget;
	public Transform CityTarget;
	public Text tutorialText;
	public Text startToSkip;
	public float DisplayTextTime = 5f;

	public GameManager gameManager;

	float x;

	bool gameStarted;
	bool transition1;
	bool transition2;
	bool trans3;
	bool trans4;
	bool trans5;
	bool trans6;
	bool trans7;
	bool trans8;
	bool trans9;

	float timeBetweenTransitions;
	
	// private variables for first transition to top of player
	// refer to this as transition 1
	private float transition1speed = 100.0F;
	private float transition1StartTime;
	private float transition1JourneyLength;
	private Vector3 transition1StartPosition;
	private Vector3 transition1StartRotation;
	private Vector3 transition1TargetPosition;
	private Vector3 transition1TargetRotation;

	// transition from sky down to player
	private float transition2speed = 100.0F;
	private float transition2StartTime;
	private float transition2JourneyLength;
	private Vector3 transition2StartPosition;
	private Vector3 transition2TargetPosition;

	//transition from player to chemical pickup
	private Transition transition3;

	//transition from chemical pickup to waste facility
	private Transition transition4;

	//transition from waste facility to truck
	private Transition transition5;

	//transition from truck to power plant
	private Transition transition6;

	// transition from power plant to city
	private Transition transition7;

	// Use this for initialization
	void Start () {
		gameStarted = false;
		transition1 = false;
		transition2 = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(startInput)){
			if (!gameStarted){
				Debug.Log("press");
				gameStarted = true;
				FadeStartScreen();
				transition1StartTime = Time.time;
				transition1StartPosition = transform.position;
				transition1StartRotation = transform.eulerAngles;
				transition1TargetPosition = new Vector3(playerOne.position.x,
				                             transform.position.y,
				                             playerOne.position.z);
				transition1TargetRotation = new Vector3(90, 0, 0);
				transition1JourneyLength = Vector3.Distance(transition1StartPosition, transition1TargetPosition);
				transition1 = true;
				startToSkip.enabled = true;
			} else {
				gameManager.StartGame();
			}
		}

		if (!gameStarted){
			RotateAroundTarget(startScreenTarget, startScreenTargetDistance);
		} 

		if (transition1){
			TransitionOntopOfPlayer();
			if (!transition1){
				transition2StartTime = Time.time;
				transition2StartPosition = transform.position;
				transition2TargetPosition = playerOne.transform.position + new Vector3(0, 1, -3f);
				transition2JourneyLength = Vector3.Distance(transition2StartPosition, transition2TargetPosition);
				transition2 = true;
			}
		}

		if (transition2){
			transition2 = !TransitionAndLookAtTarget(transition2StartPosition, transition2TargetPosition, playerOne.position, transition2StartTime, transition2speed, transition2JourneyLength);
			trans3 = !transition2;
			if (trans3){
				gameManager.UnloadCity2();
				transition3 = new Transition(Time.time + DisplayTextTime / 2, transform, ChemPickupCamPos, ChemPickupTarget);
				timeBetweenTransitions = DisplayTextTime / 2;
			}
		}


		if (trans3){
			if (timeBetweenTransitions > 0){
				timeBetweenTransitions -= Time.deltaTime;
			} else {
				trans3 = !TransitionAndLookAtTarget(transition3.startPosition.position, 
				                                    transition3.targetPosition.position, 
				                                    transition3.target.position, 
				                                    transition3.startTime, 
				                                    transition3.speed, 
				                                    transition3.journeyLength);
				trans4 = !trans3;
				if (trans4){
					transition4 = new Transition(Time.time + DisplayTextTime, ChemPickupCamPos, WasteFacilCamPos, WasteFacilTarget);
					timeBetweenTransitions = DisplayTextTime / 2;
				}
			}
		}

		if (trans4){
			if (timeBetweenTransitions > 0){
				tutorialText.enabled = true;
				tutorialText.text = "Pick up radioactive barrels to grow larger!";
				timeBetweenTransitions -= Time.deltaTime;
			} else {
				tutorialText.enabled = false;
				trans4 = !TransitionAndLookAtTarget(transition4.startPosition.position,
				                                    transition4.targetPosition.position,
				                                    transition4.target.position,
				                                    transition4.startTime,
				                                    transition4.speed,
				                                    transition4.journeyLength);
				trans5 = !trans4;
				if (trans5){
					transition5 = new Transition(Time.time + DisplayTextTime, WasteFacilCamPos, RadioTruckCamPos, RadioTruckTarget);
					timeBetweenTransitions = DisplayTextTime / 2;
				}
			}
		}



		if (trans5){
			if (timeBetweenTransitions > 0){
				tutorialText.enabled = true;
				tutorialText.text = "Destroy the radioactive waste facility to grow really big!";
				timeBetweenTransitions -= Time.deltaTime;
			} else {
				tutorialText.enabled = false;
				trans5 = !TransitionAndLookAtTarget(transition5.startPosition.position,
				                                    transition5.targetPosition.position,
				                                    transition5.target.position,
				                                    transition5.startTime,
				                                    transition5.speed,
				                                    transition5.journeyLength);
				trans6 = !trans5;
				if (trans6){
					transition6 = new Transition(Time.time + DisplayTextTime, RadioTruckCamPos, NuclearPlantCamPos, NuclearPlantTarget);
					timeBetweenTransitions = DisplayTextTime / 2;
				}
			}
		}

		if (trans6){
			if (timeBetweenTransitions > 0){
				tutorialText.enabled = true;
				tutorialText.text = "Destroy the radioactive trucks to grow bigger!";
				timeBetweenTransitions -= Time.deltaTime;
			} else {
				tutorialText.enabled = false;
				trans6 = !TransitionAndLookAtTarget(transition6.startPosition.position,
				                                    transition6.targetPosition.position,
				                                    transition6.target.position,
				                                    transition6.startTime,
				                                    transition6.speed,
				                                    transition6.journeyLength);
				trans7 = !trans6;
				if (trans7){
					gameManager.LoadCity2();
					transition7 = new Transition(Time.time + DisplayTextTime, NuclearPlantCamPos, CityCamPos, CityTarget);
					timeBetweenTransitions = DisplayTextTime / 2;
				}
			}
		}

		if (trans7){
			if (timeBetweenTransitions > 0){
				tutorialText.enabled = true;
				tutorialText.text = "Destroy the nuclear power plant to grow REALLY big!";
				timeBetweenTransitions -= Time.deltaTime;
			}  else {
				tutorialText.enabled = false;
				trans7 = !TransitionAndLookAtTarget(transition7.startPosition.position,
				                                    transition7.targetPosition.position,
				                                    transition7.target.position,
				                                    transition7.startTime,
				                                    transition7.speed,
				                                    transition7.journeyLength);
				trans8 = !trans7;
				if (trans8){
					timeBetweenTransitions = DisplayTextTime / 2;
				}
			}
		}

		if (trans8){
			if (timeBetweenTransitions > 0){
				tutorialText.enabled = true;
				tutorialText.text = "Defeat your opponent!";
				timeBetweenTransitions -= Time.deltaTime;
			} else {
				gameManager.StartGame();
			}
		}
		
	}

	void RotateAroundTarget(Transform rotateTarget, float d){
		x += rotateSpeed * 0.02f * Time.deltaTime;
		
		x = ClampAngle(x, 0, 360);
		
		float targetRotation = rotateTarget.eulerAngles.x;
		
		Quaternion rotation = Quaternion.Euler(targetRotation, x, 0);
		Vector3 normalPosition = (rotation * new Vector3(0, d / 2, -d)) + rotateTarget.position;
		
		transform.LookAt(rotateTarget);
		transform.position = normalPosition;
	}

	void TransitionOntopOfPlayer(){
		float distCovered = (Time.time - transition1StartTime) * transition1speed;
		float fracJourney = distCovered / transition1JourneyLength;
		transform.position = Vector3.Lerp(transition1StartPosition, transition1TargetPosition, fracJourney);
		transform.eulerAngles = Vector3.Lerp(transition1StartRotation, transition1TargetRotation, fracJourney);


		if (fracJourney >= 1.0f){
			transition1 = false;
		}
	}

	// moves camera to targetPosition while looking at target
	// returns true if the transition is compelte
	bool TransitionAndLookAtTarget(Vector3 origin, Vector3 targetPosition, Vector3 target, float startTime, float transitionSpeed, float journeyLength){
		float distCovered = (Time.time - startTime) * transitionSpeed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(origin, targetPosition, fracJourney);
		transform.LookAt(target);

		Quaternion desiredRotation = Quaternion.LookRotation(target - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 0.01f);
		
		return fracJourney >= 1.0f;
	}

	void FadeStartScreen(){
		Animator startScreenGraphicsAnim = startScreenGraphics.GetComponent<Animator>();
		if (startScreenGraphicsAnim){
			startScreenGraphicsAnim.SetTrigger("StartPressed");
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
