using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float timeBetweenAttacks = 0.15f;
	public float attackRange = 1.5f;
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
			anim.SetTrigger("Attack");
			Attack (attackDamage);
		} else if (Input.GetButtonDown(hardAttackInput) && timer >= timeBetweenAttacks * 2 && Time.timeScale != 0) {
			anim.SetTrigger("HardAttack");
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
		float attackRadius = transform.lossyScale.x / 2;

		if (bigHandsDuration > 0){
			attackRadius =  attackRadius * bigHandsAttackRadiusMultiplier;
			damageDealt = bigHandsDamage;
		} 
		
		hits = Physics.CapsuleCastAll(transform.position, 
		                              transform.position + Vector3.up * transform.localScale.y,
		                              transform.lossyScale.x / 2, 
		                              direction,
		                              attackRange);
		Debug.DrawRay(transform.position, direction, Color.cyan);
		Debug.DrawRay(transform.position + Vector3.up * transform.lossyScale.y, direction, Color.cyan);
		int i = 0;
		while (i < hits.Length){
			RaycastHit hit = hits[i];
			Debug.Log("hit!");
			int layerMask = 1 << hit.collider.gameObject.layer;
			if ((layerMask & destructableMask) > 0){
				if (bigHandsDuration > 0 || transform.lossyScale.y >= hit.transform.lossyScale.y * 0.75){
					//chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
					Destructable other = hit.collider.GetComponent<Destructable>();
					if (damageDealt >= other.health){
						scoreCounter.incrementScore(other.points);
						Debug.Log (other.gameObject.tag);
						if (other.gameObject.tag == "RadioactiveTruck"){
							playerControl.updateScales(1.05f);
						} else if(other.gameObject.tag == "RecyclingPlant"){
							Debug.Log("Recycling plant destroyed");
							playerControl.updateScales(2f);
						} else if (other.gameObject.tag == "NuclearPowerplant"){
							playerControl.updateScales(2f);
						}
					}

					other.takeDamage(damageDealt);
				}
			} else if (hit.collider.tag == "Player" && hit.collider.gameObject.name != this.gameObject.name){
				Debug.Log("HIT!!");
				PlayerHealth otherPlayer = hit.collider.GetComponent<PlayerHealth>();
				Debug.Log(otherPlayer.getHealth());
				otherPlayer.takeDamage(damageDealt);
			} else {
				Debug.Log("no match");
			}
			i++;
		}
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

