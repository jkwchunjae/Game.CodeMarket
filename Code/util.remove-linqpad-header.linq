<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows</Namespace>
</Query>

using System.Runtime.InteropServices;
using System.Windows.Threading;

// 클립보드를 1초마다 확인해 맨 위의 LINQPad XML 헤더(<Query>), #load 지시문, 그 주변의 빈 줄만 제거한다.
async Task Main()
{
    const string blankLine = @"[ \t]*(?:\r\n|\n|\r)";
    const string loadLine = """[ \t]*#load[ \t]+"[^"\r\n]*"[^\r\n]*(?:\r\n|\n|\r|\z)""";
    const string queryStart = """<Query(?:[ \t\r\n]+(?:[^<>/"']|"[^"]*"|'[^']*')*)?""";
    const string queryHeader = $"""[ \t]*(?<query>{queryStart}(?:/>|>[\s\S]*?</Query[ \t\r\n]*>))[ \t]*(?:\r\n|\n|\r|\z)""";
    var header = new Regex($@"\A\uFEFF?(?:{blankLine})*(?:{queryHeader}|{loadLine})(?:{blankLine}|{loadLine})*",
        RegexOptions.NonBacktracking);

    // await 이후에도 클립보드 접근은 같은 STA 스레드에서 실행한다.
    var ready = new TaskCompletionSource<Dispatcher>(TaskCreationOptions.RunContinuationsAsynchronously);
    var thread = new Thread(() =>
    {
        ready.SetResult(Dispatcher.CurrentDispatcher);
        Dispatcher.Run();
    }) { IsBackground = true };

    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();

    var dispatcher = await ready.Task;
    try
    {
        while (true)
        {
            await dispatcher.InvokeAsync(() => CleanClipboard(header)).Task;
            await Task.Delay(1000);
        }
    }
    finally
    {
        dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
    }
}

void CleanClipboard(Regex header)
{
    try
    {
        var text = Clipboard.GetText();
        var match = header.Match(text);
        if (!match.Success)
            return;

        if (match.Groups["query"] is { Success: true } query)
            XElement.Parse(query.Value);

        var remaining = text[match.Length..];
        if (string.IsNullOrWhiteSpace(remaining))
            Clipboard.Clear();
        else
            Clipboard.SetText(remaining);
    }
    catch (ExternalException)
    {
        // 클립보드가 사용 중이면 다음 주기에 다시 시도한다.
    }
    catch (XmlException)
    {
        // 올바른 XML 헤더가 아니면 클립보드를 그대로 둔다.
    }
}
