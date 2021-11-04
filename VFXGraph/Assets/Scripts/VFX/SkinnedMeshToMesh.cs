using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace VFX
{
    public class SkinnedMeshToMesh : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _skinnedMesh;
        [SerializeField] private VisualEffect _vfxGraph;

        [SerializeField] private float _refreshRate;

        private void Start()
        {
            StartCoroutine(UpdateVfxGraph());
        }

        private IEnumerator UpdateVfxGraph()
        {
            while (gameObject.activeSelf)
            {
                Mesh mesh = new Mesh();
                _skinnedMesh.BakeMesh(mesh);
                
                Vector3[] vertices = mesh.vertices;
                Mesh mesh02 = new Mesh();
                mesh02.vertices = vertices;
                mesh02.triangles = mesh.triangles;
                mesh02.normals = mesh.normals;
                
                _vfxGraph.SetMesh("CharacterMesh", mesh);
            
                yield return new WaitForSeconds(_refreshRate);
            }
        }
    }
}
