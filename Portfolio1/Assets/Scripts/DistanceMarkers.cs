using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMarkers : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] TextMesh tm;
    [SerializeField] GameObject plane;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawDistancePerimeters();
    }

    void DrawDistancePerimeters()
    {
        //Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
        //                                        new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
        //                                        new Vector4(0, 0, 1, 0),
        //                                        new Vector4(0, 0, 0, 1));
        
        //DrawPolygon(100, 10, new Vector3(0, 0.01f, 0), .1f, .1f, Vector3.forward);
        int max = 1000;
        int lineIndex = 0;
        for(int i = 10; i < max; i+=10)
        {
            DrawPolygon(100, i, new Vector3(0, 0.01f, 0), .1f, .1f, Vector3.forward, ref lineIndex);
        }
    }

    void DrawPolygonOld(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)//, Vector3 axisRotation, Matrix4x4 rotMat)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                                     new Vector4(0, 0, 1, 0),
                                                     new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            //Vector3 initialRelativePosition = axisRotation * radius;
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
            //lineRenderer.SetPosition(i, centerPos + rotMat.MultiplyPoint(initialRelativePosition));

        }
    }

    void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Vector3 axisRotation, ref int lineIndex)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = false;
        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;
        lineRenderer.SetPosition(lineIndex, radius * new Vector3(axisRotation.x * radius, axisRotation.y * radius, axisRotation.z * radius));
        for (int i = 0; i < vertexNumber; i++)
        {
            lineIndex = i;
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), 0, -Mathf.Sin(angle * i), 0),
                                                    new Vector4(0, 1, 0, 0),
                                                    new Vector4(Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
                                                    new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(axisRotation.x * radius, axisRotation.y * radius, axisRotation.z * radius);            
            lineRenderer.SetPosition(lineIndex, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
            //TextMesh newTM = Instantiate(tm, new Vector3(0, .001f, radius), Quaternion.identity);
            //newTM.text = radius.ToString();
            
        }                
    }
}
