using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

	public float maxHealth = 100f;
	public Slider healthSlider;
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	float currentHealth;
	Animator anim;
	bool damaged;

	PlayerControl playerControl;
	PlayerAttack playerAttack;


	// Use this for initialization
	void Start ()
	{

	}

	void Awake(){
		currentHealth = maxHealth;
		anim = GetComponent<Animator>();
		playerControl = GetComponent<PlayerControl> ();
		playerAttack = GetComponent<PlayerAttack> ();
	}


	// Update is called once per frame
	void Update ()
	{
		if(damaged)
		{
			damageImage.color = flashColour;
		}
		else
		{
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
	}

	public void takeDamage(float d){
		damaged = true;

		currentHealth -= d;
		healthSlider.value = currentHealth;

		if (this.currentHealth <=0){
			playerControl.enabled = false;
			playerAttack.enabled = false;
			anim.SetTrigger("Death");
		}
	}
	
	public float getHealth(){
		return this.currentHealth;
	}

	public void incrementHealth(float increment){
		maxHealth += increment;
		currentHealth += increment;

		healthSlider.maxValue = maxHealth;
		healthSlider.value = currentHealth;
	}

	public bool isDead(){
		return this.currentHealth <= 0;
	}
}

