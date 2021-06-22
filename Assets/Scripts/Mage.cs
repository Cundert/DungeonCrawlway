using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Enemy
{
	public GameObject fireball;

	[SerializeField]
	private float fireballCooldown;
	[SerializeField]
	private float fireballRandomOffset;
	private float baseCooldown;
	private float fireballWaiting = 0.0f;


	override protected void Start() {
		baseCooldown=fireballCooldown;  
		baseCooldown+=Random.Range(0.0f, fireballRandomOffset);
        maxHp += GameController.instance.e_hp_index * (maxHp / 10);
        attack += GameController.instance.e_str_index * (attack / 10);
        base.Start();
        SetWallCollidingHitbox(new Vector2(0, 0), GetComponent<Collider2D>().bounds.size);
    }

	// Update is called once per frame
	override protected void Update() {
		if (alive) {
			fireballWaiting+=Time.deltaTime;
			if (fireballWaiting>=(3.0f*fireballCooldown/4.0f)) {
				GetComponent<Animator>().SetBool("attacking", true);
			}
			if (target!=null&&target.GetComponent<Player>().hp>0&&fireballWaiting>=fireballCooldown&&TargetInSight()) {
				fireballWaiting=0.0f;
				fireballCooldown=baseCooldown+Random.Range(0.0f, fireballRandomOffset);
				Vector3 dir_to_player = GameController.instance.player.GetComponent<BoxCollider2D>().bounds.center-transform.position;
				Vector2 dir_to_player2d = new Vector2(dir_to_player.x, dir_to_player.y);
				GetComponent<Animator>().SetBool("attacking", false);
				GameController.instance.PlayAudio(2);
				Instantiate(fireball, GetComponent<BoxCollider2D>().bounds.center, Quaternion.Euler(Vector3.forward*Vector2.SignedAngle(new Vector2(1, 0), dir_to_player2d)));
			}
            if (target != null)
                transform.localScale=GameController.instance.player.transform.position.x-transform.position.x<0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
			base.Update();
		}
	}
}
