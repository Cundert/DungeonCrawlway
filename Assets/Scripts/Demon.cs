using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{

    override protected void Start()
    {
        maxHp += GameController.instance.e_hp_index * (maxHp / 10);
        attack += GameController.instance.e_str_index * (attack / 10);
        base.Start();
        SetWallCollidingHitbox(new Vector2(0, 0), 0.5f*GetComponent<Collider2D>().bounds.size);
        Debug.Log(GetComponent<Collider2D>().bounds.size);
    }

    // Update is called once per frame
    override protected void Update() {
		if (alive) {
			if (target!=null&&target.GetComponent<Player>().hp>0&&TargetInSight()) {
				Vector3 pos_target = target.GetComponent<Collider2D>().bounds.center;
				Vector3 movement = Vector3.Normalize(pos_target-transform.position);
				gameObject.transform.Translate(movement*speed);
			}
            if(target != null)
			    transform.localScale=GameController.instance.player.transform.position.x-transform.position.x<0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
			base.Update();
		}
	}

}
