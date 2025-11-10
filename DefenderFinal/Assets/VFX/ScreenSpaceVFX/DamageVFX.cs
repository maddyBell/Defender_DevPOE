using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVFX : MonoBehaviour
{
    public Material vfx;
    private float strength = 0f;
    public float decaySpeed = 1f;

    void Awake()
    {
        vfx.SetFloat("_TowerDamage", strength);
    }

    void Update()
    {
        if(strength > 0f)
        {
            strength -= decaySpeed * Time.deltaTime;
            vfx.SetFloat("_TowerDamage", strength);
        }

    }

    public void Hit(float amount)
    {
        strength = 1f;
        vfx.SetFloat("_TowerDamage", strength);
        
    }
}
