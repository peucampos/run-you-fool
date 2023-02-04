using Unity.VisualScripting;
using UnityEngine;

public class MoveLava : MonoBehaviour
{
    public float scrollSpeed;
    Renderer rend;
    private float randDirX;
    private float randDirY;

    void Start()
    {
        rend = GetComponent<Renderer>();
        randDirX = Random.Range(-1f, 1f);
        randDirY = Random.Range(-1f, 1f);
        scrollSpeed = Random.Range(0.5f, 1f);
    }

    void Update()
    {
        float moveThisX = Time.time * scrollSpeed * randDirX;
        float moveThisY = Time.time * scrollSpeed * randDirY;
        rend.material.SetTextureOffset("_MainTex", new Vector2(moveThisX, moveThisY));
    }
}
