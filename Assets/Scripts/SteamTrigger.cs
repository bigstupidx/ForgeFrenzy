using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamTrigger : MonoBehaviour {
    public ParticleSystem smoke;
    public Transform smokeTriggers;
    public PlayerController player;

    public float delayingTime;
    public float emittingTime;

    public bool enter;
    public bool exit;
 

    // Use this for initialization
    void Start () {
        StartCoroutine(smokeSpawn(delayingTime));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (player.CompareTag("Smoke Triggeries"))
        {

        }
    }


    IEnumerator smokeSpawn(float delayTime)
    {
        delayingTime = delayTime;
        yield return new WaitForSeconds(delayTime);
        StartCoroutine(smokeDuration(emittingTime));
        Instantiate(smoke, smokeTriggers);
        Debug.Log(delayingTime);
        
    }
    IEnumerator smokeDuration(float emitTime)
    {
        emittingTime = emitTime;
        yield return new WaitForSeconds(emitTime);
        Debug.Log(emittingTime);
    }

    // Update is called once per frame
    void Update () {
    
    }
}


// public bool inside;
// public bool outside;

// var trigger = smoke.trigger;
// trigger.enter = enter ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
// trigger.exit = exit ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
// trigger.inside = inside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;
// trigger.outside = outside ? ParticleSystemOverlapAction.Callback : ParticleSystemOverlapAction.Ignore;

// smoke = GetComponent<ParticleSystem>();
// var smokeTrigger = smoke.trigger;
// smokeTrigger.enabled = false;
// smokeTrigger.SetCollider(0, smokeTriggers.GetComponent<Collider>());

/*
  public void OnParticleTrigger()
  {
      if (enter)
      {
          List<ParticleSystem.Particle> enterList = new List<ParticleSystem.Particle>();
          int numEnter = smoke.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);

          for (int i = 0; i < numEnter; i++)
          {
              ParticleSystem.Particle p = enterList[i];
              enterList[i] = p;
          }

          smoke.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterList);
          Debug.Log(enter);
      }
      if (exit)
      {
          List<ParticleSystem.Particle> exitList = new List<ParticleSystem.Particle>();
          int numExit = smoke.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);

          for (int i = 0; i < numExit; i++)
          {
              ParticleSystem.Particle p = exitList[i];
              p.startColor = new Color32(0, 255, 0, 255);
              exitList[i] = p;
          }

          smoke.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exitList);
          Debug.Log(exit);
      }

      if (inside)
      {
          List<ParticleSystem.Particle> insideList = new List<ParticleSystem.Particle>();
          int numInside = smoke.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);

          for (int i = 0; i < numInside; i++)
          {
              ParticleSystem.Particle p = insideList[i];
              p.startColor = new Color32(0, 0, 255, 255);
              insideList[i] = p;
          }

          smoke.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideList);
          Debug.Log(inside);
      }

      if (outside)
      {
          List<ParticleSystem.Particle> outsideList = new List<ParticleSystem.Particle>();
          int numOutside = smoke.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);

          for (int i = 0; i < numOutside; i++)
          {
              ParticleSystem.Particle p = outsideList[i];
              p.startColor = new Color32(0, 255, 255, 255);
              outsideList[i] = p;
          }

          smoke.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, outsideList);
          Debug.Log(outside);
      }
  }
  */
