using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] float timeToLive;
    [SerializeField] int damage = 10;

    void Start()
    {
        Destroy(gameObject, timeToLive);
    }

	void Update () {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        Shoot();
	}

    void OnTriggerEnter(Collider other)
    {
        var destructible = other.transform.GetComponent<Destructible>();
        if (destructible == null)
            return;
        destructible.TakeDamage(damage);
    }
    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.GetComponent<Cover>()) {
                Destroy(gameObject);
            }
            //print(hit.transform.name);
            EnemyAI ai = hit.collider.GetComponent<EnemyAI>();
            if (ai != null)
            {
                //print("hit");
                ai.TakeDamage(damage);
                Destroy(gameObject);
                //print(ai.currentHealth);
            }
            Player player = hit.collider.GetComponent<Player>();
            if (player != null)
            {
                print("hit");
                player.TakeDamage((int)damage);
                Destroy(gameObject);
            }
        }
    }
}