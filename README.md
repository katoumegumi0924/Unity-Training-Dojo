# 🥋 Unity Training Dojo (Unity 实战训练场)

> 这是一个用于练习 Unity 核心机制、架构设计与算法实现的工程项目。
> 项目包含多个独立的功能模块，每个模块都针对特定的技术难点进行了深入实践。

---

## 📚 目录 (Table of Contents)
1. [RTS 风格移动与命令系统](#1-rts-风格移动与命令系统-command--navigation)
2. [MVC 架构背包系统](#2-mvc-架构背包系统-inventory-system)
3. [开发环境](#-开发环境)

---

## 1. RTS 风格移动与命令系统 (Command & Navigation)

<div align="center">
  <!-- 请确保你的 Docs 文件夹里有这张图，名字要对应 -->
  <img src="./gif/demo_rts.gif" width="600" />
</div>

### 1.1 系统概述 (Overview)
本模块实现了一个类似 MOBA/RTS 游戏的点击移动系统。
集成了 **Unity NavMesh 智能寻路** 与 **自定义命令队列**，支持路径预判与连续指令。同时实现了基于 **观察者模式** 的事件解耦和基于 **对象池** 的高性能特效管理。

### 1.2 核心技术点
*   **算法应用**：使用 `Queue<Vector3>` 实现 **FIFO (先进先出)** 的命令队列，解决连续点击时的路径覆盖问题。
*   **路径可视化**：动态合并 `NavMeshAgent.path.corners` (当前路径) 与 `moveQueue` (未来路径)，使用 `LineRenderer` 绘制完整的预判轨迹。
*   **性能优化**：使用基于 `Stack` 的 **对象池 (Object Pool)** 管理点击特效，利用 CPU 缓存热度 (Cache Locality)，实现 Zero GC。
*   **架构设计**：使用 **观察者模式** 解耦玩家与场景交互，使用 **单例模式** 管理全局计分状态。

### 1.3 关键类设计
*   **`CommandMove`**：核心控制器。引入 `isWalking` 状态位，精确捕捉“到达目的地”的瞬间，解决触发时机错位 Bug。
*   **`FXManager`**：特效管理器。维护 `Stack<GameObject>` 池子，负责特效的借出与回收。

---

## 2. MVC 架构背包系统 (Inventory System)

<div align="center">
  <!-- 请确保你的 Docs 文件夹里有这张图，名字要对应 -->
  <img src="./gif/demo_inventory.gif" width="600" />
</div>

### 2.1 系统概述 (Overview)
本模块实现了一个基于 **MVC 架构** 的库存系统。实现了数据的配置化（ScriptableObject）、逻辑的单例管理以及 UI 的事件驱动更新。

### 2.2 核心架构图 (Architecture)

> **数据流向 (Unidirectional Data Flow)**
>
> `ItemPickup` (玩家触碰) -> `InventoryManager` (修改数据) -> **Event: OnInventoryChanged** (广播) -> `InventoryUI` (刷新界面)

### 2.3 关键类设计 (Class Design)

#### 📄 ItemData (ScriptableObject)
*   **作用**：存储物品的静态配置信息（ID、Icon、Name、Stackable）。
*   **设计理由**：使用 SO 实现了**数据与逻辑分离**。方便策划在编辑器中批量配置数据，且 SO 资源在内存中只存一份，不占用额外的 Runtime 内存。

#### ⚙️ InventoryManager (Model + Controller)
*   **作用**：作为核心管理器，使用 `List<InventorySlot>` 维护运行时库存数据。
*   **核心方法**：`AddItem(data, amount)`。
*   **设计思路**：实现了**智能堆叠**逻辑。当添加物品时，先遍历查找背包中是否存在相同的可堆叠物品；若存在则增加数量，否则创建新格子。

#### 🖼️ InventoryUI (View)
*   **作用**：观察者模式的订阅者，负责渲染界面。
*   **核心机制**：在 `Start` 中订阅事件，在 `OnDestroy` 中注销事件。
*   **设计理由**：采用**事件驱动 (Event-Driven)** 更新。UI 不需要每帧在 `Update` 中轮询数据，只有当数据真正改变时才被动刷新。这不仅实现了逻辑解耦，也最大化了性能。

---

## 🛠️ 开发环境
*   **Engine**: Unity 2021.3 LTS (或你的版本)
*   **Language**: C#
*   **IDE**: Visual Studio 2022
*   **Tools**: Git, ScreenToGif