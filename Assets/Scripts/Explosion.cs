using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public int attack;
	public float duration;
	private float remainingTime;
	private bool damageDealt = false;
    // Start is called before the first frame update
    void Start()
    {
		remainingTime=duration;
		attack+=GameController.instance.e_str_index*(attack/10);
	}

    // Update is called once per frame
    void Update()
    {
		remainingTime-=Time.deltaTime;
		if (remainingTime<=0.0f) Destroy(gameObject);
    }

	void OnTriggerEnter2D(Collider2D other) {
		if (!damageDealt && other.gameObject.tag=="Player") {
			other.gameObject.GetComponent<Player>().TakeDamage(attack, GetComponent<Collider2D>().bounds.center);
			damageDealt=true;
		}
	}
}
