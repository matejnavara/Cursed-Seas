using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    public float speed = 5;
    public int health = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.Rotate(Vector3.up, -speed * Time.deltaTime);
    }

    public bool Shot()
    {
        health -= 1;
        return health <= 0;
    }
}
