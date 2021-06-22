using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public static CanvasScript instance = null;

    private void Start() {
        
    }

    void Awake() {
        //Debug.Log("Awaken my masters");

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            instance.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
