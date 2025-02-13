using UnityEngine;

public class Parallax : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private float animationSpeed = 0.3f;
    //private float offset;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    void Update()
    {
            //offset += 0.01f * Time.deltaTime;
            meshRenderer.material.mainTextureOffset += new Vector2(animationSpeed * Time.deltaTime, 0);
    }

    public void SetParallaxSpeed(float newSpeed)
    {
        animationSpeed = newSpeed;
    }
}
