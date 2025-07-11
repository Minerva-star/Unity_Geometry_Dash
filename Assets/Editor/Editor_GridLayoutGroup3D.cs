using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GridLayoutGroup3D)),CanEditMultipleObjects]

public class Editor_GridLayoutGroup3D : Editor
{
    private GridLayoutGroup3D gridLayoutGroup3D;
    // Start is called before the first frame update
    private void OnEnable()
    {
        gridLayoutGroup3D = target as GridLayoutGroup3D;
        gridLayoutGroup3D.CopyInfo();
    }
    public override void OnInspectorGUI()
    {
        gridLayoutGroup3D.MatrixInterval = EditorGUILayout.Vector3Field("子物体间隔", gridLayoutGroup3D.MatrixInterval);
        GUILayout.Space(10);
        gridLayoutGroup3D.MatrixSize = EditorGUILayout.Vector3IntField("矩阵大小", gridLayoutGroup3D.MatrixSize);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("矩阵显示调节:");
        gridLayoutGroup3D.MatrixColor = EditorGUILayout.ColorField(gridLayoutGroup3D.MatrixColor);
        GUILayout.Space(10);
        float MaxValue = Mathf.Max(new float[3] { gridLayoutGroup3D.MatrixInterval.x, gridLayoutGroup3D.MatrixInterval.y, gridLayoutGroup3D.MatrixInterval.z });
        gridLayoutGroup3D.Radius = EditorGUILayout.Slider(gridLayoutGroup3D.Radius,0.01f, MaxValue/2);
        GUILayout.EndHorizontal();
        if (gridLayoutGroup3D.MatrixSize.y==0|| gridLayoutGroup3D.MatrixSize.x==0|| gridLayoutGroup3D.MatrixSize.z==0)
        {
            EditorGUILayout.HelpBox("矩阵大小，最小值为1", MessageType.Info);
        }
        GUILayout.Space(10);
        if (GUILayout.Button("移除并恢复"))
        {
            if (EditorUtility.DisplayDialog("警告！", "这将使当前的子物体恢复到开始状态。", "是的", "手滑了~"))
            {
                Debug.Log(gridLayoutGroup3D.PasteInfo());
                DestroyImmediate(gridLayoutGroup3D);
            }
        }
    }
}