using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex;

    public float blinkDuration = 1f;
    public float blinkInterval = 0.1f;

    public int maxLives = 3;
    private int currentLives;

    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    private Rigidbody2D rb;

    //Serial Com
    private SerialReader serialReader;
    public string receivedData;

    public float volumeChange;
    public float airFlow;

    //for keyboard
    private Vector2 playerDirection;
    
    //Players Level
    public float forceMultiplier;
    private float speedMultiplier;

    //Moving the bird
    public float distance;

    private Game gameManagerScript;

    //Game Timer
    private float elapsedTime = 0f;
    private bool isTrackingTime = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void onEnable()
    {
        rb.position = new Vector2(0, 0); // Set initial position in 2D space -- Should be in Play() enable
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
        
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 10;

        gameManagerScript = GameObject.Find("GameManager").GetComponent<Game>();
        serialReader = FindObjectOfType<SerialReader>();

        rb = GetComponent<Rigidbody2D>();

        currentLives = maxLives;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHeartsUI();

        //Calculate forceMultipler
        float maxInhalationVolume = LoadMaxInhalationVolume();
        float maxTimeInhale = LoadMaxTimeInhale();
        forceMultiplier = 2f / (maxInhalationVolume+500f);
        //forceMultiplier = (maxInhalationVolume + 200f) / maxTimeInhale;
        //speedMultiplier = 30* 2f / maxTimeInhale;


        Debug.Log($"Max Inhalation Volume: {maxInhalationVolume} mL");
        Debug.Log($"Max Inhalation Time: {maxTimeInhale} s");
        Debug.Log($"Calculated Bird Upward Force: {forceMultiplier}");

    }

    float LoadMaxInhalationVolume()
    {
        string currenUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        DataManager.UserDataCollection collection = DataManager.LoadUserData();

        DataManager.UserData user = collection.users.Find(u => u.username == currenUser);
        if (user != null && user.maxInhalationVolume > 0) 
        {
            return user.maxInhalationVolume;
        }

        return 3000f;
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



    void Update()
    {
        try
        {
            if (serialReader.port.IsOpen)
            {
                receivedData = serialReader.port.ReadLine();
                Debug.Log(receivedData);
            }
        }
        catch (System.Exception ex)
        {
            ex = new System.Exception();
        }

        ProcessReceivedData(receivedData);

        if (isTrackingTime)
        {
            elapsedTime += Time.deltaTime;  // Increment time while tracking
        }

    }

    void FixedUpdate()
    {
        distance = volumeChange * forceMultiplier * (airFlow);
        rb.MovePosition(rb.position + (Vector2.up * distance * Time.fixedDeltaTime));
    }

    void ProcessReceivedData(string data)
    {
        string[] values = data.Split(',');

        float.TryParse(values[0], out volumeChange);
        float.TryParse(values[1], out airFlow);
          
    }


    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
            LoseLife();
           
        } 

        else if(other.gameObject.tag == "Boundary")
        {
            FindObjectOfType<Game>().GameOver();
        }
        
        else if (other.gameObject.tag == "Score")
        {
            FindObjectOfType<Game>().IncreaseScore();
            PathDrawer pathDrawer = FindObjectOfType<PathDrawer>();

            if (!pathDrawer.HasStartedTracking())
            {
                pathDrawer.StartTrackingPath();
                elapsedTime = 0f;
                isTrackingTime = true;
                
            }
            
            if (pathDrawer == null) return;

            float birdFakeX = pathDrawer.GetCurrentFakeX();

            // Store the final position of the coin using bird's fake X
            Vector2 finalCoinPosition = new Vector2(birdFakeX, other.transform.position.y);

            // Store the updated position in CoinPathTracker
            CoinPathTracker.Instance.StoreCoinFinalPosition(finalCoinPosition);

            Destroy(other.gameObject);

        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void LoseLife()
    {
        if(currentLives > 0)
        {
            currentLives--;
            UpdateHeartsUI();
            StartCoroutine(BlinkEffect());
        }

        if(currentLives <= 0)
        {
            FindObjectOfType<Game>().GameOver();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
            {
                heartImages[i].sprite = fullHeart; // Show full heart
            }
            else
            {
                heartImages[i].sprite = emptyHeart; // Show empty heart
            }
        }
    }

    IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        spriteRenderer.enabled = true;
    }

    
}
