using System.Collections.Generic;
using UnityEngine;

public class CoinPathTracker : MonoBehaviour
{
    private List<Vector2> coinPath = new List<Vector2>();
    public static CoinPathTracker Instance;

    private Transform bird;
    private List<GameObject> activeCoins = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
   
        }
    }

    void Start()
    {
        bird = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (bird == null || activeCoins.Count == 0) return;

        PathDrawer pathDrawer = FindObjectOfType<PathDrawer>();
        if (pathDrawer == null) return;

        List<Vector2> birdPath = pathDrawer.GetPath();
        if (birdPath.Count == 0) return;

        float birdFakeX = pathDrawer.GetCurrentFakeX();

        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            GameObject coin = activeCoins[i];

            if (coin == null) continue;

            float coinX = coin.transform.position.x;
            float birdX = bird.position.x;

            if(!pathDrawer.HasStartedTracking() && coinX <= birdX)
            {
                pathDrawer.StartTrackingPath();
                Debug.Log("Path tracking started as the first coin passed the bird.");
            }

            // Store position when the coin reaches bird's x position
            if (coinX <= birdX)
            {
                if (!coinPath.Exists(pos => Mathf.Approximately(pos.x, birdX) && Mathf.Approximately(pos.y, coin.transform.position.y)))
                {
                    Vector2 finalCoinPosition = new Vector2(birdFakeX, coin.transform.position.y);
                    coinPath.Add(finalCoinPosition);
                    Debug.Log($"Stored Coin at: {finalCoinPosition}");
                }

                // Remove from active list (prevents duplicates)
                activeCoins.RemoveAt(i);
          
            }
        }
    }

    public void TrackCoin(GameObject coin)
    {
        if (!activeCoins.Contains(coin))
        {
            activeCoins.Add(coin);
        }
    }

    public void StoreCoinFinalPosition(Vector2 position)
    {
        if (!coinPath.Contains(position))
        {
            coinPath.Add(position);
            
        }
    }

    public List<Vector2> GetCoinPath()
    {
        return coinPath;
    }
    
}
