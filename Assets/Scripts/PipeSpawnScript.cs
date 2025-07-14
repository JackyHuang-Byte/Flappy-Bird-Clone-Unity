using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject Pipe;
    public List<GameObject> PipeSpawned = new List<GameObject>();
    public float SpawnRate = 1.0f;
    public float heightOffset = 10.0f;
    public bool isSpawning = true;

    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPipe(isSpawning);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < SpawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnPipe(isSpawning);
            timer = 0;
        }
        
    }

    private void SpawnPipe(bool myBool)
    {
        if (myBool)
        {
            float lowestPoint = transform.position.y - heightOffset;
            float highestPoint = transform.position.y + heightOffset;
            GameObject pipeInstantiated = Instantiate(Pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
            PipeSpawned.Add(pipeInstantiated);
            
        }

    }

    [ContextMenu("Spawn Parameter")]
    private void SpawnParameter()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        Instantiate(Pipe);
        Instantiate(Pipe, new Vector3(transform.position.x + 10, lowestPoint, 0), transform.rotation);
        Instantiate(Pipe, new Vector3(transform.position.x + 40, highestPoint, 0), transform.rotation);

    }
}
