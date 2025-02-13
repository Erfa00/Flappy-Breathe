using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GraphManager : MonoBehaviour
{
    public RectTransform graphPanel;
    public TMP_Text xAxisTitle;
    public TMP_Text yAxisTitle;

    //public Transform xAxisLabelsContainer;
    public Transform yAxisLabelsContainer;
    public GameObject axisLabelPrefab;

    public int numXTicks = 5;
    public int numYTicks = 5;
    public float maxScore = 100;
    public float maxDistance = 4;

    public Transform xAxisPopupLabelsContainer;
    public Transform yAxisPopupLabelsContainer;
    public int numYPopupTicks = 10;
    public int numXPopupTicks = 15;

    public LineRenderer xAxisRenderer;
    public LineRenderer yAxisRenderer;

    public GameObject pointPrefab;
    public LineRenderer lineRenderer;
    private List<Vector2> points = new List<Vector2>();

    public GameObject tooltipText;
    public TMP_Text textT;

    public GameObject popupPanel;
    public RectTransform popupGraph;
    public GameObject pathPanel;
    public TMP_Text popupText;
    public LineRenderer popupLineRenderer;
    public LineRenderer popupCoinLineRenderer;

    public GameObject redPointPrefab;
    private List<GameObject> coinMarkers = new List<GameObject>();

    public LineRenderer popupXAxisRenderer;
    public LineRenderer popupYAxisRenderer;
    



    // Start is called before the first frame update
    void Start()
    {
        SetupAxisTitles();
        //GenerateAxisLabels();
        DrawAxes();
        DrawGraph();
        
    }

    void TestDrawLine()
    {
        if (popupXAxisRenderer == null) return;

        popupXAxisRenderer.positionCount = 2;
        popupXAxisRenderer.SetPosition(0, new Vector3(-3, -2, 0));  // Example start position
        popupXAxisRenderer.SetPosition(1, new Vector3(3, -2, 0));   // Example end position
        popupXAxisRenderer.startColor = Color.white;
        popupXAxisRenderer.endColor = Color.white;
        popupXAxisRenderer.enabled = true;

        Debug.Log("TestDrawLine executed.");
    }

    void Update()
    {
        DetectHover();
    }

    private void DetectHover()
    {

        if (points.Count == 0) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(graphPanel, Input.mousePosition, Camera.main, out localMousePos);


        float minDistance = 20f;
        bool found = false;

        for( int i = 0; i< points.Count; i++)
        {
            float distance = Vector2.Distance(localMousePos, points[i]);

            if (distance < minDistance)
            {
                ShowToolTip(points[i], i);
                found = true;

                if (Input.GetMouseButtonDown(0))
                {
                    ShowPopup(i);
                    
                    GeneratePopupAxisLabels(i);
                }
                break;
            }
        }

        if (!found)
        {
            tooltipText.SetActive(false);
        }
    }

    private void ShowPopup(int index)
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(currentUser);

        if (index >= gameSessions.Count) return;

        popupPanel.SetActive(true);
        pathPanel.SetActive(true);
        lineRenderer.gameObject.SetActive(false);
        yAxisRenderer.gameObject.SetActive(false);
        xAxisRenderer.gameObject.SetActive(false);

        string date = gameSessions[index].dateTime;
        string score = gameSessions[index].score.ToString();

        popupText.text = $" Session: {index+1}\n Date: {date}\n Score: {score}";
        popupText.alignment = TextAlignmentOptions.Center;


        Debug.Log($"Drawing stored paths for index {index}: Bird Path Count = {gameSessions[index].drawnPath.Count}, Coin Path Count = {gameSessions[index].coinPath.Count}");

        
        StartCoroutine(DrawAfterFrame(gameSessions[index].drawnPath, gameSessions[index].coinPath));

    }

    IEnumerator DrawAfterFrame(List<Vector2> storedPath, List<Vector2> storedCoinPath)
    {
        yield return new WaitForEndOfFrame();
        DrawStoredPath(storedPath, storedCoinPath, popupLineRenderer, popupCoinLineRenderer);
        TestDrawLine();
        DrawPopupAxes();

    }

    void DrawStoredPath(List<Vector2> birdPath, List<Vector2> coinPath, LineRenderer birdRenderer, LineRenderer coinRenderer)
    {
        if (birdRenderer == null || birdPath == null || birdPath.Count == 0) return;
        if (coinRenderer == null || coinPath == null || coinPath.Count == 0) return;

        birdRenderer.positionCount = birdPath.Count;
        coinRenderer.positionCount = coinPath.Count;

        birdRenderer.sortingOrder = 10;
        coinRenderer.sortingOrder = 10;

        birdRenderer.widthMultiplier = 0.1f;
        coinRenderer.widthMultiplier = 0.05f;

        birdRenderer.startColor = Color.black;
        birdRenderer.endColor = Color.black;
        coinRenderer.startColor = Color.yellow;
        coinRenderer.endColor = Color.yellow;

        RectTransform popupRect = popupPanel.GetComponent<RectTransform>();

        // Find the min X and min Y to normalize the path
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector2 point in birdPath)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        foreach (Vector2 point in coinPath)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        // Calculate scaling factors to fit path within the popup
        float scaleX = popupRect.rect.width / (maxX - minX + 1);
        float scaleY = popupRect.rect.height / (maxY - minY + 1);

        // 🔹 Draw Bird Path
        for (int i = 0; i < birdPath.Count; i++)
        {
            float normalizedX = (birdPath[i].x - minX) * scaleX - popupRect.rect.width / 2;
            float normalizedY = (birdPath[i].y - minY) * scaleY - popupRect.rect.height / 2;

            birdRenderer.SetPosition(i, new Vector2(normalizedX, normalizedY));
        }

        // 🔹 Draw Coin Path
        for (int i = 0; i < coinPath.Count; i++)
        {
            float normalizedX = (coinPath[i].x - minX) * scaleX - popupRect.rect.width / 2;
            float normalizedY = (coinPath[i].y - minY) * scaleY - popupRect.rect.height / 2;

            coinRenderer.SetPosition(i, new Vector2(normalizedX, normalizedY));

            // **Instantiate Red Points**
            GameObject redPoint = Instantiate(redPointPrefab, popupPanel.transform);
            redPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(normalizedX, normalizedY);
            coinMarkers.Add(redPoint);
        }

        birdRenderer.enabled = true;
        coinRenderer.enabled = true;
    }

    Vector2 ConvertToPopupCoordinates(Vector2 worldPos)
    {
        RectTransform popupRect = popupPanel.GetComponent<RectTransform>();

        Vector2 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        float uiX = Mathf.Lerp(-popupRect.rect.width / 2, popupRect.rect.width / 2, viewportPos.x);
        float uiY = Mathf.Lerp(-popupRect.rect.height / 2, popupRect.rect.height / 2, viewportPos.y);

        return new Vector2(uiX, uiY);
    }

    void DrawPopupAxes()
    {
        //if (popupXAxisRenderer == null || popupYAxisRenderer == null) return;

        //RectTransform popupRect = popupPanel.GetComponent<RectTransform>();

        // Convert UI RectTransform coordinates to world space
        Vector3 bottomLeft = popupGraph.position - new Vector3(popupGraph.rect.width / 2, popupGraph.rect.height / 2, 0);
        Vector3 bottomRight = bottomLeft + new Vector3(popupGraph.rect.width, 0, 0);
        Vector3 topLeft = bottomLeft + new Vector3(0, popupGraph.rect.height, 0);

        xAxisRenderer.sortingOrder = 1;
        yAxisRenderer.sortingOrder = 5;

        // ✅ **X-Axis (Time in Seconds)**
        popupXAxisRenderer.positionCount = 2;
        popupXAxisRenderer.SetPosition(0, bottomLeft);
        popupXAxisRenderer.SetPosition(1, bottomRight);
        popupXAxisRenderer.useWorldSpace = false;
        popupXAxisRenderer.gameObject.SetActive(true);
        popupXAxisRenderer.widthMultiplier = 0.05f;
        popupXAxisRenderer.startColor = Color.white;
        popupXAxisRenderer.endColor = Color.white;

        // ✅ **Y-Axis (Volume in mL)**
        popupYAxisRenderer.positionCount = 2;
        popupYAxisRenderer.SetPosition(0, bottomLeft);
        popupYAxisRenderer.SetPosition(1, topLeft);
        popupYAxisRenderer.startColor = Color.white;
        popupYAxisRenderer.endColor = Color.white;
        popupYAxisRenderer.useWorldSpace = false;
        popupYAxisRenderer.gameObject.SetActive(true);
        popupYAxisRenderer.widthMultiplier = 0.05f;

        //popupXAxisRenderer.enabled = true;
        //popupYAxisRenderer.enabled = true;

        //GeneratePopupAxisLabels();

    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        pathPanel.SetActive(false);
        lineRenderer.gameObject.SetActive(true);
        yAxisRenderer.gameObject.SetActive(true);
        xAxisRenderer.gameObject.SetActive(true);

        popupLineRenderer.positionCount = 0;
        popupCoinLineRenderer.positionCount = 0;

        // Destroy all red coin markers
        foreach (GameObject marker in coinMarkers)
        {
            Destroy(marker);
        }
        coinMarkers.Clear();

        foreach (Transform child in xAxisPopupLabelsContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in yAxisPopupLabelsContainer)
        {
            Destroy(child.gameObject);
        }
    }



    private void ShowToolTip(Vector2 position, int index)
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(currentUser);

        if (index >= gameSessions.Count) return;

        tooltipText.SetActive(true);
        tooltipText.GetComponent<RectTransform>().anchoredPosition = position + new Vector2(80f, -18f);

        string date = gameSessions[index].dateTime; // Ensure you have this data
        string score = gameSessions[index].score.ToString();
        string maxVol = gameSessions[index].maxVolumeInhale.ToString();
        string avgFlow = gameSessions[index].avgFlow.ToString();

        if (textT != null)
        {
            textT.text = $" {date}\n Score: {score}\n Max Inhalation: {maxVol}mL\n Average Airflow: {avgFlow}mL/s";
        }
        else
        {
            Debug.LogError(" textT (Tooltip Text) is NOT assigned in the Inspector!");
        }
    }

    void SetupAxisTitles()
    {
        xAxisTitle.text = "Session";
        yAxisTitle.text = "Score";
    }

    void GenerateAxisLabels()
    {
        
        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(PlayerPrefs.GetString("CurrentUser", "Guest"));

        if (gameSessions.Count == 0) return;  // Ensure there's data to process

        // Initialize min and max with the first recorded session score
        float minScore = gameSessions[0].score;
        float maxScore = gameSessions[0].score;

        foreach (var session in gameSessions)
        {
            minScore = Mathf.Min(minScore, session.score);
            maxScore = Mathf.Max(maxScore, session.score);
        }

        if (minScore == maxScore)
        {
            minScore -= 5;  // Small adjustment for better visibility
            maxScore += 5;
        }

        // ✅ Generate Y-Axis Labels (Score Ticks)
        for (int i = 0; i < numYTicks; i++)
        {
            GameObject label = Instantiate(axisLabelPrefab, yAxisLabelsContainer);
            TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();


            // Set score range dynamically
            float scoreValue = Mathf.Lerp(minScore, maxScore, i / (float)(numYTicks - 1));
            labelText.text = $"{scoreValue:F0}";
            labelText.color = Color.black;

            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchoredPosition = new Vector3(-10, i * (graphPanel.rect.height / numYTicks), 0);

        }
    }

    void GeneratePopupAxisLabels(int index)
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(currentUser);

        if (index >= gameSessions.Count) return;

        DataManager.GameSessionData session = gameSessions[index];

        float minX = 0f;
        float maxX = session.gameDuration;

        // ✅ Generate X-Axis Labels (Dates/Time)
        for (int i = 0; i < numXPopupTicks; i++)
        {
            GameObject labelPopup = Instantiate(axisLabelPrefab, xAxisPopupLabelsContainer);
            TextMeshProUGUI labelPopupText = labelPopup.GetComponent<TextMeshProUGUI>();

            float timeValue = Mathf.Lerp(minX, maxX, i / (float)numXPopupTicks);

            labelPopupText.text = $"{timeValue:F1}";  
            labelPopupText.fontSize = 18;
            labelPopupText.enableAutoSizing = false;
            labelPopupText.alignment = TextAlignmentOptions.Right;
            labelPopupText.rectTransform.sizeDelta = new Vector2(60, 30);

            float xPos = i * (popupGraph.rect.width / numXPopupTicks);
            labelPopup.transform.localPosition = new Vector3(xPos, -20, 0);
        }

        // Initialize min and max Y values using the first recorded point in drawnPath
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        float forceM = session.forceMul;



        // Loop through all sessions to find the min and max Y positions from drawnPath

            foreach (Vector2 point in session.drawnPath)
            {
                minY = Mathf.Min(minY, point.y);
                maxY = Mathf.Max(maxY, point.y);
            }
        

        // Ensure there's some difference to avoid division by zero later
        if (Mathf.Approximately(minY, maxY))
        {
            minY -= 5;  // Small adjustment for better visibility
            maxY += 5;
        }

        
        // ✅ Generate Y-Axis Labels (Score Ticks)
        for (int i = 0; i < numYPopupTicks; i++)
        {
            GameObject labelPopup = Instantiate(axisLabelPrefab, yAxisPopupLabelsContainer);
            TextMeshProUGUI labelPopupText = labelPopup.GetComponent<TextMeshProUGUI>();

            // Scale between minY and maxY
            float yValue = minY + ((maxY - minY) * (i / (float)(numYPopupTicks - 1)));
            float yVol = yValue / forceM;

            labelPopupText.text = $"{1.4*yVol:F0}";
            labelPopupText.fontSize = 20;
            labelPopupText.enableAutoSizing = false;
            labelPopupText.alignment = TextAlignmentOptions.Right;
            labelPopupText.rectTransform.sizeDelta = new Vector2(60, 30);

            RectTransform labelRect = labelPopup.GetComponent<RectTransform>();
            labelRect.anchoredPosition = new Vector3(-10, i * (popupGraph.rect.height / numYPopupTicks), 0);

            //Debug.Log($"Label {i}: Value = {yVol}, Position = {labelPosition.y}");
        }
    }

    void DrawAxes()
    {
        Vector3 panelBottomLeft = graphPanel.position - new Vector3(graphPanel.rect.width / 2, graphPanel.rect.height / 2, 0);
        Vector3 panelBottomRight = panelBottomLeft + new Vector3(graphPanel.rect.width, 0, 0);
        Vector3 panelTopLeft = panelBottomLeft + new Vector3(0, graphPanel.rect.height, 0);

        xAxisRenderer.sortingOrder = 5;
        yAxisRenderer.sortingOrder = 5;

        // ✅ Set X-Axis Line
        xAxisRenderer.positionCount = 2;
        xAxisRenderer.SetPosition(0, panelBottomLeft);
        xAxisRenderer.SetPosition(1, panelBottomRight);
        xAxisRenderer.useWorldSpace = false;
        xAxisRenderer.gameObject.SetActive(true);
        xAxisRenderer.widthMultiplier = 0.02f;

        // ✅ Set Y-Axis Line
        yAxisRenderer.positionCount = 2;
        yAxisRenderer.SetPosition(0, panelBottomLeft);
        yAxisRenderer.SetPosition(1, panelTopLeft);
        yAxisRenderer.useWorldSpace = false;
        yAxisRenderer.gameObject.SetActive(true);
        yAxisRenderer.widthMultiplier = 0.02f;

    }

    void DrawGraph()
    {
        string currentUser = PlayerPrefs.GetString("CurrentUser", "Guest");
        List<DataManager.GameSessionData> gameSessions = DataManager.GetUserGameSessions(currentUser);

        if (gameSessions.Count == 0) return;

        float panelWidth = graphPanel.rect.width;
        float panelHeight = graphPanel.rect.height;

        float minScore = int.MaxValue;
        float maxScore = int.MinValue;

        foreach (var session in gameSessions)
        {
            minScore = Mathf.Min(minScore, session.score);
            maxScore = Mathf.Max(maxScore, session.score);
        }

        float xSpacing = panelWidth / Mathf.Max(1, gameSessions.Count - 1);

        for (int i = 0; i < gameSessions.Count; i++)
        {
            float normalizedY = (gameSessions[i].score - minScore) / (maxScore - minScore + 1);
            float xPos = i * xSpacing - panelWidth / 2;
            float yPos = normalizedY * panelHeight - panelHeight / 2;

            Vector2 pointPos = new Vector2(xPos, yPos);
            points.Add(pointPos);

            CreatePoint(pointPos);
        }

        DrawLine();

    }

    void CreatePoint(Vector2 position)
    {
        GameObject newPoint = Instantiate(pointPrefab, graphPanel);
        RectTransform rt = newPoint.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.localScale = Vector3.one * 0.1f;
        newPoint.GetComponent<Image>().color = Color.red;
    }

    void DrawLine()
    {
        if (points.Count < 2) return;

        lineRenderer.positionCount = points.Count;
        
        lineRenderer.widthMultiplier = 0.05f; ;
        lineRenderer.useWorldSpace = false;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 localPoint = graphPanel.TransformPoint(points[i]);
            lineRenderer.SetPosition(i, graphPanel.InverseTransformPoint(localPoint));
        }

        lineRenderer.gameObject.SetActive(true);
    }


}
