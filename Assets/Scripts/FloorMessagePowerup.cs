using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMessagePowerup : MonoBehaviour
{

    private int r, s;

    // Start is called before the first frame update
    void Start()
    {
        r = Mathf.FloorToInt(Random.Range(1.0f, 4.0f));
        if (r == 1)
            GetComponent<UnityEngine.UI.Text>().text = "Your HP increases!";
        else if (r == 2)
            GetComponent<UnityEngine.UI.Text>().text = "Your strength increases!";
        else
            GetComponent<UnityEngine.UI.Text>().text = "Your speed increases!";

        s = Mathf.FloorToInt(Random.Range(1.0f, 4.0f));
        if (s == 1)
            GetComponent<UnityEngine.UI.Text>().text += " But there are more enemies now!";
        else if (s == 2)
            GetComponent<UnityEngine.UI.Text>().text += " But your enemies are tougher now!";
        else
            GetComponent<UnityEngine.UI.Text>().text += " But your enemies are stronger now!";

        GameController.instance.SetRS(r,s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
