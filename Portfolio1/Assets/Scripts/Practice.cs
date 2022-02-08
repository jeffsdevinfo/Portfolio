using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Practice : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 currentLinePosition;
    float rotationRadius;
    int numberOfVertices = 100;
    LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        lr = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawNextLine();
        }
    }

    void DrawNextLine()
    {
        lr.SetPosition(0, gameObject.transform.position);
        lr.SetPosition(1, gameObject.transform.position + new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z));
    }


}
