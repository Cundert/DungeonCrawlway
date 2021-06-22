using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
	private float slash_duration;
	private float current_slash = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
		slash_duration=GameController.instance.player.GetComponent<Player>().slash_duration;
    }

    // Update is called once per frame
    void Update()
    {
		current_slash+=Time.deltaTime;
        if (current_slash >= slash_duration)
        {
            Destroy(gameObject);
        }
    }

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag=="Enemy") {
			other.gameObject.GetComponent<Character>().TakeDamage(GameController.instance.player.GetComponent<Player>().attack, GetComponent<Collider2D>().bounds.center);
            Destroy(gameObject);
		}
	}

    public float GetSlashDuration()
    {
        return slash_duration;
    }
}
