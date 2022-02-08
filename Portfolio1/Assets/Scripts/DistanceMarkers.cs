using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceMarkers : MonoBehaviour
{
    float rotationRadius;
    int numberOfCircles = 40;
    int numberOfVerticesPerCircle = 100;
    float circleIncrementDistance = 100;

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
    }


    //LineRenderer lineRenderer;
    //[SerializeField] TextMesh tm;
    //[SerializeField] GameObject plane;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    lineRenderer = gameObject.GetComponent<LineRenderer>();
    //    DrawMarkers();
    //}

    //void DrawMarkers()
    //{
    //    for(int i = 10; i <= 1000; i+=10)
    //    {
    //        TextMesh newTM = Instantiate(tm, new Vector3(0, .001f, i), Quaternion.AngleAxis(90, Vector3.right));
    //        newTM.text = i.ToString();

    //        TextMesh newTM1 = Instantiate(tm, new Vector3(i, .001f, 0), Quaternion.AngleAxis(90, Vector3.right));
    //        newTM1.text = i.ToString();

    //        TextMesh newTM2 = Instantiate(tm, new Vector3(0, .001f, -i), Quaternion.AngleAxis(90, Vector3.right));
    //        newTM2.text = i.ToString();

    //        TextMesh newTM3 = Instantiate(tm, new Vector3(-i, .001f, 0), Quaternion.AngleAxis(90, Vector3.right));
    //        newTM3.text = i.ToString();
    //    }
    //}


    //// Update is called once per frame
    //void Update()
    //{
    //    DrawDistancePerimeters();
    //}

    //void DrawDistancePerimeters()
    //{
    //    //Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
    //    //                                        new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
    //    //                                        new Vector4(0, 0, 1, 0),
    //    //                                        new Vector4(0, 0, 0, 1));

    //    //DrawPolygon(100, 10, new Vector3(0, 0.01f, 0), .1f, .1f, Vector3.forward);
    //    int max = 1000;
    //    int lineIndex = 0;
    //    for(int i = 10; i < max; i+=10)
    //    {
    //        DrawPolygon(100, i, new Vector3(0, 0.01f, 0), .1f, .1f, Vector3.forward, ref lineIndex);
    //    }
    //}

    //void DrawPolygonOld(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)//, Vector3 axisRotation, Matrix4x4 rotMat)
    //{
    //    lineRenderer.startWidth = startWidth;
    //    lineRenderer.endWidth = endWidth;
    //    lineRenderer.loop = true;
    //    float angle = 2 * Mathf.PI / vertexNumber;
    //    lineRenderer.positionCount = vertexNumber;

    //    for (int i = 0; i < vertexNumber; i++)
    //    {
    //        Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
    //                                                 new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
    //                                                 new Vector4(0, 0, 1, 0),
    //                                                 new Vector4(0, 0, 0, 1));
    //        Vector3 initialRelativePosition = new Vector3(0, radius, 0);
    //        //Vector3 initialRelativePosition = axisRotation * radius;
    //        lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
    //        //lineRenderer.SetPosition(i, centerPos + rotMat.MultiplyPoint(initialRelativePosition));

    //    }
    //}

    //void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Vector3 axisRotation, ref int lineIndex)
    //{
    //    lineRenderer.startWidth = startWidth;
    //    lineRenderer.endWidth = endWidth;
    //    lineRenderer.loop = false;
    //    float angle = 2 * Mathf.PI / vertexNumber;
    //    lineRenderer.positionCount = vertexNumber;
    //    lineRenderer.SetPosition(lineIndex, radius * new Vector3(axisRotation.x * radius, axisRotation.y * radius, axisRotation.z * radius));
    //    for (int i = 1; i < vertexNumber; i++)
    //    {
    //        lineIndex = i;
    //        Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), 0, -Mathf.Sin(angle * i), 0),
    //                                                new Vector4(0, 1, 0, 0),
    //                                                new Vector4(Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
    //                                                new Vector4(0, 0, 0, 1));
    //        Vector3 initialRelativePosition = new Vector3(axisRotation.x * radius, axisRotation.y * radius, axisRotation.z * radius);
    //        lineRenderer.SetPosition(lineIndex, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

    //    }
    //}

    //void DrawPolygonPrac(Vector3 startPoint, Vector3 axisOfRotation, int numberOfPoints)
    //{
    //    float individualVertexAngle = 2 * Mathf.PI / numberOfPoints; // radian angle
    //    for(int i = 1; i <= numberOfPoints; i++)
    //    {

    //    }
    //}
}
