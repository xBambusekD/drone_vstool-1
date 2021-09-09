using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapGenerator : MonoBehaviour
{
    public Material heatMapMaterial;
    public int imageWidth;
    public int imageHeight;

    private List<List<Vector3>> vertices2d = new List<List<Vector3>>();
    private List<List<int>> triangles2d = new List<List<int>>();
    private List<List<Color>> colors2d = new List<List<Color>>();
    private List<GameObject> MeshGOList = new List<GameObject>();

    public Gradient gradient;

    // Start is called before the first frame update
    void Start() {
        Texture2D hMap = Resources.Load("clouds1") as Texture2D;
        imageHeight = hMap.height;
        imageWidth = hMap.width;
        int size = imageWidth * imageHeight;
        

        if (imageHeight > 65000 || imageWidth > 65000)
            return;

        // kolko mozem stlpcov spravit
        int temp = 65000 / imageWidth -1;
        
        int k = 0;
        int sameRow = 0;
        vertices2d.Add(new List<Vector3>());
        triangles2d.Add(new List<int>());
        colors2d.Add(new List<Color>());

        for (int i = 0; i < imageWidth; i++) {
            for (int j = 0; j < imageHeight; j++) {
                if (j == 0 && i % temp == 0 && i != 0) {
                    k++;
                    vertices2d.Add(new List<Vector3>());
                    triangles2d.Add(new List<int>());
                    colors2d.Add(new List<Color>());
                    sameRow = 1;
                }

                Color vertexColor = gradient.Evaluate(hMap.GetPixel(i, j).grayscale);
                if (sameRow == 1) {
                    vertices2d[k].Add(new Vector3(i, hMap.GetPixel(i, j).grayscale * 100, j));
                    colors2d[k].Add(vertexColor);
                    vertices2d[k - 1].Add(new Vector3(i, hMap.GetPixel(i, j).grayscale * 100, j));
                    colors2d[k - 1].Add(vertexColor);
                } else {
                    vertices2d[k].Add(new Vector3(i, hMap.GetPixel(i, j).grayscale * 100, j));
                    colors2d[k].Add(vertexColor);
                }
                    

                
                


                if (j == imageHeight - 1) {
                    sameRow = 0;
                }


            }
        }

        for (int kk = 0; kk <= k; kk++) {
            for (int i = 0; i <= temp; i++) {
                for (int j = 0; j < imageWidth; j++) {
                    if (i == 0 || j == 0)
                        continue;
                    if (kk == k)
                        temp = vertices2d[k].Count / imageWidth - 1;
                    triangles2d[kk].Add(imageWidth * i + j); //Top right
                    triangles2d[kk].Add(imageWidth * i + j - 1); //Bottom right
                    triangles2d[kk].Add(imageWidth * (i - 1) + j - 1); //Bottom left - First triangle
                    triangles2d[kk].Add(imageWidth * (i - 1) + j - 1); //Bottom left 
                    triangles2d[kk].Add(imageWidth * (i - 1) + j); //Top left
                    triangles2d[kk].Add(imageWidth * i + j); //Top right - Second triangle
                }
            }
        }
        
        for (int i = 0; i <= k; i++) {
            GameObject renderer = new GameObject("HeatMapRenderer" + i);
            renderer.AddComponent<MeshFilter>();
            renderer.AddComponent<MeshRenderer>();
            renderer.GetComponent<MeshRenderer>().material = heatMapMaterial;
            MeshGOList.Add(renderer);
            Mesh mesh = new Mesh();
            mesh.vertices = vertices2d[i].ToArray();
            mesh.colors = colors2d[i].ToArray();
            mesh.triangles = triangles2d[i].ToArray();
            mesh.RecalculateNormals();
            MeshGOList[i].GetComponent<MeshFilter>().mesh = mesh;
        }

       

        ////for (int i = 0, k = 0; i < imageWidth; i++) {
        ////    for (int j = 0; j < imageHeight; j++) {
        ////        if (i * imageWidth + j > (k + 1) * 65000)
        ////            k++;
        ////        vertices2d[k].Add(new Vector3(i, hMap.GetPixel(i, j).grayscale * 100, j));
        ////        if (i == 0 || j == 0)
        ////            continue;

        //triangles2d[k].Add(imageWidth * i + j); //Top right
        //triangles2d[k].Add(imageWidth * i + j - 1); //Bottom right
        //triangles2d[k].Add(imageWidth * (i - 1) + j - 1); //Bottom left - First triangle
        //triangles2d[k].Add(imageWidth * (i - 1) + j - 1); //Bottom left 
        //triangles2d[k].Add(imageWidth * (i - 1) + j); //Top left
        //triangles2d[k].Add(imageWidth * i + j); //Top right - Second triangle
        ////    }
        ////}

        for (int i = 0; i <= k; i++) {
           
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
