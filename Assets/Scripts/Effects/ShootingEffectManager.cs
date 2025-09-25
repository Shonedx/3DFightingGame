using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShootingEffectManager : MonoBehaviour
{

    [SerializeField] private ParticleSystem shootingEffect1;
    [SerializeField] private ParticleSystem shootingEffect2;
    //[SerializeField] private TrailRenderer trail;
    void Awake()
    {
        
    }
    void Start()
    {

    }
    public void PlayEffects()
    {
        shootingEffect1.Play();
        shootingEffect2.Play();
    }

}
