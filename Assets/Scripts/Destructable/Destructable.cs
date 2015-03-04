﻿using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float health = 2f;
	public Transform explosion;
	public Transform itemDrop; // the item to be dropped
	public float dropProbability = 0.5f;
	public int points = 100; // the number of points this will give

	public HudHandler hud;

	float maxHealth;
	Color originalColor;

	AudioSource audioSource;
	public AudioClip explosionSound;

	// Use this for initialization
	void Start () {
		maxHealth = health;
		hud = FindObjectOfType<HudHandler> ();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void takeDamage(float damage){
		Debug.Log ("ouch");
		Debug.Log (health);
		this.health -= damage;
		if (this.health <= 0){
			destruct();
		}

		if (this.health < maxHealth){
			if (renderer){
				this.renderer.material.color = Color.Lerp(Color.red, Color.black, health/maxHealth);
			}
		}
	}

	public void destruct(){
		// TODO: increment the score according to what we destroy?
		hud.incrementPoints (points);

		if (audioSource){
			Debug.Log("AUDIO");
			if (explosionSound){
				AudioSource.PlayClipAtPoint(explosionSound, transform.position);
			}
		}

		Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
		bool shouldDrop = Random.value < dropProbability;
		if (itemDrop && shouldDrop){
			Vector3 dropPosition = gameObject.transform.position - Vector3.up * gameObject.transform.position.y;
			Instantiate(itemDrop, dropPosition, Quaternion.identity);
		}
		Debug.Log("EXPLODE");
		Destroy(gameObject);
	}
}
