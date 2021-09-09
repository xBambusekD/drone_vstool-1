using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using UnityEngine;

public class CubePoints : ObjectPoints
{
    readonly int FaceCount = 6;
    readonly int FaceVertexCount = 4;
    List<List<Vector3>> Faces = new List<List<Vector3>>();

    public void Calculate()
    {
        CalculateCornerPoints();
        /*int i = 0;
        foreach (var VARIABLE in CornerPoints)
        {
            new GameObject("auto" + i).transform.position = new Vector3(VARIABLE.x,VARIABLE.y,VARIABLE.z);
            i++;
        }*/
        float width = Vector3.Distance(CornerPoints[0],CornerPoints[1]);
        float height = Vector3.Distance(CornerPoints[0], CornerPoints[2]);
        new GameObject("l").transform.position = new Vector3(CornerPoints[0].x, CornerPoints[0].y, CornerPoints[0].z);
        new GameObject("r").transform.position = new Vector3(CornerPoints[1].x, CornerPoints[1].y, CornerPoints[2].z);
        Debug.Log(ObjectLocalVertices[0]);
        Debug.Log(ObjectLocalVertices[1]);
        Debug.Log(width);
        Debug.Log(height);

    }

    protected override void CalculateCornerPoints()   //Random point on cube calculated with faces, so faces are defined as well
    {
        base.CalculateCornerPoints();
        List<Vector3> OneFace = new List<Vector3>();

        for (int i = 0; i < ObjectVertices.Count; i++)
        {
            OneFace.Add(ObjectVertices[i]);
            if ((i + 1) % FaceVertexCount == 0)
            {
                Faces.Add(OneFace);
                OneFace = new List<Vector3>();
            }
        }
        CornerPoints = ObjectUniqueVertices;
    }

    private int GetRandomFaceIndex()
    {
        return Random.Range(0, FaceCount - 1);
    }
    protected override void CalculateEdgeVectors(int VectorCornerIdx, int faceIndex)
    {
        base.CalculateEdgeVectors(VectorCornerIdx, faceIndex);
        EdgeVectors.Add(Faces[faceIndex][3] - Faces[faceIndex][VectorCornerIdx]);
        EdgeVectors.Add(Faces[faceIndex][1] - Faces[faceIndex][VectorCornerIdx]);
    }

    protected override void CalculateRandomPoint()
    {
        int randomCornerIdx = Random.Range(0, 2) == 0 ? 0 : 2;
        int randomFaceIdx = GetRandomFaceIndex();
        CalculateEdgeVectors(randomCornerIdx, randomFaceIdx);

        float u = Random.Range(0.0f, 1.0f);
        float v = Random.Range(0.0f, 1.0f);

        if (v + u > 1) //sum of coordinates should be smaller than 1 for the point be inside the triangle
        {
            v = 1 - v;
            u = 1 - u;
        }

        RandomPoint = Faces[randomFaceIdx][randomCornerIdx] + u * EdgeVectors[0] + v * EdgeVectors[1];
        Debug.Log(RandomPoint);
    }
}
