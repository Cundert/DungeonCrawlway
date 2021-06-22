using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
            float xo1o = other.bounds.center.x - other.bounds.size.x / 2 + 0.1f;
            float xo2o = other.bounds.center.x + other.bounds.size.x / 2 - 0.1f;
            float xo1 = 0.95f * xo1o + 0.05f * xo2o;
            float xo2 = 0.05f * xo1o + 0.95f * xo2o;
            float yo1 = other.bounds.center.y - other.bounds.size.y / 2;
            float yo2 = other.bounds.center.y + other.bounds.size.y / 2;
            yo1 = 0.7f * yo1 + 0.3f * yo2;
            float xm1 = GetComponent<Collider2D>().bounds.center.x - GetComponent<Collider2D>().bounds.size.x / 2;
            float xm2 = GetComponent<Collider2D>().bounds.center.x + GetComponent<Collider2D>().bounds.size.x / 2;
            float ym1 = GetComponent<Collider2D>().bounds.center.y - GetComponent<Collider2D>().bounds.size.y / 2;
            float ym2 = GetComponent<Collider2D>().bounds.center.y + GetComponent<Collider2D>().bounds.size.y / 2;

            float dxp = xm2 - xo1;
            float dxn = - xm1 + xo2;
            float dyp = ym2 - yo1;
            float dyn = - ym1 + yo2;

            if (dxp < 0 || dxn < 0 || dyp < 0 || dyn < 0) return;

            if (dxp < dxn && dxp < dyp && dxp < dyn) other.gameObject.transform.Translate(new Vector3(dxp, 0, 0));
            else if (dxn < dyp && dxn < dyn) other.gameObject.transform.Translate(new Vector3(-dxn, 0, 0));
            else if (dyp < dyn) other.gameObject.transform.Translate(new Vector3(0, dyp, 0));
            else other.gameObject.transform.Translate(new Vector3(0, -dyn, 0));
        } else if(other.gameObject.tag == "Player") {
            float xo1o = other.bounds.center.x - other.bounds.size.x / 2 + 0.1f;
            float xo2o = other.bounds.center.x + other.bounds.size.x / 2 - 0.1f;
            float xo1 = 0.95f * xo1o + 0.05f * xo2o;
            float xo2 = 0.05f * xo1o + 0.95f * xo2o;
            float yo1 = other.bounds.center.y - other.bounds.size.y / 2;
            float yo2 = other.bounds.center.y + other.bounds.size.y / 2;
            yo1 = 0.7f * yo1 + 0.3f * yo2;
            float xm1 = GetComponent<Collider2D>().bounds.center.x - GetComponent<Collider2D>().bounds.size.x / 2;
            float xm2 = GetComponent<Collider2D>().bounds.center.x + GetComponent<Collider2D>().bounds.size.x / 2;
            float ym1 = GetComponent<Collider2D>().bounds.center.y - GetComponent<Collider2D>().bounds.size.y / 2;
            float ym2 = GetComponent<Collider2D>().bounds.center.y + GetComponent<Collider2D>().bounds.size.y / 2;

            float dxp = xm2 - xo1;
            float dxn = -xm1 + xo2;
            float dyp = ym2 - yo1;
            float dyn = -ym1 + yo2;

            if (dxp < 0 || dxn < 0 || dyp < 0 || dyn < 0) return;

            List<float[]> positions = new List<float[]>();
            if (other.GetComponent<Player>().last_movement.x <= 0) positions.Add(new float[2] {  dxp, 0 });
            if (other.GetComponent<Player>().last_movement.x >= 0) positions.Add(new float[2] { -dxn, 0 });
            if (other.GetComponent<Player>().last_movement.y <= 0) positions.Add(new float[2] { 0,  dyp });
            if (other.GetComponent<Player>().last_movement.y >= 0) positions.Add(new float[2] { 0, -dyn });

            float min1 = float.PositiveInfinity;
            int posmin = -1;

            for(int i = 0; i < positions.Count; ++i) {
                if(Mathf.Abs(positions[i][0]) + Mathf.Abs(positions[i][1]) < min1) {
                    min1 = Mathf.Abs(positions[i][0]) + Mathf.Abs(positions[i][1]);
                    posmin = i;
                }
            }

            other.gameObject.transform.Translate(new Vector3(positions[posmin][0], positions[posmin][1], 0));
        }
	}
}
