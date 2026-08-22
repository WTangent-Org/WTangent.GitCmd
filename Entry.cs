using WTangent.Core;

namespace WTangent.GitCmd;

/// <summary>git 组件入口（[AgentEntry] 元数据 + 生命周期钩子；命令由生成器收集）。</summary>
[AgentEntry("git", "git 命令", false)]
public sealed partial class Entry : IEntry
{
    /// <summary>宿主运行时上下文（StartAsync 注入；组件内部静态访问）</summary>
    public static Application? App { get; private set; }

    [EntryStart]
    private static void OnStart(Application app) => App = app;

    [EntryStop]
    private static void OnStop() => App = null;
}
