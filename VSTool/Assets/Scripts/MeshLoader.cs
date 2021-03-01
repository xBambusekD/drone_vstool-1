using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLoader : MonoBehaviour
{
    public GameObject renderer;

    /// <summary>
    /// Creates a binary dump of a mesh
    /// </summary>
    public void MeshDump()
    {
        Mesh myMesh = renderer.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.FileStream fs = new System.IO.FileStream(Application.dataPath + "meshFile.dat", System.IO.FileMode.Create);
        SerializableMeshInfo smi = new SerializableMeshInfo(myMesh);
        // bf.Serialize(fs, smi);
        // fs.Close();
    }
    /// <summary>
    /// Loads a mesh from a binary dump
    /// </summary>

    public GameObject loader;
    public void MeshUndump()
    {
        if (!System.IO.File.Exists(Application.dataPath + "meshFile.dat"))
        {
            Debug.LogError("meshFile.dat file does not exist.");
            return;
        }
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        System.IO.FileStream fs = new System.IO.FileStream(Application.dataPath + "meshFile.dat", System.IO.FileMode.Open);
        SerializableMeshInfo smi = (SerializableMeshInfo)bf.Deserialize(fs);
        loader.GetComponent<MeshFilter>().mesh = smi.GetMesh();
        fs.Close();
    }
}
