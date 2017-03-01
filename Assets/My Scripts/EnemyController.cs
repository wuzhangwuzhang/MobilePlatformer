﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	bool following;
	GameObject target;
	public int currentHealth;
	public int maxHealth;
	public int experience;

	public GameObject healthBar;

	void Start () {
		following = false;
		HideHealthBar();
	}

	void Update () {
		CheckTargetPosition ();
		CheckTargetHeight ();
		CheckHealth ();
	}

	void CheckHealth()
	{
		string currentHP = currentHealth.ToString ();
		string maxHP = maxHealth.ToString ();;

		float cur_HP= float.Parse(currentHP); 
		float max_HP = float.Parse(maxHP); 

		float calc_health =  cur_HP/ max_HP;
		SetHealthBar (calc_health);
	}
	public void SetHealthBar(float myHealth){
		healthBar.transform.localScale = new Vector3(Mathf.Clamp(myHealth,0f ,1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
	}

	public void ShowHealthBar()
	{
		StartCoroutine(showHealth());
	}

	IEnumerator showHealth ()
	{ 
		GameObject healthBar = transform.Find("HealthBar").gameObject;
		healthBar.SetActive (true);
		yield return new WaitForSeconds(1);
		HideHealthBar();
	}

	public void HideHealthBar()
	{
		GameObject healthBar = transform.Find("HealthBar").gameObject;
		healthBar.SetActive (false);
	}

	void CheckTargetPosition(){
		GameObject[] objectProtected = GameObject.FindGameObjectsWithTag("Protected");
		if(objectProtected.Length>0)
			target = objectProtected [0];
	}

	void CheckTargetHeight()
	{
		if (target != null) {
			if (transform.position.y >= target.transform.position.y -5) {
				following = true;
			}
			ChaseTarget ();
		}
	}

	void ChaseTarget (){
		if (following) {
			transform.position = 
				Vector2.MoveTowards(transform.position, 
					target.transform.position, 1.5f * Time.deltaTime);
			if (transform.position.y == target.transform.position.y && transform.position.x == target.transform.position.x) {
				//Inflict damage on Protected
				GameObject[] gc = GameObject.FindGameObjectsWithTag("GameController");
				if (gc != null) {
					gc [0].GetComponent<CharacterController> ().DecreaseHealthOfProtected(1);
					Destroy (gameObject);
				}
			}
		}
	}

	public void DecreaseHealth(){
		currentHealth--;
		CallDamage (1);
		ShowHealthBar ();
		if (currentHealth <= 0) {
			GameObject[] gc = GameObject.FindGameObjectsWithTag("GameController");
			if (gc != null) {
				gc [0].GetComponent<CharacterController> ().IncreaseScore (experience);
				int index = Random.Range (0, 30);
				if (index == 15) {
					gc [0].GetComponent<CharacterController> ().ProtectedDropRareItem ();
				} else {
					gc [0].GetComponent<CharacterController> ().ProtectedDropItem ();
				}
				Destroy (gameObject);
			}
	
		}
	}

	void CallDamage(int value)
	{
		Vector3 firePosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
		GameObject damage = (GameObject)Resources.Load ("Damage");
		GameObject bPrefab = Instantiate(damage, firePosition, Quaternion.identity) as GameObject;
		bPrefab.GetComponent<DamageController> ().CreateDamage ("-"+value.ToString());
	}

	public int GetHealth()
	{
		return currentHealth;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Projectile")
		{
			DecreaseHealth ();
			Destroy (other.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Projectile")
		{
			DecreaseHealth ();
			Destroy (coll.gameObject);
		}
	}
}
