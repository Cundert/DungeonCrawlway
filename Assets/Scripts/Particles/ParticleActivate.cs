using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleActivate : MonoBehaviour
{

    public float duration = 0.002f;

    private ParticleSystem[] ps;
    // Start is called before the first frame update
    void Start()
    {        
        Component[] comp;
        comp =  GetComponentsInChildren(typeof(ParticleSystem));

        ps = new ParticleSystem[comp.Length];
        for (int i = 0; i < comp.Length; ++i)
        {
            ps[i] = (ParticleSystem) comp[i].GetComponent<ParticleSystem>();
            ps[i].Stop();
        }
    }
 

    public void activateForDuration()
    {
        foreach(ParticleSystem p in ps)
        {
            p.Play();
        }
        Invoke("OffParticleSystems", duration);
    }

    public void OffParticleSystems()
    {
        foreach (ParticleSystem p in ps)
        {
            p.Stop();
        }
    }

    public void OnParticleSystems()
    {
        foreach (ParticleSystem p in ps)
        {
            p.Play();
        }
    }

}
