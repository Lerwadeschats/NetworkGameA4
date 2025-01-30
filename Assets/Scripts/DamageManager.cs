using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageManager : MonoBehaviour
{
    
    public virtual void OnTakingDamanges(float damages)
    {
        
    }
    

    public virtual void OnBeingAttacked(float damages) { }
    
}
