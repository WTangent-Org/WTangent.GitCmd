# Changelog

## [0.0.3](https://github.com/WTangent-Org/WTangent.GitCmd/compare/v0.0.2...v0.0.3) (2026-08-29)


### Features

* App 静态属性构造注入（PCL-CE 式）：生成器产 static App + ctor，IEntry 移除 App 成员 ([1ddf326](https://github.com/WTangent-Org/WTangent.GitCmd/commit/1ddf32683e390a38d1c44920d466785ccac75add))
* 最终特性集 [AgentEntry(id,name,isAsync)]/[EntryStart]/[EntryStop]/[AgentCommand(parent)]/[AgentTool] ([6c3b177](https://github.com/WTangent-Org/WTangent.GitCmd/commit/6c3b1779cba7effb0d72fdb315e51e607b85b6d9))
* 构造注入 App（无 null!）+ Current 静态桥（PCL-CE 式）；钩子实例方法，纯业务 ([c2c876a](https://github.com/WTangent-Org/WTangent.GitCmd/commit/c2c876ab54ba52838648a6197bd230ea2d7347f4))


### Bug Fixes

* CI 布局——本仓 checkout 进同名子目录复刻本地工作区布局（ProjectReference 的 ../ 不再越出工作区），构建路径加前缀 ([1ef73f7](https://github.com/WTangent-Org/WTangent.GitCmd/commit/1ef73f75396b46450dff2150f139c8638a38f681))

## [0.0.2](https://github.com/WTangent-Org/WTangent.GitCmd/compare/v0.0.1...v0.0.2) (2026-08-22)


### Features

* git 双模式——本地透传 + --server 远程执行（serve 新增 /git-exec 端点） ([982a624](https://github.com/WTangent-Org/WTangent.GitCmd/commit/982a62488ca45fc98de93b2fd11f496f8b42a85f))
* git 透传命令组件（cmd 类型）——init/clone 包装 + 全参数透传真 git ([f24b09e](https://github.com/WTangent-Org/WTangent.GitCmd/commit/f24b09eefef1b3787d0d95b6c0419b48a0a5e4f8))
* IEntry 元组命令（父路径挂接）+ 三形态（cmd/sub/tool）+ 类型字段废弃 ([5e56cb1](https://github.com/WTangent-Org/WTangent.GitCmd/commit/5e56cb18fc670c30b6379a10dbedc70916ac9178))
* IEntry 手写入口（0.0.3）——类型字段废弃，能力由 Entry 声明（Commands/Default/Tools + StartAsync 生命周期） ([ded7711](https://github.com/WTangent-Org/WTangent.GitCmd/commit/ded7711ce7569bfcc2feb6f85618336c8d62b0a6))
