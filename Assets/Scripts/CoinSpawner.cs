using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Vector2 spawnPosition;
    public float coinSpacingY = 1f;
    public float coinSpacingX;
    private float pipeXPosition;
    public Transform birdTransform;

    private float breathHoldDistance = 5f;
    public float coinHoldSpacingX;

    public float spawnInterval = 15f;

    private List<Vector2> coinPositions = new List<Vector2>();
    private PipeSpawner pipeSpawner;


    // Start is called before the first frame update
    void Start()
    {
        pipeSpawner = FindObjectOfType<PipeSpawner>();

        float rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x-0.1f;
        pipeXPosition = rightEdge;

        float inhaledTime = LoadMaxTimeInhale();
        Debug.Log(inhaledTime);
        
        coinSpacingX = inhaledTime / 3f;
        if (coinSpacingX < 0.6f)
        {
            coinSpacingX = 0.67f;
        }
        Debug.Log(coinSpacingX);
        coinHoldSpacingX = breathHoldDistance / 6f;

        //float bottomCoinX = pipeXPosition - (coinSpacingX * 2);

        //spawnPosition = new Vector2(bottomCoinX, 0f);

        StartCoroutine(SpawnCoins());
    }

    IEnumerator SpawnCoins()
    {

        yield return new WaitUntil(() => pipeSpawner.pipeSpawned);
        
        while (true)
        {
            
            float bottomCoinX = pipeXPosition - (coinSpacingX * 2);

            float birdY = birdTransform.position.y;

            spawnPosition = new Vector2(bottomCoinX, birdY);

            for (int i = 0; i < 2; i++)
            {
                SpawnCoin(spawnPosition);
                spawnPosition.y += coinSpacingY;
                yield return new WaitForSeconds(coinSpacingX);
            }


            SpawnCoin(spawnPosition);
            spawnPosition.y += coinSpacingY;

            //spawnPosition.x = pipeXPosition;
            spawnPosition.y -= coinSpacingY;

            yield return new WaitForSeconds(coinHoldSpacingX);

            for (int i = 0; i < 1; i++)
            {
                spawnPosition.x += coinHoldSpacingX;
                SpawnCoin(spawnPosition);
                yield return new WaitForSeconds(coinHoldSpacingX);
            }

            spawnPosition.x += coinHoldSpacingX;
            SpawnCoin(spawnPosition);
            yield return new WaitForSeconds(coinSpacingX);

            for (int i = 0; i < 2; i++)
            {
                spawnPosition.y -= coinSpacingY;
                SpawnCoin(spawnPosition);
                yield return new WaitForSeconds(coinSpacingX);
            }

            yield return new WaitUntil(() => pipeSpawner.pipeSpawned);

        }

    }

    void SpawnCoin(Vector2 position)
    {
        GameObject newCoin = Instantiate(coinPrefab, position, Quaternion.identity);
        
        CoinPathTracker.Instance.TrackCoin(newCoin);
        
    }

    float LoadMaxTimeInhale()
    {
        string currenUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        DataManager.UserDataCollection collection = DataManager.LoadUserData();

        DataManager.UserData user = collection.users.Find(u => u.username == currenUser);
        if (user != null && user.timeInhale > 0)
        {
            return user.timeInhale;
        }

        return 3f;
    }

}
