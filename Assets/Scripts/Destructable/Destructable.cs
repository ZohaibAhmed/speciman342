using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float health = 2f;
	public Transform explosion;
	public Transform itemDrop; // the item to be dropped

	float maxHealth;
	Color originalColor;

	// Use this for initialization
	void Start () {
		maxHealth = health;
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
		Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
		if (itemDrop){
			Instantiate(itemDrop, gameObject.transform.position, Quaternion.identity);
		}
		Debug.Log("EXPLODE");
		Destroy(gameObject);
	}
}
