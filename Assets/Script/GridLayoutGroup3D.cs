#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

/// <summary>
/// 3D网格布局组件 - 用于在Unity编辑器中自动排列子对象的3D网格布局
/// 这个组件只在编辑器中工作，运行时不会执行
/// </summary>
public class GridLayoutGroup3D : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("网格设置")]
    [Tooltip("网格间距 - 每个网格点之间的距离")]
    public Vector3 matrixInterval = new Vector3(0.25f, 0.25f, 0.25f);
    
    [Tooltip("网格大小 - 网格的X、Y、Z轴上的网格点数量")]
    public Vector3Int matrixSize = new Vector3Int(3, 3, 3);
    
    [Tooltip("网格颜色 - 在Scene视图中显示的网格点颜色")]
    public Color matrixColor = new Color(255 / 225f, 225 / 225f, 0, 100 / 255f);
    
    [Tooltip("网格点半径 - 在Scene视图中显示的网格点大小")]
    public float radius = 0.1f;
    
    /// <summary>
    /// 当前处理的子对象索引
    /// </summary>
    private int ChildIndex = 0;
    
    /// <summary>
    /// 存储子对象位置信息的列表 - 用于复制粘贴功能
    /// </summary>
    public List<Location> locations = new List<Location>();

    // ========== 属性访问器 - 当值改变时自动刷新编辑器 ==========
    
    /// <summary>
    /// 网格间距属性 - 设置时会触发编辑器更新
    /// </summary>
    public Vector3 MatrixInterval
    {
        get => matrixInterval; 
        set
        {
            matrixInterval = value;
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate(); // 触发编辑器更新
        }
    }
    
    /// <summary>
    /// 网格大小属性 - 设置时会触发编辑器更新
    /// </summary>
    public Vector3Int MatrixSize
    {
        get => matrixSize; 
        set
        {
            matrixSize = value;
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate(); // 触发编辑器更新
        }
    }

    /// <summary>
    /// 网格点半径属性 - 设置时会触发编辑器更新
    /// </summary>
    public float Radius
    {
        get => radius; 
        set
        {
            radius = value;
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate(); // 触发编辑器更新
        }
    }
    
    /// <summary>
    /// 网格颜色属性 - 设置时会触发编辑器更新
    /// </summary>
    public Color MatrixColor
    {
        get => matrixColor; 
        set
        {
            matrixColor = value;
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate(); // 触发编辑器更新
        }
    }
    
    /// <summary>
    /// 当对象被选中时在Scene视图中绘制网格
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        DrawAndSetLocation();
    }

    /// <summary>
    /// 核心方法：绘制网格并设置子对象位置
    /// 这个方法会根据网格设置自动排列所有子对象
    /// </summary>
    public void DrawAndSetLocation()
    {
        ChildIndex = 0; // 重置子对象索引
        
        // 获取网格的绝对大小（确保为正数）
        int MatrixSizeX = Mathf.Abs(MatrixSize.x);
        int MatrixSizeY = Mathf.Abs(MatrixSize.y);
        int MatrixSizeZ = Mathf.Abs(MatrixSize.z);

        // 三层循环遍历3D网格的每个位置
        for (int j = 0; j < MatrixSizeY; j++)      // Y轴（高度）
        {
            for (int z = 0; z < MatrixSizeZ; z++)  // Z轴（深度）
            {
                for (int i = 0; i < MatrixSizeX; i++) // X轴（宽度）
                {
                    // 计算当前网格点的逻辑坐标
                    // 根据MatrixSize的正负值决定网格方向
                    Vector3 CurLoc = new Vector3(
                        MatrixSize.x > 0 ? i : -i,    // X轴方向
                        MatrixSize.y > 0 ? j : -j,    // Y轴方向  
                        MatrixSize.z > 0 ? -z : z     // Z轴方向
                        );
                    
                    // 将逻辑坐标转换为实际的世界坐标
                    Vector3 Location = new Vector3(
                        CurLoc.x * MatrixInterval.x,  // X轴位置
                        CurLoc.y * MatrixInterval.y,  // Y轴位置
                        CurLoc.z * MatrixInterval.z   // Z轴位置
                        );
                    
                    // 如果还有子对象需要排列
                    if (ChildIndex < transform.childCount)
                    {
                        // 如果子对象位置与计算的位置不同，则更新位置
                        if (transform.GetChild(ChildIndex).localPosition != Location)
                        {
                            transform.GetChild(ChildIndex).localPosition = Location;
                            // 标记场景为已修改状态
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                    
                    // 在Scene视图中绘制网格点
                    Gizmos.color = MatrixColor;                    // 设置网格点颜色
                    Gizmos.matrix = transform.localToWorldMatrix;  // 设置变换矩阵
                    Gizmos.DrawCube(Location, Radius * Vector3.one); // 绘制立方体网格点
                    
                    ChildIndex++; // 移动到下一个子对象
                }
            }
        }
    }
    
    /// <summary>
    /// 复制功能：保存当前所有子对象的位置和旋转信息
    /// </summary>
    public void CopyInfo()
    {
        if (locations.Count == 0) // 只在列表为空时执行
        {
            // 遍历所有子对象，保存它们的信息
            foreach (var item in GetComponentsInChildren<Transform>())
            {
                locations.Add(new Location(item.name, item.position, item.rotation));
            }
        }
    }
    
    /// <summary>
    /// 粘贴功能：将保存的位置信息应用到当前子对象
    /// </summary>
    /// <returns>操作结果信息</returns>
    public string PasteInfo()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        
        // 检查子对象数量是否匹配
        if (transforms.Length != locations.Count)
        {
            return "子对象数量不相等，停止赋值！";
        }
        
        // 遍历所有子对象，恢复它们的位置和旋转
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == locations[i].name) // 检查名称是否匹配
            {
                transforms[i].position = locations[i].position;   // 恢复位置
                transforms[i].rotation = locations[i].rotation;   // 恢复旋转
            }
            else
            {
                return "对象" + locations[i].name + "有变动，停止赋值！";
            }
        }
        return "赋值成功！";
    }
    
    /// <summary>
    /// 位置信息数据类 - 用于存储对象的位置、旋转和名称
    /// </summary>
    public class Location
    {
        public string name;           // 对象名称
        public Vector3 position;      // 世界坐标位置
        public Quaternion rotation;   // 旋转信息

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="position">世界坐标位置</param>
        /// <param name="rotation">旋转信息</param>
        public Location(string name, Vector3 position, Quaternion rotation)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
        }
    }
#endif
}