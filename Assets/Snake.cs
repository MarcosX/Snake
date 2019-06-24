using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{

    Vector2 dir = Vector2.right;

    List<GameObject> tail = new List<GameObject>();

    bool ate = false;
    
    public float startSpeed = 0.3f;
    public float currentSpeed = 0.3f;
    public float foodAccelerator = 0.01f;

    public Transform borderTop;
    public Transform borderRight;
    public Transform borderBottom;
    public Transform borderLeft;
    public GameObject foodPrefab;
    public GameObject tailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = startSpeed;
        InvokeRepeating("Move", startSpeed, startSpeed);
        SpawnFood();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) && dir != Vector2.left)
            dir = Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow) && dir != Vector2.right)
            dir = Vector2.left;
        else if (Input.GetKey(KeyCode.UpArrow) && dir != Vector2.down)
            dir = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow) && dir != Vector2.up)
            dir = Vector2.down;
    }

    void Move()
    {
        Vector2 headPreviousPosition = transform.position;
        transform.Translate(dir);

        if (ate)
        {
            GameObject newTail = (GameObject)Instantiate(tailPrefab, headPreviousPosition, Quaternion.identity);
            tail.Insert(0, newTail);
            
            float factor = (tail.Count % 5) == 0 ? 1.0f : 0.0f;
            borderTop.position = new Vector2(borderTop.position.x, borderTop.position.y + factor);
            borderRight.position = new Vector2(borderRight.position.x + factor, borderRight.position.y);
            borderBottom.position = new Vector2(borderBottom.position.x, borderBottom.position.y - factor);
            borderLeft.position = new Vector2(borderLeft.position.x - factor, borderLeft.position.y);

            SpawnFood();
            CancelInvoke("Move");
            if (currentSpeed > 0.05f)
            {
                currentSpeed -= foodAccelerator + (0.005f * (((int)tail.Count/10)));
            }
            InvokeRepeating("Move", currentSpeed, currentSpeed);
            ate = false;
        }
        else if (tail.Count > 0)
        {
            tail.Last().transform.position = headPreviousPosition;
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count-1);
        }
    }

    void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        if (colliderInfo.name.StartsWith("FoodPrefab"))
        {
            ate = true;
            Destroy(colliderInfo.gameObject);
        }
        if (colliderInfo.name.StartsWith("Border") || colliderInfo.name.StartsWith("TailPrefab"))
        {
            ResetGame();
        }
    }

    void SpawnFood()
    {
        int x = (int)Random.Range(borderLeft.position.x + 1.0f, borderRight.position.x - 1.0f);
        int y = (int)Random.Range(borderBottom.position.y + 1.0f, borderTop.position.y - 1.0f);

        Instantiate(foodPrefab, new Vector2(x, y), Quaternion.identity);
    }

    void ResetGame()
    {
        foreach (var item in tail)
        {
            Destroy(item);
        }
        tail.Clear();
        transform.position = Vector2.zero;
        CancelInvoke("Move");
        InvokeRepeating("Move", startSpeed, startSpeed);
        currentSpeed = startSpeed;
        borderTop.position = new Vector2(0.0f, 6.5f);
        borderRight.position = new Vector2(10.5f, 0.0f);
        borderBottom.position = new Vector2(0.0f, -6.5f);
        borderLeft.position = new Vector2(-10.5f, 0.0f);
        dir = Vector2.right;
    }
}
