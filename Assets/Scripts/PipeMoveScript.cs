using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    public float DeadZone = 190f;
    public float MoveSpeed = 5.0f; 

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * MoveSpeed) * Time.deltaTime;

        if (transform.position.x < DeadZone)
        {
            Destroy(gameObject);
        }

    }

    public void SetPipesMoveSpeed(float speed)
    {
        MoveSpeed *= speed;
    }
}
