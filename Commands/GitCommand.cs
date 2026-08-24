using System.CommandLine;
using System.Text;
using System.Text.Json;
using WTangent.GitCmd.Store;

namespace WTangent.GitCmd.Commands;


/// <summary>wtangent git：双模式。
/// 本地模式（默认）：init/clone 是 wtangent 包装（.agent 清单 + 服务器解析），其余参数完全透传给真 git（在项目目录执行）。
/// 远程模式（--server）：客户端触发服务端执行——调 serve 的 /git-exec，在服务端项目目录跑 git。
/// 透传示例：wtangent git push / add -A / commit -m "..." / pull / branch / status / checkout —— 等价于在该目录跑 git。</summary>
[AgentCommand]
public sealed class GitCommand : Command
{
    private static readonly Option<string?> DirOption = new("--dir") { Description = "项目目录（缺省当前目录；本地透传时 git 在此执行）" };
    private static readonly Option<string?> ServerOption = new("--server") { Description = "远程模式：在指定服务器（remotes.json）上执行 git" };
    private static readonly Option<string?> ProjectOption = new("--project") { Description = "远程模式的项目名（缺省读当前目录 .agent；没有则必填）" };
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(60) };

    public GitCommand() : base("git", "git 透传（init/clone 为 wtangent 包装，其余参数直跑真 git；--server 远程执行）")
    {
        TreatUnmatchedTokensAsErrors = false;
        Add(DirOption);
        Add(ServerOption);
        Add(ProjectOption);
        Add(BuildInitCommand());
        Add(BuildCloneCommand());

        // 其余任意参数（含 git remote/add/commit/push/pull/branch/status…）→ 本地透传或远程执行
        SetAction(pr =>
        {
            var args = pr.UnmatchedTokens.ToArray();
            var server = pr.GetValue(ServerOption);
            return server is not null
                ? RemoteRun(server, pr.GetValue(ProjectOption), args)
                : new GitStore(pr.GetValue(DirOption) ?? ".").RunGit(args);
        });
    }

    /// <summary>远程模式：POST {server}/git-exec {project, args} → 服务端项目目录跑 git，打印输出返回退出码</summary>
    private static int RemoteRun(string server, string? project, string[] args)
    {
        var hit = new ServerRegistry().Find(server);
        if (hit is null)
        {
            Console.Error.WriteLine($"[git --server] 服务器 {server} 未配置，先 wtangent remote add {server} <ip> <port>");
            return 1;
        }
        project ??= GitStore.ManifestName(".");
        if (project is null)
        {
            Console.Error.WriteLine("[git --server] 未指定项目：--project <name>，或先 wtangent git init（生成 .agent）");
            return 1;
        }
        try
        {
            var body = JsonSerializer.Serialize(new { project, args });
            using var resp = Http.PostAsync($"{hit.Url.TrimEnd('/')}/git-exec",
                new StringContent(body, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
            var text = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!resp.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"[git --server] HTTP {(int)resp.StatusCode}：{text.Trim()}");
                return 1;
            }
            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;
            var output = root.GetProperty("output").GetString() ?? "";
            if (output.Length > 0) Console.Write(output);
            return root.GetProperty("exit_code").GetInt32();
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[git --server] 调用 {hit.Url} 失败：{e.Message}");
            return 1;
        }
    }

    /// <summary>本地项目：git init -b main + 身份 + .agent 清单（之后 add/commit/push 全是透传 git）。</summary>
    private static Command BuildInitCommand()
    {
        var initDir = new Argument<string?>("dir") { Arity = ArgumentArity.ZeroOrOne, Description = "目标目录（缺省当前目录）" };
        var initName = new Option<string?>("--name") { Description = "项目名（缺省目录名）" };
        var init = new Command("init", "新建本地项目（git init + .agent 清单）") { initDir, initName };
        init.SetAction(pr =>
        {
            var dir = pr.GetValue(initDir) ?? ".";
            var name = pr.GetValue(initName) ?? new DirectoryInfo(dir).Name;
            GitStore.Init(dir, name);
            Console.WriteLine($"[git init] 项目 {name} 就绪：{Path.GetFullPath(dir)}");
            return 0;
        });
        return init;
    }

    /// <summary>从服务器克隆：按名查服务器 URL → git clone + .agent 清单。</summary>
    private static Command BuildCloneCommand()
    {
        var serverArg = new Argument<string>("server");
        var projectArg = new Argument<string>("project");
        var cloneDir = new Argument<string?>("dir") { Arity = ArgumentArity.ZeroOrOne, Description = "目标目录（缺省项目名）" };
        var clone = new Command("clone", "从服务器克隆项目（服务器名 / ET 加入码）") { serverArg, projectArg, cloneDir };
        clone.SetAction(async pr =>
        {
            var server = pr.GetValue(serverArg);
            var project = pr.GetValue(projectArg);
            if (server is null || project is null) return 1;
            var hit = new ServerRegistry().Find(server);
            if (hit is null)
            {
                await Console.Error.WriteLineAsync($"[git clone] 服务器 {server} 未配置，先 wtangent remote add {server} <ip> <port>");
                return 1;
            }
            var dir = pr.GetValue(cloneDir) ?? project;
            GitStore.Clone(server, hit.Url, project, dir);
            Console.WriteLine($"[git clone] {server}:{project} → {Path.GetFullPath(dir)}");
            return 0;
        });
        return clone;
    }
}
