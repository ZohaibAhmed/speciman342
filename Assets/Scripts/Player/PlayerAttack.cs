using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float timeBetweenAttacks = 0.15f;
	public float attackRange = 0.5f;
	public float attackDamage = 1f;
	public string regularAttackInput;
	public string hardAttackInput;


	float bigHandsDuration = 0f;
	float bigHandsDamage;
	float bigHandsAttackRadiusMultiplier = 100;

	Animator anim;	
	float timer;

	int destructableMask;

	PlayerScoreCounter scoreCounter;
	PlayerControl playerControl;

	// Use this for initialization
	void Start ()
	{

	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		anim = GetComponent<Animator> ();
		scoreCounter = GetComponent<PlayerScoreCounter>();
		playerControl = GetComponent<PlayerControl>();
		timer = timeBetweenAttacks;
	}

	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if (Input.GetButtonDown(regularAttackInput) && timer >= timeBetweenAttacks && Time.timeScale != 0){
			Debug.Log("Attack!");
			anim.SetTrigger("Attack");
			Attack (attackDamage);
		} else if (Input.GetButtonDown(hardAttackInput) && timer >= timeBetweenAttacks * 2 && Time.timeScale != 0) {
			anim.SetTrigger("HardAttack");
			playerControl.hardAttack(timeBetweenAttacks * 2f);
			Attack(attackDamage * 2);
		}

		if (bigHandsDuration > 0){
			bigHandsDuration -= Time.deltaTime;
		}

	}

	void Attack(float damageDealt){
		RaycastHit[] hits;
		timer = 0f;
		
		Vector3 direction = transform.forward;

		if (bigHandsDuration > 0){
			damageDealt = bigHandsDamage;
		} 

		// + transform.forward.normalized * transform.lossyScale.x / 4


		float range = attackRange + transform.lossyScale.x * 0.3f;

		hits = Physics.CapsuleCastAll(transform.position, 
		                              transform.position + Vector3.up * transform.localScale.y,
		                              transform.lossyScale.z * 0.35f, 
		                              direction,
		                              range);
		Debug.DrawRay(transform.position , direction * range, Color.red, 2, false);
		Debug.DrawRay(transform.position + Vector3.up * transform.lossyScale.y, direction * range, Color.red, 2, false);

		int i = 0;
		while (i < hits.Length){
			RaycastHit hit = hits[i];
			Debug.Log("hit: " + hit.collider.gameObject.ToString());
			int layerMask = 1 << hit.collider.gameObject.layer;
			if ((layerMask & destructableMask) > 0){
				if (bigHandsDuration > 0 || transform.lossyScale.y >= hit.transform.lossyScale.y * 0.5f){
					//chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
					Destructable other = hit.collider.GetComponent<Destructable>();
					if (other == null){
						Debug.Log("No destructable script: " + hit.collider.gameObject.ToString());
					}
					if (damageDealt >= other.health){
						scoreCounter.incrementScore(other.points);
						Debug.Log (other.gameObject.tag);
						if (other.gameObject.tag == "RadioactiveTruck"){
							playerControl.Grow(playerControl.radioactiveTruckGrowth);
						} else if(other.gameObject.tag == "RecyclingPlant"){
							Debug.Log("Recycling plant destroyed");
							playerControl.Grow(playerControl.wasteDisposalFacilityGrowth);
						} else if (other.gameObject.tag == "NuclearPowerplant"){
							playerControl.Grow(playerControl.nuclearPowerPlantGrowth);
						}
					}

					other.takeDamage(damageDealt);
				}
			} else if (hit.collider.tag == "Player" && hit.collider.gameObject.name != this.gameObject.name){
				Debug.Log("HIT!!");
				PlayerHealth otherPlayer = hit.collider.GetComponent<PlayerHealth>();
				otherPlayer.takeDamage(damageDealt);
				if (otherPlayer.getHealth() <= 0){
					anim.SetTrigger("Win");
					playerControl.faceCamera();
					playerControl.enabled = false;
					this.enabled = false;
				}
			}
			i++;
		}
		Debug.Log(hits.Length);
	}

	public void updateAttackDamage(float increase){
		attackDamage += increase;
	}

	public void updateAttackRange(float increase){
		attackRange += increase;
	}

	public void activateBigHands(){
		bigHandsDamage = attackDamage * 2;
		bigHandsDuration = 30f;
	}
}

