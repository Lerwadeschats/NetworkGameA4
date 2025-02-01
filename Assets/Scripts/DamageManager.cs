using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour
{
    public ParticleSystem DamagesParticles;
    public virtual void OnTakingDamages(float damages) { }
    

    public virtual void OnBeingAttacked(float damages) { }
    public virtual void GetHurtEffects() 
    {
        Instantiate(DamagesParticles, this.gameObject.transform.position, Quaternion.identity);
    }
    
}
