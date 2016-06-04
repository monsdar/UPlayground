using UnityEngine;
using System.Collections;

public class GoalManager : MonoBehaviour {
    
    public void OnCollision(float lanePos)
    {
        var lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            if(lanePos-light.transform.position.z <= 3.0 &&
                lanePos - light.transform.position.z >= -3.0)
            {
                light.enabled = true;
            }
        }

        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSys in particles)
        {
            if (lanePos - particleSys.transform.position.z <= 3.0 &&
                lanePos - particleSys.transform.position.z >= -3.0)
            {
                particleSys.loop = false; //only play once
                particleSys.Play();
            }
        }
    }
}
