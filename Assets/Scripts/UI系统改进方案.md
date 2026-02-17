UI Canvas 分层改进方案（ARPG 项目）
1. 背景与目标
当前 UISystem 采用单 Canvas + 层级节点（Bottom/Middle/Top/System）管理所有面板。该结构功能完整，但在高频 UI 更新（HUD、血条、交互提示、滚动列表）场景下，容易引发较大范围的 Canvas 重建开销。.

本方案目标：

引入 动静分离（多 Canvas）

降低高频 UI 对全局重建的影响

保持现有 IUISystem 调用方式，渐进迁移

保证弹窗层级与交互一致性

2. 现状评估（摘要）
IUISystem 已有统一入口：ShowPanel/HidePanel/GetPanel/GetLayerFather。.

UISystem 运行时动态创建 UICamera、Canvas、EventSystem，并维护四层节点。.

面板通过 ShowPanel<T> 异步加载并挂到指定层。.

3. Canvas 分层设计（建议）
建议建立 3 类 Canvas：

StaticCanvas（静态）

基本不变化内容：背景框架、静态装饰、主菜单静态元素

CommonCanvas（中频交互）

常规面板交互：背包、装备、任务、设置、存档

DynamicCanvas（高频动态）

高频变化元素：HUD（HP/精力/法力/毒条）、Boss 血条、交互提示等

说明：每个 Canvas 内仍可保留 Bottom/Middle/Top/System 子层级，便于复用你现有层级路由逻辑。

4. 面板分配建议表
面板	建议 Canvas	依据
GamePanel	DynamicCanvas	高密度属性与快捷槽响应式刷新。.
InteractionPanel	DynamicCanvas	Boss HUD、交互提示、选择窗口频繁变化。.
BagPanel	CommonCanvas	滚动列表检查/刷新，属于中频交互。.
EquipPanel	CommonCanvas	打开时批量槽位生成和点击绑定。.
TaskPanel	CommonCanvas	打开时构建任务/需求/奖励列表。.
SavePanel	CommonCanvas	打开时刷新槽位数据。.
SettingPanel	CommonCanvas	滑条与开关交互，中频变化。.
BeginPanel	StaticCanvas	主菜单主体变化低频。.
TipPanel	Top Overlay（建议 CommonCanvas.Top 或独立 Overlay）	弹窗必须全局最高层。.
5. 预制体组织建议
推荐方案：一个 UIRoot 预制体，内部包含三个 Canvas
结构示例：

UIRoot
├── UICamera
├── EventSystem
├── StaticCanvas
│   ├── Bottom
│   ├── Middle
│   ├── Top
│   └── System
├── CommonCanvas
│   ├── Bottom
│   ├── Middle
│   ├── Top
│   └── System
└── DynamicCanvas
    ├── Bottom
    ├── Middle
    ├── Top
    └── System
选择该方案的原因
初始化与生命周期统一（贴合当前 UISystem.OnInit 的一次性创建逻辑）。.

层级与排序关系更稳定，维护成本低。

可渐进迁移，不需要一次性改完所有面板。

6. 路由策略（架构嵌入方式）
在不破坏现有 API 的前提下，建议在 UISystem 内新增“面板配置路由”：

输入：PanelType

输出：CanvasCategory + E_UILayer + ResourceKey

例如：

GamePanel -> DynamicCanvas + Middle + "ui/GamePanel"

TipPanel -> CommonCanvas + Top + "ui/TipPanel"

这样外部依旧调用 ShowPanel<T>()，内部自动选择 Canvas 父节点。.

7. 渐进实施步骤（建议）
Phase 1（低风险）
搭建 UIRoot(3 Canvas) 预制体（保留现有层级命名）

UISystem 增加 Canvas 分类父节点缓存

先迁移 GamePanel、InteractionPanel 到 DynamicCanvas

Phase 2（中风险）
迁移 BagPanel/EquipPanel/TaskPanel/SettingPanel/SavePanel 到 CommonCanvas

迁移 BeginPanel 到 StaticCanvas

明确 TipPanel 全局最高层规则

Phase 3（优化）
面板配置表化（Panel -> Canvas/Layer/Path）

做 UI 性能对比（重建次数、CPU 时间、批次）

再决定是否需要更细粒度拆分

8. 风险与注意事项
Canvas 不是越多越好：过度拆分会增加批次和管理复杂度。

排序规则需统一：先定义三 Canvas 的 sorting order 与 Top 层优先级。

迁移过程避免全量改动：先迁高频 UI，再迁普通面板。

9. 验收标准（建议）
高频战斗场景下，HUD 更新对菜单类 UI 的影响明显下降

打开/关闭背包、装备、任务时不影响常驻 HUD 稳定性

弹窗（Tip）在任意场景都能正确置顶

不改变现有业务层调用方式（ShowPanel/HidePanel/GetPanel）