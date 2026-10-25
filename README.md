# Transformation-Unity-Deform
> Computer Graphics Mesh Deformation Experiment on Unity

## 📖 项目简介
本项目是计算机图形学课程实验，基于 Unity 引擎实现网格顶点形变。
通过 C# 脚本读取、修改 Mesh 顶点数据，结合仿射变换、贝塞尔曲线等算法，对三维模型进行程序化形变，直观验证图形学几何变换原理。
项目全部代码为自研实验代码，用于课程学习与技术验证。

## 🎯 实验目标
1. 理解三维网格 Mesh 的顶点、三角面基础结构
2. 掌握顶点在 CPU 端的读取、修改、刷新流程
3. 实现平移、旋转、缩放等基础仿射变换
4. 使用 Bezier 曲线驱动模型顶点，实现平滑可控形变效果
5. 对比 CPU 顶点修改与 Shader 内顶点变换两种实现思路

## 🛠 开发环境
- 引擎：Unity 2022.3 LTS
- 渲染管线：Built-in Render Pipeline
- 开发语言：C#、ShaderLab
- 依赖：无外部第三方包

## 📁 仓库目录说明
Assets
├── Scenes/          # 测试场景文件
├── Scripts/         # 形变核心控制脚本
├── Shaders/         # 自定义顶点变换 Shader
├── Prefabs/         # 测试模型预制体
├── Materials/       # 材质资源
└── Textures/        # 贴图资源

## 🚀 快速运行
1. 克隆仓库
```bash
git clone https://github.com/xiaomazi27/Transformation-Unity-Deform.git
