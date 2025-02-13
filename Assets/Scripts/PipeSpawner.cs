using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject bottomPipePrefab;
    public GameObject topPipePrefab;
   
    public float spawnInterval = 15f;
    public float topPipeDelay = 5f;
   

    private Vector2 spawnPositionTop;
    private Vector2 spawnPositionBottom;

    public float firstSpawnDelay = 2f;

    public Transform birdTransform;

    public bool pipeSpawned = false;

    private int pipeCount = 0;
    private const int maxPipes = 3;


    void OnEnable()
    {
        
        StartCoroutine(SpawnPipes());
    }

    private void OnDisable()
    {
        StopCoroutine(SpawnPipes());
    }

    IEnumerator SpawnPipes()
    {
        
        yield return new WaitForSeconds(firstSpawnDelay);

        
            while (pipeCount < maxPipes)
            {
                yield return StartCoroutine(SpawnPipe()); // Call coroutine version of SpawnPipe
                pipeCount++;

                yield return new WaitForSeconds(spawnInterval);
            }

            FindObjectOfType<Game>().EndGame();  // Trigger EndGame
        
    }

    IEnumerator SpawnPipe()
    {
        pipeSpawned = true;
        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        float birdY = birdTransform.position.y;
        
        spawnPositionBottom = new Vector2(rightEdge, birdY - 3.5f);

        float bottomPipeY = spawnPositionBottom.y;
        
        Instantiate(bottomPipePrefab, spawnPositionBottom, Quaternion.identity);

        
        yield return new WaitForSeconds(topPipeDelay);
        pipeSpawned = false;
        spawnPositionTop = new Vector2(rightEdge, bottomPipeY + 8.5f);

        Instantiate(topPipePrefab, spawnPositionTop, Quaternion.identity);
        
        

        //yield return null;
    }
}
