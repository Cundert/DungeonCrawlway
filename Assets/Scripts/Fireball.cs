using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{

	private GameObject target;
	private Vector2 direction;
	public int attack;

	[SerializeField]
	protected float speed;
	private float fireball_duration = 5.0f;
	private float current_fireball = 0.0f;

	// Start is called before the first frame update
	void Start() {
		target=GameController.instance.player;
		//Vector3 pos_target = new Vector3(target.transform.position.x, target.transform.position.y,0);
		//direction = Vector3.Normalize(pos_target-transform.position);
        direction = new Vector3(1, 0, 0);
		attack+=GameController.instance.e_str_index*(attack/10);
	}

    // Update is called once per frame
    void Update() {
		current_fireball+=Time.deltaTime;
		if (current_fireball>=fireball_duration) Destroy(gameObject);
		if (target!=null&&target.GetComponent<Player>().hp>0) {
			gameObject.transform.Translate(direction*speed);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag=="Player") {
			if (!other.gameObject.GetComponent<Player>().isDashing()) {
				other.gameObject.GetComponent<Player>().TakeDamage(attack, GetComponent<Collider2D>().bounds.center);
				Destroy(gameObject);
			}
		} else if (other.gameObject.tag=="Wall") Destroy(gameObject);
	}
}
