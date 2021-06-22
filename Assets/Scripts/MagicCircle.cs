using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircle : MonoBehaviour
{

	public GameObject explosion;
	public OrcShaman caster;
	public float explosionCooldown;
	private float time_left_for_explosion;

    // Start is called before the first frame update
    void Start()
    {
		time_left_for_explosion=explosionCooldown;
    }

    // Update is called once per frame
    void Update()
    {
		time_left_for_explosion-=Time.deltaTime;
		if (time_left_for_explosion<=0.0f) {
			Instantiate(explosion, transform.position, Quaternion.identity);
			if (caster!=null) caster.casting=false;
			GameController.instance.PlayAudio(3);
			Destroy(gameObject);
		}
    }
}
