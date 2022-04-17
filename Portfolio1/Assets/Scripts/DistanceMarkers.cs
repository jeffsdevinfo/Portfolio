// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceMarkers : MonoBehaviour
{
    float rotationRadius;
    int numberOfCircles = 20;
    int numberOfVerticesPerCircle = 100;
    float circleIncrementDistance = 500;

    [SerializeField] GameObject MarkerContainer;

    LineRenderer lr;
    [SerializeField] TMPro.TMP_Text textPrefab;
    // Start is called before the first frame update
    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.loop = false;

        DrawMeasures();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DrawMeasures()
    {
        int totalNumberOfVertices = numberOfCircles + ((numberOfVerticesPerCircle + 1) * numberOfCircles);
        lr.positionCount = totalNumberOfVertices;
        Vector3 currentPosition = gameObject.transform.position;
        int circleIndex = 0;
        lr.SetPosition(circleIndex, currentPosition);
        for (int i = 0; i < numberOfCircles; i++)
        {
            DrawNextLineWithIndex(ref circleIndex, ref currentPosition);
        }
    }

    void DrawNextLineWithIndex(ref int circleIndex, ref Vector3 position)
    {
        float stepAngle = Mathf.PI * 2 / numberOfVerticesPerCircle;
        circleIndex++;
        position = position + new Vector3(0, 0, circleIncrementDistance);
        PlaceDistanceTextMarker(position);
        lr.SetPosition(circleIndex++, position);
        for (int i = 1; i <= numberOfVerticesPerCircle; i++)
        {
            position = Quaternion.AngleAxis(Mathf.Rad2Deg * stepAngle, Vector3.up) * position;
            lr.SetPosition(circleIndex++, position);
        }
    }

    void PlaceDistanceTextMarker(Vector3 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y + 2, position.z);
        TMP_Text textInstance = Instantiate<TMP_Text>(textPrefab, newPosition, Quaternion.AngleAxis(90.0f, Vector3.right));
        textInstance.text = ((int)Mathf.RoundToInt(position.z)).ToString();
        textInstance.transform.SetParent(MarkerContainer.transform);
    }
}
