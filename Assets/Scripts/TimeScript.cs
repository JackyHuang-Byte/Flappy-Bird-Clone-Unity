using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScript : MonoBehaviour
{
    public BirdScript BirdScript;
    public PipeMoveScript PipeMoveScript;
    public PipeSpawnScript PipeSpawnScript;
    public CloudMoveScript CloudMoveScript;
    public AudioSource BGM;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (BirdScript._birdIsAlive == false)
        {
            foreach (var pipe in PipeSpawnScript.PipeSpawned)
            {
                if(pipe != null)
                {
                    PipeMoveScript localSciprt = pipe.GetComponent<PipeMoveScript>(); //access PipeMoveScript of each spawned pipe

                    localSciprt.SetPipesMoveSpeed(0);
                }
                
            }

            PipeSpawnScript.isSpawning = false;
            CloudMoveScript.Clouds.Pause();
        }
    }

    public void SpeedUp(float mutiplier) //speed *= mutiplier
    {
        /*global pipe move speed modified in BirdSciprt*/

        /*modify move speed of spawned pipes*/
        foreach (var pipe in PipeSpawnScript.PipeSpawned)
        {
            if (pipe != null)
            {
                PipeMoveScript localPipeSciprt = pipe.GetComponent<PipeMoveScript>(); //access PipeMoveScript of each spawned pipe

                localPipeSciprt.SetPipesMoveSpeed(mutiplier);
            }

        }

        /*modify pipe spawn rate*/
        PipeSpawnScript.SpawnRate *= (1/mutiplier);

        CloudMoveScript.SetCloudsMoveSpeed(mutiplier);

        BGM.pitch += 0.05f * mutiplier; 

        Debug.Log("pitch = " +  BGM.pitch);
    }
}
