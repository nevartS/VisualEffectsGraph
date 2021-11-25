using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GrassPainter
{
    [CustomEditor(typeof(GeometryGrassPainter))]
    public class GrassPainterEditor : Editor
    {
        private GeometryGrassPainter _grassPainter;
        private readonly string[] _toolbarStrings = {"Add", "Remove", "Edit"};

        private void OnEnable()
        {
            _grassPainter = (GeometryGrassPainter) target;
        }

        private void OnSceneGUI()
        {
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
            
            Handles.color = new Color(0, 0.5f, 0.5f, 0.4f);
            Handles.DrawSolidDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
            
            if (_grassPainter.ToolbarInt == 1)
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
                
                Handles.color = new Color(0.5f, 0f, 0f, 0.4f);
                Handles.DrawSolidDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
            }

            if (_grassPainter.ToolbarInt == 2)
            {
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
                
                Handles.color = new Color(0.5f, 0.5f, 0f, 0.4f);
                Handles.DrawSolidDisc(_grassPainter.HitPositionGizmo, _grassPainter.HitNormal, _grassPainter.BrushSize);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Grass Limit", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_grassPainter.I.ToString() + "/", EditorStyles.label);
            _grassPainter.GrassLimit = EditorGUILayout.IntField(_grassPainter.GrassLimit);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Paint Status (Right-Mouse Button to paint)", EditorStyles.boldLabel);
            _grassPainter.ToolbarInt = GUILayout.Toolbar(_grassPainter.ToolbarInt, _toolbarStrings);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);
            LayerMask tempMask = EditorGUILayout.MaskField("Hit Mask",
                InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_grassPainter.HitMask),
                InternalEditorUtility.layers);
            _grassPainter.HitMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            LayerMask tempMask2 = EditorGUILayout.MaskField("Painting Mask",
                InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_grassPainter.PaintMask),
                InternalEditorUtility.layers);
            _grassPainter.PaintMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask2);

            _grassPainter.BrushSize = EditorGUILayout.Slider("Brush Size", _grassPainter.BrushSize, 0.1f, 10f);
            _grassPainter.Density = EditorGUILayout.Slider("Density", _grassPainter.Density, 0.1f, 10f);
            _grassPainter.NormalLimit = EditorGUILayout.Slider("Normal Limit", _grassPainter.NormalLimit, 0f, 1f);


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Width and Length ", EditorStyles.boldLabel);
            _grassPainter.SizeWidth = EditorGUILayout.Slider("Grass Width", _grassPainter.SizeWidth, 0f, 2f);
            _grassPainter.SizeLength = EditorGUILayout.Slider("Grass Length", _grassPainter.SizeLength, 0f, 2f);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
            _grassPainter.AdjustedColor = EditorGUILayout.ColorField("Brush Color", _grassPainter.AdjustedColor);
            EditorGUILayout.LabelField("Random Color Variation", EditorStyles.boldLabel);
            _grassPainter.RangeR = EditorGUILayout.Slider("Red", _grassPainter.RangeR, 0f, 1f);
            _grassPainter.RangeG = EditorGUILayout.Slider("Green", _grassPainter.RangeG, 0f, 1f);
            _grassPainter.RangeB = EditorGUILayout.Slider("Blue", _grassPainter.RangeB, 0f, 1f);

            if (GUILayout.Button("Clear Mesh"))
            {
                if (EditorUtility.DisplayDialog("Clear Painted Mesh?",
                    "Are you sure you want to clear the mesh?", "Clear", "Don't Clear"))
                {
                    _grassPainter.ClearMesh();
                }
            }
        }
    }
}