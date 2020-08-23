using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject[] Ships;
    public float spawnRange = 2000.0f;
    private float _d = 0f;

    void SpawnShip()
    {
        Vector3 position = new Vector3(Random.Range(-spawnRange, spawnRange), 0, Random.Range(-spawnRange, spawnRange));
        int randomInt = (int)Random.Range(0, Ships.Length);
        GameObject randomShip = Ships[randomInt];
        Instantiate(randomShip, position, Quaternion.identity);
        Debug.Log("SPAWNED: " + randomShip.name);
    }

    // Update is called once per frame
    void Update()
    {
        _d += Random.Range(Time.deltaTime, Time.deltaTime * 3);
        if((int)_d % 10 == 9)
        {
            SpawnShip();
            _d = 0;
        }    
    }
}
