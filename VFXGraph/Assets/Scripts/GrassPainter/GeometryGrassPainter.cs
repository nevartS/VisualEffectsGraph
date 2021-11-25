using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GrassPainter
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class GeometryGrassPainter : MonoBehaviour
    {
        [SerializeField] List<Vector3> _positions = new List<Vector3>();
        [SerializeField] List<Color> _colors = new List<Color>();
        [SerializeField] List<int> _indicies = new List<int>();
        [SerializeField] List<Vector3> _normals = new List<Vector3>();
        [SerializeField] List<Vector2> _lengths = new List<Vector2>();

        [Range(1, 600000)] public int GrassLimit = 50000;
        public Color AdjustedColor;
        public int ToolbarInt = 0;

        [SerializeField] private bool _painting;
        [SerializeField] private bool _removing;
        [SerializeField] private bool _editing;

        public float SizeWidth = 1f;
        public float SizeLength = 1f;
        public float Density = 1f;

        public float NormalLimit = 1f;

        public float RangeR;
        public float RangeG;
        public float RangeB;

        public LayerMask HitMask = 1;
        public LayerMask PaintMask = 1;
        public float BrushSize;

        public int I = 0;
        
        private Vector3 _mousePosition;
        private Vector3 _hitPositionGizmo;
        private Vector3 _hitPosition;
        private Vector3 _hitNormal;
        private int[] _indi;
        private Vector3 _lastPosition = Vector3.zero;
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        
        [HideInInspector]
        public Vector3 HitPositionGizmo
        {
            get => _hitPositionGizmo;
            set => _hitPositionGizmo = value;
        }

        [HideInInspector]
        public Vector3 HitNormal
        {
            get => _hitNormal;
            set => _hitNormal = value;
        }

        #if UNITY_EDITOR
        private void OnFocus()
        {
            // Remove delegate listener if it has previously been assigne
            SceneView.duringSceneGui -= this.OnScene;

            // Add (or re-add) the delegate
            SceneView.duringSceneGui += this.OnScene;
        }

        private void OnDestroy()
        {
            // When the window is destroyed, remove the delegate so that it will no longer do any drawing
            SceneView.duringSceneGui -= this.OnScene;
        }

        private void OnEnable()
        {
            _meshFilter = GetComponent<MeshFilter>();
            SceneView.duringSceneGui += this.OnScene;
        }

        public void ClearMesh()
        {
            I = 0;
            _positions = new List<Vector3>();
            _indicies = new List<int>();
            _colors = new List<Color>();
            _normals = new List<Vector3>();
            _lengths = new List<Vector2>();
        }

        private void OnScene(SceneView sceneView)
        {
            // Only allow painting while this object is selected
            if ((Selection.Contains(gameObject)))
            {
                Event e = Event.current;
                RaycastHit terrainHit;
                _mousePosition = e.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                _mousePosition.y = sceneView.camera.pixelHeight - _mousePosition.y * ppp;
                _mousePosition.x *= ppp;

                // Ray for gizmo
                Ray rayGizmo = sceneView.camera.ScreenPointToRay(_mousePosition);
                RaycastHit hitGizmo;

                if (Physics.Raycast(rayGizmo, out hitGizmo, 200f, HitMask.value))
                {
                    _hitPositionGizmo = hitGizmo.point;
                }

                if (e.type == EventType.MouseDrag && e.button == 1 && ToolbarInt == 0)
                {
                    // Place based on density
                    for (int k = 0; k < Density; k++)
                    {
                        // Brush range 
                        float t = 2f * Mathf.PI * Random.Range(0f, BrushSize);
                        float u = Random.Range(0f, BrushSize) + Random.Range(0f, BrushSize);
                        float r = (u > 1 ? 2 - u : u);
                        Vector3 origin = Vector3.zero;

                        // Place random in radius, except for first one
                        if (k != 0)
                        {
                            origin.x += r * Mathf.Cos(t);
                            origin.y += r * Mathf.Sin(t);
                        }
                        else
                        {
                            origin = Vector3.zero;
                        }

                        // Add random range to ray
                        Ray ray = sceneView.camera.ScreenPointToRay(_mousePosition);
                        ray.origin += origin;

                        // If the ray hits something thats on the layer mask, within the grass limit and within the y normal limit
                        if (Physics.Raycast(ray, out terrainHit, 200f, HitMask.value) && I < GrassLimit &&
                            terrainHit.normal.y <= (1 + NormalLimit)
                            && terrainHit.normal.y >= (1 - NormalLimit))
                        {
                            if ((PaintMask.value & (1 << terrainHit.transform.gameObject.layer)) > 0)
                            {
                                _hitPosition = terrainHit.point;
                                _hitNormal = terrainHit.normal;
                                if (k != 0)
                                {
                                    var grassPosition = _hitPosition; // Vector3.Cross(origin, hitNormal)
                                    grassPosition -= this.transform.position;

                                    _positions.Add((grassPosition));
                                    _indicies.Add(I);
                                    _lengths.Add(new Vector2(SizeWidth, SizeLength));

                                    // Add random color variations
                                    _colors.Add(new Color(
                                        AdjustedColor.r + (Random.Range(0, 1.0f) * RangeR),
                                        AdjustedColor.g + (Random.Range(0, 1.0f) * RangeG),
                                        AdjustedColor.b + (Random.Range(0, 1.0f) * RangeB), 1));

                                    // Colors.Add(temp);
                                    _normals.Add(terrainHit.normal);
                                    I++;
                                }
                                else
                                {
                                    // To not place everything at once, check if the first placed point far enough away from the last placed first one
                                    if (Vector3.Distance(terrainHit.point, _lastPosition) > BrushSize)
                                    {
                                        var grassPosition = _hitPosition;
                                        grassPosition -= this.transform.position;
                                        _positions.Add((grassPosition));
                                        _indicies.Add(I);
                                        _lengths.Add(new Vector2(SizeWidth, SizeLength));
                                        _colors.Add(new Color(
                                            AdjustedColor.r + (Random.Range(0, 1.0f) * RangeR),
                                            AdjustedColor.g + (Random.Range(0, 1.0f) * RangeG),
                                            AdjustedColor.b + (Random.Range(0, 1.0f) * RangeB), 1));
                                        _normals.Add(terrainHit.normal);
                                        I++;

                                        if (origin == Vector3.zero)
                                        {
                                            _lastPosition = _hitPosition;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    e.Use();
                }

                // Removing mesh points
                if (e.type == EventType.MouseDrag && e.button == 1 && ToolbarInt == 1)
                {
                    Ray ray = sceneView.camera.ScreenPointToRay(_mousePosition);

                    if (Physics.Raycast(ray, out terrainHit, 200f, HitMask.value))
                    {
                        _hitPosition = terrainHit.point;
                        _hitPositionGizmo = _hitPosition;
                        _hitNormal = terrainHit.normal;

                        for (int j = 0; j < _positions.Count; j++)
                        {
                            Vector3 pos = _positions[j];

                            pos += this.transform.position;
                            float dist = Vector3.Distance(terrainHit.point, pos);

                            // If it's within the radius of the brush, remove all info
                            if (dist <= BrushSize)
                            {
                                _positions.RemoveAt(j);
                                _colors.RemoveAt(j);
                                _normals.RemoveAt(j);
                                _lengths.RemoveAt(j);
                                _indicies.RemoveAt(j);
                                I--;

                                for (int i = 0; i < _indicies.Count; i++)
                                {
                                    _indicies[i] = i;
                                }
                            }
                        }
                    }

                    e.Use();
                }

                if (e.type == EventType.MouseDrag && e.button == 1 && ToolbarInt == 2)
                {
                    Ray ray = sceneView.camera.ScreenPointToRay(_mousePosition);

                    if (Physics.Raycast(ray, out terrainHit, 200f, HitMask.value))
                    {
                        _hitPosition = terrainHit.point;
                        _hitPositionGizmo = _hitPosition;
                        _hitNormal = terrainHit.normal;
                        for (int j = 0; j < _positions.Count; j++)
                        {
                            Vector3 pos = _positions[j];

                            pos += this.transform.position;
                            float dist = Vector3.Distance(terrainHit.point, pos);

                            // if its within the radius of the brush, remove all info
                            if (dist <= BrushSize)
                            {
                                _colors[j] = (new Color(
                                    AdjustedColor.r + (Random.Range(0, 1.0f) * RangeR),
                                    AdjustedColor.g + (Random.Range(0, 1.0f) * RangeG),
                                    AdjustedColor.b + (Random.Range(0, 1.0f) * RangeB), 1));

                                _lengths[j] = new Vector2(SizeWidth, SizeLength);
                            }
                        }
                    }

                    e.Use();
                }

                // set all info to mesh
                _mesh = new Mesh();
                _mesh.SetVertices(_positions);
                _indi = _indicies.ToArray();
                _mesh.SetIndices(_indi, MeshTopology.Points, 0);
                _mesh.SetUVs(0, _lengths);
                _mesh.SetColors(_colors);
                _mesh.SetNormals(_normals);
                _meshFilter.mesh = _mesh;
            }
        }
    }
    #endif
}