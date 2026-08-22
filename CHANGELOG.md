# Changelog

## [0.0.2](https://github.com/WTangent-Org/WTangent.GitCmd/compare/v0.0.1...v0.0.2) (2026-08-22)


### Features

* git 双模式——本地透传 + --server 远程执行（serve 新增 /git-exec 端点） ([982a624](https://github.com/WTangent-Org/WTangent.GitCmd/commit/982a62488ca45fc98de93b2fd11f496f8b42a85f))
* git 透传命令组件（cmd 类型）——init/clone 包装 + 全参数透传真 git ([f24b09e](https://github.com/WTangent-Org/WTangent.GitCmd/commit/f24b09eefef1b3787d0d95b6c0419b48a0a5e4f8))
* IEntry 元组命令（父路径挂接）+ 三形态（cmd/sub/tool）+ 类型字段废弃 ([5e56cb1](https://github.com/WTangent-Org/WTangent.GitCmd/commit/5e56cb18fc670c30b6379a10dbedc70916ac9178))
* IEntry 手写入口（0.0.3）——类型字段废弃，能力由 Entry 声明（Commands/Default/Tools + StartAsync 生命周期） ([ded7711](https://github.com/WTangent-Org/WTangent.GitCmd/commit/ded7711ce7569bfcc2feb6f85618336c8d62b0a6))
