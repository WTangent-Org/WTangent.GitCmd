using System.Text.Json;
using WTangent.Core;

namespace WTangent.GitCmd.Store;

/// <summary>服务器条目（remotes.json 记录）</summary>
public sealed record RemoteEntry(string Name, string Host, int Port, string? EtCode, string Kind = "lan")
{
    public string Url => $"http://{Host}:{Port}";
}

/// <summary>服务器注册表读取（remotes.json，由客户端维护）：按名/加入码查 URL。
/// 优先走宿主注入的 Entry.App.Store，未注入时回退直接文件访问。</summary>
public sealed class ServerRegistry
{
    private static string StorePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "agent", "remotes.json");

    private static IAppStore? AppStore => Entry.Current?.App?.Store;

    public string? Get(string name) => Load().FirstOrDefault(r => r.Name == name)?.Url;

    /// <summary>按 ET 加入码（优先）或名字查找；找不到返回 null</summary>
    public RemoteEntry? Find(string codeOrName) =>
        Load().FirstOrDefault(r => r.EtCode is { Length: > 0 } && r.EtCode == codeOrName)
        ?? Load().FirstOrDefault(r => r.Name == codeOrName);

    private List<RemoteEntry> Load()
    {
        if (AppStore is not null)
        {
            var viaStore = AppStore.ReadJson<List<RemoteEntry>>("remotes.json");
            if (viaStore is not null) return viaStore;
        }
        if (!File.Exists(StorePath)) return [];
        try
        {
            var json = File.ReadAllText(StorePath);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                return doc.RootElement.EnumerateObject()
                    .Select(p => EntryFromUrl(p.Name, p.Value.GetString() ?? ""))
                    .ToList();
            }
            if (json.Contains("\"Code\":", StringComparison.Ordinal) && !json.Contains("\"EtCode\":", StringComparison.Ordinal))
                json = json.Replace("\"Code\":", "\"EtCode\":", StringComparison.Ordinal);
            if (!json.Contains("\"Url\":", StringComparison.Ordinal) || json.Contains("\"Host\":", StringComparison.Ordinal))
                return JsonSerializer.Deserialize<List<RemoteEntry>>(json) ?? [];
            var old = JsonSerializer.Deserialize<List<OldRemote>>(json) ?? [];
            return old.Select(o => EntryFromUrl(o.Name, o.Url, o.EtCode, o.Kind)).ToList();
        }
        catch { return []; }
    }

    private static RemoteEntry EntryFromUrl(string name, string url, string? code = null, string kind = "lan")
    {
        var u = Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri : new Uri("http://" + url);
        return new RemoteEntry(name, u.Host, u.Port > 0 ? u.Port : 8890, code, kind);
    }

    private sealed record OldRemote(string Name, string Url, string? EtCode, string Kind);
}
