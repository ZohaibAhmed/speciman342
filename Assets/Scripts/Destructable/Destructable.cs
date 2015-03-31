using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float health = 2f;
	public Transform explosion;
	public Transform itemDrop; // the item to be dropped
	public float dropProbability = 0.5f;
	public int points = 100; // the number of points this will give
	public bool building = false;
	public bool ShakeWhenHit;
	public bool MoveDownWhenHit;
	public Mesh damagedMesh;
	public Texture damageTexture;
	public GameObject smoke;
	public Vector3 smokeOffset;
	public Vector3 explosionOffset;
	public Vector3 explosionRotation;

	public float dropHeightOffset = 0f;

	public HudHandler hud;

	float maxHealth;
	Color originalColor;
		
	AudioSource audioSource;
	public AudioClip explosionSound;

	Animator anim;
	int destroyHash = Animator.StringToHash("destroy");

	Vector3 originalPosition;
	Vector3 currentPosition;
	float shakeIntensity;
	float shakeDecay;

	float smokeTimer;

	ParticleSystem hitParticles;

	// Use this for initialization
	void Start () {
		maxHealth = health;
		hud = FindObjectOfType<HudHandler> ();
		audioSource = GetComponent<AudioSource>();
		anim = GetComponent<Animator> ();
		originalPosition = transform.position;
		currentPosition = originalPosition;
		hitParticles = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update() {
		if (shakeIntensity > 0){
			this.transform.position = currentPosition + Random.insideUnitSphere * shakeIntensity;
			shakeIntensity -= shakeDecay;
			if (shakeIntensity <= 0){
				this.transform.position = currentPosition;
			}
		} 

		if (smokeTimer > 0){
			smokeTimer -= Time.deltaTime;
		}
	}

	public void takeDamage(float damage){
		Debug.Log ("ouch");
		Debug.Log (health);
		this.health -= damage;
		if (this.health <= 0){
			destruct();
		}

		if (this.health < maxHealth){
			Renderer r = GetComponent<Renderer>();
			if (this.health + damage == maxHealth){ // first hit

				MeshFilter meshFilter = GetComponent<MeshFilter>();
				if (meshFilter && damagedMesh){
					meshFilter.mesh = damagedMesh;
					MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
					Debug.Log(damagedMesh.subMeshCount);
					if (r.materials.Length < damagedMesh.subMeshCount){
						Material[] materials = new Material[damagedMesh.subMeshCount];
						for (int j = 0; j < materials.Length; j++){
							if (j < r.materials.Length){
								materials[j] = r.materials[j];
								materials[j].mainTexture = damageTexture;
							} else {
								materials[j] = r.materials[0];
								materials[j].mainTexture = damageTexture;
							}
						}
						meshRenderer.materials = materials;
					}

					MeshCollider col = GetComponent<MeshCollider>();
					if (col){
						col.sharedMesh = damagedMesh;
					}
				}
			}


			if (r){
				foreach (Material m in r.materials){
					m.color = Color.Lerp(Color.yellow, Color.red, 1 - health/maxHealth);
					m.mainTexture = damageTexture;
					m.mainTextureScale = new Vector2(maxHealth / (health + damage), maxHealth / (health + damage)) * 2f;
				}
			} 
			Renderer[] childrenRenderer = this.GetComponentsInChildren<Renderer>();
			int i = 0;
			while (i < childrenRenderer.Length){
				if (childrenRenderer[i].gameObject.tag != "DamageParticle"){
					foreach (Material m in childrenRenderer[i].materials){
						m.color = Color.Lerp(Color.yellow, Color.red, 1 - health/maxHealth);
						m.mainTexture = damageTexture;
						m.mainTextureScale = new Vector2(maxHealth / (health + damage), maxHealth / (health + damage)) * 2f;
					}
				}
				i++;
			}

			if (MoveDownWhenHit){
				currentPosition = new Vector3(originalPosition.x,
				                              Mathf.Lerp(originalPosition.y, originalPosition.y - transform.lossyScale.y / 2, 1 - health/maxHealth),
				                              originalPosition.z);
				
				transform.position = currentPosition;
			}
			
			
			if (smoke && smokeTimer <= 0){
				Quaternion smokeRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
				Instantiate (smoke, transform.position + smokeOffset, smokeRotation);
				smokeTimer = 10f;
			}

			if (hitParticles){
				hitParticles.Play();
			}

			if (ShakeWhenHit){
				Shake(damage);
			}
		}
	}

	public void destruct(){
		// TODO: increment the score according to what we destroy?
		//hud.incrementPoints (points);

		if (hitParticles){
			hitParticles.Play();
		}
		
		if (audioSource){
			Debug.Log("AUDIO");
			if (explosionSound){
				AudioSource.PlayClipAtPoint(explosionSound, transform.position);
			}
		}
		if (explosion && !building) {
			Debug.Log("Explode me");
			Quaternion explosionQ = Quaternion.Euler(explosionRotation.x, explosionRotation.y, explosionRotation.z);
			Instantiate (explosion, originalPosition + explosionOffset, explosionQ);
		} else if (building) {
			anim.SetTrigger (destroyHash);
			Instantiate (smoke, gameObject.transform.position + smokeOffset, Quaternion.identity);
		}

		bool shouldDrop = Random.value < dropProbability;
		if (itemDrop && shouldDrop){
			Vector3 dropPosition = gameObject.transform.position - Vector3.up * gameObject.transform.position.y + Vector3.up * dropHeightOffset;
			Instantiate(itemDrop, dropPosition, Quaternion.identity);
		}
		Debug.Log("EXPLODE");
		Destroy(gameObject);
	}

	void Shake(float damage){
		shakeIntensity = Mathf.Clamp(0.2f * damage, 0f, 0.25f * transform.lossyScale.x);
		shakeDecay = 0.1f * shakeIntensity;
	}
}
