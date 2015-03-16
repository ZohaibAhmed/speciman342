using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{

	public float maxHealth = 100f;
	float currentHealth;

	// Use this for initialization
	void Start ()
	{

	}

	void Awake(){
		currentHealth = maxHealth;
	}


	// Update is called once per frame
	void Update ()
	{

	}

	public void takeDamage(float d){
		this.currentHealth -= d;
	}
	
	public float getHealth(){
		return this.currentHealth;
	}
}

