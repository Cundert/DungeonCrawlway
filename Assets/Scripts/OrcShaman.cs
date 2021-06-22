using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcShaman : Enemy
{
	public GameObject magic_circle;

	[SerializeField]
	private float circleCooldown;
	[SerializeField]
	private float circleRandomOffset;
	private float baseCooldown;
	private float circleWaiting = 0.0f;

	public bool casting = false;

	override protected void Start() {
		baseCooldown=circleCooldown;
		circleCooldown+=Random.Range(0.0f, circleRandomOffset);
        maxHp += GameController.instance.e_hp_index * (maxHp / 10);
        attack += GameController.instance.e_str_index * (attack / 10);
        base.Start();
        SetWallCollidingHitbox(new Vector2(0, 0), GetComponent<Collider2D>().bounds.size);
    }

	// Update is called once per frame
	override protected void Update() {
		if (alive) {
			GetComponent<Animator>().SetBool("spell", casting);
			if (target!=null&&!TargetInSight()) {
				circleWaiting=0.0f;
			} else if (target!=null&&target.GetComponent<Player>().hp>0) {
				if (!casting) circleWaiting+=Time.deltaTime;
				if (circleWaiting>=circleCooldown) {
					circleWaiting=0.0f;
					casting=true;
					circleCooldown=baseCooldown+Random.Range(0.0f, circleRandomOffset);
					GameObject c = Instantiate(magic_circle, target.transform.position, Quaternion.identity);
					c.GetComponent<MagicCircle>().caster=this;
				}
                if (target != null)
                    transform.localScale=GameController.instance.player.transform.position.x-transform.position.x<0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
			}
			base.Update();
		}
	}
}
