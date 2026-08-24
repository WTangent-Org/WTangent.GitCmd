using WTangent.Core;

namespace WTangent.GitCmd;

/// <summary>git 组件入口（[AgentEntry] 元数据；命令由生成器收集）。</summary>
[AgentEntry("git", "git 命令", false)]
public sealed partial class Entry : IEntry
{
}
