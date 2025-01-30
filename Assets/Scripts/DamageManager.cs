using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour
{
    public ParticleSystem DamagesParticles;
    public virtual void OnTakingDamanges(float damages)
    {
        Instantiate(DamagesParticles, this.gameObject.transform.position, Quaternion.identity);
    }
    

    public virtual void OnBeingAttacked(float damages) { }
    
}
