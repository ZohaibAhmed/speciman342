using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float timeBetweenAttacks = 0.15f;
	public float attackRange = 1.5f;
	public float attackDamage = 1f;
	public string fireInput;


	float bigHandsDuration = 0f;
	float bigHandsDamage;
	float bigHandsAttackRadiusMultiplier = 100;

	Animator anim;	
	float timer;

	int destructableMask;

	// Use this for initialization
	void Start ()
	{

	}

	void Awake(){
		destructableMask = LayerMask.GetMask("Destructable");
		anim = GetComponent<Animator> ();
		timer = timeBetweenAttacks;
	}

	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if (Input.GetButtonDown(fireInput) && timer >= timeBetweenAttacks && Time.timeScale != 0){
			Attack ();
		}

		if (bigHandsDuration > 0){
			bigHandsDuration -= Time.deltaTime;
		}

	}

	void Attack(){
		RaycastHit[] hits;
		timer = 0f;
		
		Vector3 direction = transform.forward;
		float damageDealt = attackDamage;
		float attackRadius = transform.lossyScale.x / 2;
		
		anim.SetTrigger("Attack");
		
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
			
			int layerMask = 1 << hit.collider.gameObject.layer;
			if ((layerMask & destructableMask) > 0){
				if (bigHandsDuration > 0 || transform.lossyScale.y >= hit.transform.lossyScale.y){
					//chemicalSpawnManager.SpawnChemical(new Vector3(hit.transform.position.x, 0.5f, hit.transform.position.z));
					Destructable other = hit.collider.GetComponent<Destructable>();
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

