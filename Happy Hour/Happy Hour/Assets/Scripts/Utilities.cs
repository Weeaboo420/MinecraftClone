using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Mesh WeldVertices(Mesh aMesh, float aMaxDelta = 0.01f) 
    {
        var verts = aMesh.vertices;
        var normals = aMesh.normals;
        var uvs = aMesh.uv;
        Dictionary<Vector3, int> duplicateHashTable = new Dictionary<Vector3, int>();
        List<int> newVerts = new List<int>();
        int[] map = new int[verts.Length];

        //create mapping and find duplicates, dictionaries are like hashtables, mean fast
        for (int i = 0; i < verts.Length; i++) {
            if (!duplicateHashTable.ContainsKey(verts[i])) {
                duplicateHashTable.Add(verts[i], newVerts.Count);
                map[i] = newVerts.Count;
                newVerts.Add(i);
            }
            else {
                map[i] = duplicateHashTable[verts[i]];
            }
        }

        // create new vertices
        var verts2 = new Vector3[newVerts.Count];
        var normals2 = new Vector3[newVerts.Count];
        var uvs2 = new Vector2[newVerts.Count];
        for (int i = 0; i < newVerts.Count; i++) {
            int a = newVerts[i];
            verts2[i] = verts[a];
            normals2[i] = normals[a];
            uvs2[i] = uvs[a];
        }
        // map the triangle to the new vertices
        var tris = aMesh.triangles;
        for (int i = 0; i < tris.Length; i++) {
            tris[i] = map[tris[i]];
        }
        aMesh.triangles = tris;
        aMesh.vertices = verts2;
        aMesh.normals = normals2;
        aMesh.uv = uvs2;

        aMesh.RecalculateBounds();
        aMesh.RecalculateNormals();

        return aMesh;
    }
}
