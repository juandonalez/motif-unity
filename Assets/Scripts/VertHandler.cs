using UnityEngine;
using UnityEditor;
using System.Collections;
 
[ExecuteInEditMode]
 
public class VertHandler : MonoBehaviour 
{
     Mesh mesh;
     Vector3[] verts;
     Vector3 vertPos;
     GameObject[] handles;
     
     void OnEnable()
     {
         mesh = GetComponent<MeshFilter>().mesh;
         verts = mesh.vertices;

         foreach(Vector3 vert in verts)
         {
             vertPos = transform.TransformPoint(vert);
             GameObject handle = new GameObject("handle");
             handle.transform.position = vertPos;
             handle.transform.parent = transform;
             handle.tag = "handle";
             //handle.AddComponent<Gizmo_Sphere>();
             //print(vertPos);
         }
		 
		/*Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Clears all the data that the mesh currently has
        mesh.Clear();

        // create 3 vertices for the triangle
        mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)};
        mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
        mesh.triangles = new int[] {0, 1, 2};*/

		/*mesh = GetComponent<MeshFilter>().mesh;
		AssetDatabase.CreateAsset(mesh, "Assets/Meshes/Triangle.asset");
		AssetDatabase.SaveAssets();*/
     }
     
     void OnDisable()
     {
         GameObject[] handles = GameObject.FindGameObjectsWithTag("handle");
         foreach(GameObject handle in handles)
         {
             DestroyImmediate(handle);    
         }
     }
     
     void Update()
     {
         handles = GameObject.FindGameObjectsWithTag ("handle");
         for(int i = 0; i < verts.Length; i++)
         {
             verts[i] = handles[i].transform.localPosition;    
         }
         mesh.vertices = verts;
         mesh.RecalculateBounds();
         mesh.RecalculateNormals();
	}
}