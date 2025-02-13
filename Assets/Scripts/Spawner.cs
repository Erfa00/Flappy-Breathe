using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{

    public GameObject prefab;
    public float firstSpawnDelay = 5f;
    public float rate = 8f;
    public float spawnXOffset = 2f; // Distance outside the right edge

    private Vector2 spawnPosition;
    

    private void OnEnable()
    {

        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        spawnPosition = new Vector2(rightEdge + spawnXOffset, 0); // Spawn outside screen
        StartCoroutine(Spawn());
        
    }

        private void OnDisable()
    {
        StopCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {

        yield return new WaitForSeconds(firstSpawnDelay); // Delay before first spawn

        while (true)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
            //currentIndex = (currentIndex == 0) ? 1 : 0;

            yield return new WaitForSeconds(rate); // Wait 5 seconds before spawning the next object
        }

    }


}
