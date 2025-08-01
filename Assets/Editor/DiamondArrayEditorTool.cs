using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DiamondArrayEditorTool : EditorWindow
{
    private GameObject diamondPrefab;
    private float radius = 5f;
    private float startAngle = 0f;
    private float endAngle = 360f;
    private float angleInterval = 15f;
    private bool faceCenter = true;
    private Vector3 diamondScale = Vector3.one;
    private Color diamondColor = Color.white;
    private bool showGizmos = true;
    
    private Vector3 centerPosition = Vector3.zero;
    private List<GameObject> previewDiamonds = new List<GameObject>();
    private bool isPreviewActive = false;
    
    [MenuItem("Tools/菱形阵列编辑器")]
    public static void ShowWindow()
    {
        DiamondArrayEditorTool window = GetWindow<DiamondArrayEditorTool>("菱形阵列编辑器");
        window.minSize = new Vector2(300, 500);
    }
    
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        ClearPreview();
    }
    
    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("圆形菱形阵列编辑器", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        DrawBasicSettings();
        EditorGUILayout.Space();
        DrawAngleSettings();
        EditorGUILayout.Space();
        DrawDiamondSettings();
        EditorGUILayout.Space();
        DrawActionButtons();
        EditorGUILayout.Space();
        DrawInfo();
        
        // 如果有任何改变，更新预览
        if (GUI.changed)
        {
            UpdatePreview();
        }
    }
    
    private void DrawBasicSettings()
    {
        EditorGUILayout.LabelField("基础设置", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        diamondPrefab = (GameObject)EditorGUILayout.ObjectField("菱形预制体", diamondPrefab, typeof(GameObject), false);
        radius = EditorGUILayout.FloatField("圆形半径", radius);
        centerPosition = EditorGUILayout.Vector3Field("圆心位置", centerPosition);
        
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePreview();
        }
    }
    
    private void DrawAngleSettings()
    {
        EditorGUILayout.LabelField("角度控制", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        // 角度范围滑块
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("角度范围", GUILayout.Width(60));
        EditorGUILayout.MinMaxSlider(ref startAngle, ref endAngle, 0f, 360f);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"起始: {startAngle:F1}°", GUILayout.Width(80));
        EditorGUILayout.LabelField($"结束: {endAngle:F1}°", GUILayout.Width(80));
        EditorGUILayout.EndHorizontal();
        
        // 间隔角度
        angleInterval = EditorGUILayout.FloatField("间隔角度", angleInterval);
        angleInterval = Mathf.Clamp(angleInterval, 1f, 90f);
        
        // 快速间隔按钮
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("5°", GUILayout.Width(40)))
        {
            angleInterval = 5f;
            UpdatePreview();
        }
        if (GUILayout.Button("10°", GUILayout.Width(40)))
        {
            angleInterval = 10f;
            UpdatePreview();
        }
        if (GUILayout.Button("15°", GUILayout.Width(40)))
        {
            angleInterval = 15f;
            UpdatePreview();
        }
        if (GUILayout.Button("30°", GUILayout.Width(40)))
        {
            angleInterval = 30f;
            UpdatePreview();
        }
        if (GUILayout.Button("45°", GUILayout.Width(40)))
        {
            angleInterval = 45f;
            UpdatePreview();
        }
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePreview();
        }
    }
    
    private void DrawDiamondSettings()
    {
        EditorGUILayout.LabelField("菱形设置", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        faceCenter = EditorGUILayout.Toggle("面向圆心", faceCenter);
        diamondScale = EditorGUILayout.Vector3Field("菱形缩放", diamondScale);
        diamondColor = EditorGUILayout.ColorField("菱形颜色", diamondColor);
        showGizmos = EditorGUILayout.Toggle("显示调试线", showGizmos);
        
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePreview();
        }
    }
    
    private void DrawActionButtons()
    {
        EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成阵列", GUILayout.Height(30)))
        {
            GenerateDiamondArray();
        }
        if (GUILayout.Button("清除预览", GUILayout.Height(30)))
        {
            ClearPreview();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全圆显示", GUILayout.Height(25)))
        {
            startAngle = 0f;
            endAngle = 360f;
            UpdatePreview();
        }
        if (GUILayout.Button("半圆显示", GUILayout.Height(25)))
        {
            startAngle = 0f;
            endAngle = 180f;
            UpdatePreview();
        }
        if (GUILayout.Button("四分之一圆", GUILayout.Height(25)))
        {
            startAngle = 0f;
            endAngle = 90f;
            UpdatePreview();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawInfo()
    {
        EditorGUILayout.LabelField("信息", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"预览菱形数量: {previewDiamonds.Count}");
        EditorGUILayout.LabelField($"显示角度范围: {startAngle:F1}° - {endAngle:F1}°");
        EditorGUILayout.LabelField($"间隔角度: {angleInterval:F1}°");
        
        if (diamondPrefab == null)
        {
            EditorGUILayout.HelpBox("请设置菱形预制体！", MessageType.Warning);
        }
    }
    
    private void UpdatePreview()
    {
        if (diamondPrefab == null) return;
        
        ClearPreview();
        SpawnPreviewDiamonds();
        SceneView.RepaintAll();
    }
    
    private void SpawnPreviewDiamonds()
    {
        // 确保角度范围正确
        float normalizedStartAngle = NormalizeAngle(startAngle);
        float normalizedEndAngle = NormalizeAngle(endAngle);
        
        // 如果结束角度小于起始角度，说明跨越了360度
        if (normalizedEndAngle < normalizedStartAngle)
        {
            normalizedEndAngle += 360f;
        }
        
        // 计算需要生成的菱形数量
        int diamondCount = Mathf.FloorToInt((normalizedEndAngle - normalizedStartAngle) / angleInterval) + 1;
        
        for (int i = 0; i < diamondCount; i++)
        {
            float currentAngle = normalizedStartAngle + (i * angleInterval);
            
            // 如果角度超过360度，取模
            currentAngle = currentAngle % 360f;
            
            // 计算菱形位置
            Vector3 position = CalculateDiamondPosition(currentAngle);
            
            // 生成菱形
            GameObject diamond = Instantiate(diamondPrefab, position, Quaternion.identity);
            diamond.name = $"PreviewDiamond_{i}";
            diamond.hideFlags = HideFlags.HideInHierarchy; // 在Hierarchy中隐藏
            
            // 设置菱形朝向
            if (faceCenter)
            {
                Vector3 directionToCenter = (centerPosition - position).normalized;
                diamond.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToCenter);
            }
            else
            {
                // 让菱形沿着切线方向
                float tangentAngle = currentAngle + 90f;
                diamond.transform.rotation = Quaternion.Euler(0, 0, tangentAngle);
            }
            
            // 设置菱形缩放
            diamond.transform.localScale = diamondScale;
            
            // 设置菱形颜色（如果有SpriteRenderer）
            SpriteRenderer spriteRenderer = diamond.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = diamondColor;
            }
            
            previewDiamonds.Add(diamond);
        }
        
        isPreviewActive = true;
    }
    
    private Vector3 CalculateDiamondPosition(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * radius;
        float y = Mathf.Sin(radians) * radius;
        return centerPosition + new Vector3(x, y, 0);
    }
    
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0f)
        {
            angle += 360f;
        }
        return angle;
    }
    
    private void ClearPreview()
    {
        foreach (GameObject diamond in previewDiamonds)
        {
            if (diamond != null)
            {
                DestroyImmediate(diamond);
            }
        }
        previewDiamonds.Clear();
        isPreviewActive = false;
    }
    
    private void GenerateDiamondArray()
    {
        if (diamondPrefab == null)
        {
            EditorUtility.DisplayDialog("错误", "请先设置菱形预制体！", "确定");
            return;
        }
        
        // 创建一个父物体来管理所有菱形
        GameObject parentObject = new GameObject("DiamondArray");
        parentObject.transform.position = centerPosition;
        
        // 生成实际的菱形
        float normalizedStartAngle = NormalizeAngle(startAngle);
        float normalizedEndAngle = NormalizeAngle(endAngle);
        
        if (normalizedEndAngle < normalizedStartAngle)
        {
            normalizedEndAngle += 360f;
        }
        
        int diamondCount = Mathf.FloorToInt((normalizedEndAngle - normalizedStartAngle) / angleInterval) + 1;
        
        for (int i = 0; i < diamondCount; i++)
        {
            float currentAngle = normalizedStartAngle + (i * angleInterval);
            currentAngle = currentAngle % 360f;
            
            Vector3 position = CalculateDiamondPosition(currentAngle);
            GameObject diamond = Instantiate(diamondPrefab, position, Quaternion.identity, parentObject.transform);
            diamond.name = $"Diamond_{i}";
            
            if (faceCenter)
            {
                Vector3 directionToCenter = (centerPosition - position).normalized;
                diamond.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToCenter);
            }
            else
            {
                float tangentAngle = currentAngle + 90f;
                diamond.transform.rotation = Quaternion.Euler(0, 0, tangentAngle);
            }
            
            diamond.transform.localScale = diamondScale;
            
            SpriteRenderer spriteRenderer = diamond.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = diamondColor;
            }
        }
        
        // 选中生成的父物体
        Selection.activeGameObject = parentObject;
        EditorUtility.DisplayDialog("成功", $"已生成 {diamondCount} 个菱形！", "确定");
    }
    
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!showGizmos) return;
        
        // 绘制圆形
        Handles.color = Color.yellow;
        DrawCircle(centerPosition, radius, 32);
        
        // 绘制角度范围
        Handles.color = Color.red;
        float startRad = startAngle * Mathf.Deg2Rad;
        float endRad = endAngle * Mathf.Deg2Rad;
        
        Vector3 startPos = centerPosition + new Vector3(Mathf.Cos(startRad), Mathf.Sin(startRad), 0) * radius;
        Vector3 endPos = centerPosition + new Vector3(Mathf.Cos(endRad), Mathf.Sin(endRad), 0) * radius;
        
        Handles.DrawLine(centerPosition, startPos);
        Handles.DrawLine(centerPosition, endPos);
        
        // 绘制角度标签
        Handles.color = Color.cyan;
        Handles.Label(startPos + Vector3.up * 0.5f, $"{startAngle:F0}°");
        Handles.Label(endPos + Vector3.up * 0.5f, $"{endAngle:F0}°");
        
        // 绘制半径信息
        Handles.Label(centerPosition + Vector3.right * (radius + 1f), $"R: {radius:F1}");
        
        // 绘制间隔信息
        Vector3 intervalPos = centerPosition + Vector3.up * (radius + 1f);
        Handles.Label(intervalPos, $"间隔: {angleInterval:F1}°");
        
        // 绘制菱形位置点
        Handles.color = Color.green;
        float normalizedStartAngle = NormalizeAngle(startAngle);
        float normalizedEndAngle = NormalizeAngle(endAngle);
        
        if (normalizedEndAngle < normalizedStartAngle)
        {
            normalizedEndAngle += 360f;
        }
        
        int diamondCount = Mathf.FloorToInt((normalizedEndAngle - normalizedStartAngle) / angleInterval) + 1;
        
        for (int i = 0; i < diamondCount; i++)
        {
            float currentAngle = normalizedStartAngle + (i * angleInterval);
            currentAngle = currentAngle % 360f;
            Vector3 position = CalculateDiamondPosition(currentAngle);
            Handles.DrawWireDisc(position, Vector3.forward, 0.1f);
        }
    }
    
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        Vector3 prevPos = center;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Handles.DrawLine(prevPos, newPos);
            prevPos = newPos;
        }
    }
} 