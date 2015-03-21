using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{

	public float maxHealth = 100f;
	float currentHealth;
	Animator anim;

	// Use this for initialization
	void Start ()
	{

	}

	void Awake(){
		currentHealth = maxHealth;
		anim = GetComponent<Animator>();
	}


	// Update is called once per frame
	void Update ()
	{

	}

	public void takeDamage(float d){
		this.currentHealth -= d;
		if (this.currentHealth <=0){
			anim.SetTrigger("Death");
		}
	}
	
	public float getHealth(){
		return this.currentHealth;
	}
}

