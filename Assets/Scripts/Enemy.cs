using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	protected GameObject target;

	// Start is called before the first frame update
	override protected void Start()
    {
		target=GameController.instance.player;
		base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
		if (alive) {
			if (target!=null&&target.GetComponent<Player>().hp>0&&TargetInSight()) {
				Vector3 pos_target = target.transform.position;
				Vector3 movement = Vector3.Normalize(pos_target-transform.position);
				gameObject.transform.Translate(movement*speed);
			}
			base.Update();
		}
    }

	protected void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag=="Player") {
			other.gameObject.GetComponent<Player>().TakeDamage(attack, GetComponent<Collider2D>().bounds.center);
		}
	}

	protected bool TargetInSight() {
		LayerMask mask = LayerMask.GetMask("Player", "Wall");
		Vector3 point = target.GetComponent<Collider2D>().bounds.center;
		RaycastHit2D hit = Physics2D.Raycast(GetComponent<Collider2D>().bounds.center, point - GetComponent<Collider2D>().bounds.center, 100f, mask);
		if (hit.collider!=null && hit.collider.gameObject.tag=="Player") return true;
		return false;
	}

	public void Die() {
		Destroy(gameObject);
	}
}
