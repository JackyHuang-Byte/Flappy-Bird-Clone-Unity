using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMoveScript : MonoBehaviour
{
    public ParticleSystem Clouds;
    public float OffsetMultiplier = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*Set the distance of particles from the camera according to their speeds*/
        var clouds = new ParticleSystem.Particle[Clouds.particleCount];
        int currentClouds = Clouds.GetParticles(clouds);
        float speedOffset;

        for (int i = 0; i < currentClouds; i++)
        {
            speedOffset = Clouds.main.startSpeed.constantMax - clouds[i].velocity.magnitude;

            clouds[i].position = new Vector3(clouds[i].position.x, speedOffset * OffsetMultiplier, clouds[i].position.z);
            /*the reason of offsetting y variable is because the shape of particle system, suppouse to offset z variable in common stances*/

            Clouds.SetParticles(clouds, currentClouds);
        }

        /*Size by Speed: a built-in function in particle system*/
    }

    public void SetCloudsMoveSpeed(float mutiplier)
    {
        var main = Clouds.main;
        main.simulationSpeed *= mutiplier;

        Debug.Log("Clouds: " + main.simulationSpeed);

    }
}
