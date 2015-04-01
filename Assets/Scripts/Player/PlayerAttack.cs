using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
	public float timeBetweenAttacks = 0.15f;
	public float attackRange = 0.5f;
	public float attackDamage = 1f;
	public string regularAttackInput;
	public string hardAttackInput;
	public float hardAttackMultiplier = 4f;

	// sounds
	AudioSource audioSource;
	public AudioClip[] slaps;
	public AudioClip land;
	public AudioClip swooshSmall;
	public AudioClip swooshBig;

	Animator anim;	
	float timer;
	float hardAttackDelay;

	int destructableMask;

	PlayerScoreCounter scoreCounter;
	PlayerControl playerControl;

	CameraControl camControl;

	private int slapIndex = 0;

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
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if (Input.GetButtonDown(regularAttackInput) && timer >= timeBetweenAttacks && Time.timeScale != 0){
			Debug.Log("Attack!");
			anim.SetTrigger("Attack");
			Attack (attackDamage);

		} else if (Input.GetButtonDown(hardAttackInput) && timer >= timeBetweenAttacks * 2f && Time.timeScale != 0) {
			Debug.Log(timeBetweenAttacks * 4f);
			anim.SetTrigger("HardAttack");
			playerControl.hardAttack(timeBetweenAttacks * 2f);
			timer = 0f;
			Debug.Log ("Timer:" + timer);
			hardAttackDelay = 0.7f;
		}

		if (hardAttackDelay > 0){
			hardAttackDelay -= Time.deltaTime;
			if (hardAttackDelay <= 0){
				Debug.Log("Attack delay over");

				Debug.Log (playerControl.transform.lossyScale.y);
				// Play hard attack land here
				AudioSource.PlayClipAtPoint(land, transform.position, playerControl.transform.lossyScale.y/20.0f);

				Attack(attackDamage * hardAttackMultiplier);
			}
		}



	}

	void Attack(float damageDealt){
		RaycastHit[] hits;
		timer = 0f;
		
		Vector3 direction = transform.forward;

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
				if (transform.lossyScale.y >= hit.transform.lossyScale.y * 0.25f){
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
					if (audioSource) {
						if (slapIndex >= slaps.Length) {
							slapIndex = 0;
						}
						
						AudioSource.PlayClipAtPoint(slaps[slapIndex], transform.position);
						slapIndex++;
					}	
				}
			} else if (hit.collider.tag == "Player" && hit.collider.gameObject.name != this.gameObject.name){
				if (audioSource) {
					if (slapIndex >= slaps.Length) {
						slapIndex = 0;
					}
					
					AudioSource.PlayClipAtPoint(slaps[slapIndex], transform.position);
					slapIndex++;
				}	
				Debug.Log("HIT!!");
				PlayerHealth otherPlayer = hit.collider.GetComponent<PlayerHealth>();
				PlayerControl controlPlayer = hit.collider.GetComponent<PlayerControl>();
				
				// send a knockback
				controlPlayer.knockBack(direction, damageDealt);
				otherPlayer.takeDamage(damageDealt);
				if (otherPlayer.getHealth() <= 0){
					anim.SetTrigger("Win");
					playerControl.faceCamera();
					playerControl.enabled = false;
					this.enabled = false;
				}
			} else {
				// we missed everything
				if (playerControl.transform.lossyScale.y < 20.0f) {
					AudioSource.PlayClipAtPoint(swooshSmall, transform.position);
				} else {
					AudioSource.PlayClipAtPoint(swooshBig, transform.position);
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
}

