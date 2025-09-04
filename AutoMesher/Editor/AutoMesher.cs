using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AutoMesher : AssetPostprocessor
{
    void OnPostprocessModel(GameObject importedModel)
    {
        string path = assetImporter.assetPath;
        if (Path.GetExtension(path).ToLower() != ".obj") return;

        MeshFilter[] meshFilters = importedModel.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combine.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "AutoMeshed_" + importedModel.name;
        combinedMesh.CombineMeshes(combine.ToArray(), true, true);

        // Replace root mesh
        MeshFilter rootFilter = importedModel.GetComponent<MeshFilter>();
        if (rootFilter == null)
            rootFilter = importedModel.AddComponent<MeshFilter>();

        rootFilter.sharedMesh = combinedMesh;

        MeshRenderer rootRenderer = importedModel.GetComponent<MeshRenderer>();
        if (rootRenderer == null)
            rootRenderer = importedModel.AddComponent<MeshRenderer>();

        Debug.Log($"AutoMesher: Combined {meshFilters.Length} meshes into one for {importedModel.name}");
    }
}
