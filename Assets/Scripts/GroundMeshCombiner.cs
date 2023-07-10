using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMeshCombiner : MonoBehaviour
{
    void Start()
    {
        var filters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] instances = new CombineInstance[filters.Length];
        for (int i = 0; i < filters.Length; i++) {
            instances[i] = new CombineInstance() {
                mesh = filters[i].sharedMesh,
                transform = filters[i].transform.localToWorldMatrix,
            };
        }
        var combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(instances, true, true, false);
        GetComponent<MeshCollider>().sharedMesh = combinedMesh;
    }
}
