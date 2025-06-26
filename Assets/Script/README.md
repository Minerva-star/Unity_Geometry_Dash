# Geometry Dash 游戏脚本说明

## 脚本概述

这个项目包含了一个类似Geometry Dash的2D跑酷游戏的完整脚本系统。

## 主要脚本文件

### 1. PlayerBehavior.cs
**功能**: 玩家控制脚本
**主要特性**:
- 重力系统
- 跳跃控制（空格键或鼠标左键）
- 长按持续跳跃
- 屏幕位置限制（保持在左1/3处）
- 碰撞检测
- 暂停/恢复功能

**设置参数**:
- `jumpForce`: 跳跃力度
- `horizontalSpeed`: 水平移动速度
- `screenPositionX`: 屏幕位置（0.33 = 左1/3）
- `particleEffectPrefab`: 碰撞粒子特效预制体

### 2. GameManager.cs
**功能**: 游戏主管理器
**主要特性**:
- 游戏状态管理（Playing/Paused/GameOver）
- 背景和障碍物移动控制
- 进度计算和显示
- 障碍物生成
- 粒子特效管理
- 暂停/重新开始功能

**设置参数**:
- `player`: 玩家对象引用
- `backgroundSpeed`: 背景移动速度
- `obstacleSpeed`: 障碍物移动速度
- `totalMapLength`: 总地图长度
- `obstacleSpawnInterval`: 障碍物生成间隔

### 3. UIManager.cs
**功能**: UI界面管理
**主要特性**:
- 主游戏UI
- 暂停界面
- 游戏结束界面
- 进度显示
- 按钮事件处理

### 4. BackgroundMover.cs
**功能**: 背景移动控制
**主要特性**:
- 循环背景移动
- 自动重置位置
- 暂停/恢复功能

### 5. Obstacle.cs
**功能**: 障碍物行为控制
**主要特性**:
- 自动移动
- 碰撞检测
- 自动销毁
- 暂停/恢复功能

### 6. ParticleManager.cs
**功能**: 粒子特效管理
**主要特性**:
- 粒子对象池
- 碰撞特效播放
- 自动回收

## 设置步骤

### 1. 场景设置
1. 创建玩家对象，添加 `PlayerBehavior` 脚本
2. 创建GameManager对象，添加 `GameManager` 脚本
3. 创建UI Canvas，添加 `UIManager` 脚本
4. 创建背景对象，添加 `BackgroundMover` 脚本
5. 创建障碍物预制体，添加 `Obstacle` 脚本
6. 创建粒子管理器对象，添加 `ParticleManager` 脚本

### 2. 组件配置
**玩家对象**:
- Rigidbody2D (重力启用)
- Collider2D (触发器)
- PlayerBehavior脚本

**背景对象**:
- Sprite Renderer
- BackgroundMover脚本

**障碍物预制体**:
- Sprite Renderer
- BoxCollider2D (isTrigger = true)
- Obstacle脚本

### 3. 标签设置
- 障碍物对象设置为 "Obstacle" 标签

### 4. 层级设置
- 创建 "Obstacle" 层级用于障碍物

## 游戏逻辑流程

1. **游戏开始**: 玩家受重力影响向下移动
2. **玩家控制**: 按空格键或鼠标左键给玩家向上的力
3. **位置限制**: 玩家始终保持在屏幕左1/3位置
4. **背景移动**: 背景向左移动，营造前进感
5. **障碍物生成**: 定期在屏幕右侧生成障碍物
6. **碰撞检测**: 玩家碰到障碍物时触发游戏结束
7. **特效播放**: 碰撞时播放粒子特效
8. **UI显示**: 显示进度和游戏结束界面
9. **暂停功能**: ESC键或UI按钮暂停游戏
10. **重新开始**: 重置所有游戏状态

## 控制说明

- **空格键/鼠标左键**: 跳跃（长按持续跳跃）
- **ESC键**: 暂停/恢复游戏
- **UI按钮**: 暂停、恢复、重新开始、退出

## 注意事项

1. 确保所有脚本的引用正确设置
2. 障碍物预制体必须设置正确的标签
3. 粒子特效预制体需要包含ParticleSystem组件
4. UI Canvas需要正确设置Canvas Scaler
5. 相机设置建议使用Orthographic模式

## 扩展建议

1. 添加音效系统
2. 实现关卡系统
3. 添加分数系统
4. 实现成就系统
5. 添加更多障碍物类型
6. 实现背景音乐
7. 添加视觉效果（后处理）
8. 实现存档系统 