using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	[SerializeField]
	private LayerMask mask;

	[SerializeField]
	private int damage;

	private Vector3 prevPos;

    private void Start()
    {
		prevPos = transform.position;
    }
    private void Update()
	{
		prevPos = transform.position;

		if (Input.GetMouseButtonDown(0))
		{
			Shoot();
		}
	}

	/*private void Shoot()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, mask))
		{
			EnemyAI ai = hit.collider.GetComponent<EnemyAI>();
			if(ai != null)
			{
				print("hit");
				ai.TakeDamage(damage);
				print(ai.currentHealth);
			}
		}
	}*/
/*private void Shoot()
{
	RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);
	for (int i = 0; i < hits.Length; i++) {

		if (hits[i].collider.CompareTag("Enemy")) {
			print("comparing");
			EnemyAI ai = hits[i].collider.GetComponent<EnemyAI>();
			if (ai != null)
			{
				print("hit");
				ai.TakeDamage(damage);
				Destroy(gameObject);
				print(ai.currentHealth);
				i = hits.Length;
			}
		}
	}
}*/

private void Shoot()
{
	RaycastHit hit;
	if (Physics.Raycast(transform.position, transform.forward, out hit))
	{
			print(hit.transform.name);
		EnemyAI ai = hit.collider.GetComponent<EnemyAI>();
		if(ai != null)
		{
			print("hit");
			ai.TakeDamage(damage);
			print(ai.currentHealth);
		}
	}
}
}
