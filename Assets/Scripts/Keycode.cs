using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycode : MonoBehaviour
{
    public GameObject keydisplay;

    protected virtual void Update() {
        if(GameController.instance.player != null) {
            Vector2 mov = GameController.instance.player.transform.position - transform.position;
            Vector2 mov2 = GameController.instance.player.transform.position - transform.position;
            mov.Normalize();
            mov = mov * 0.02f * Mathf.Exp(-0.1f * Mathf.Pow(mov2.magnitude, 2));
            transform.Translate(mov);
        }
        transform.GetChild(0).transform.position = transform.position + new Vector3(0, 0.1f * (Mathf.Sin(Time.time * 1.2f) + 1), 0);

    }

    void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag=="Player") {
            GameController.instance.keyUI.SetActive(true);
            gameObject.SetActive(false);
        } 
	}
}
