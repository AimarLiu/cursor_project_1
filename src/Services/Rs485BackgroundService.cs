using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace CursorTestApp.Services;

/// <summary>
/// 背景讀取 RS485（COM1: 9600,8,N,1）並更新 <see cref="Rs485Service"/>。
/// 若 COM1 無法開啟則靜默結束，UI 仍可由 F1 模擬更新。
/// </summary>
public sealed class Rs485BackgroundService
{
    private const string DefaultPort = "COM1";
    private const int BaudRate = 9600;
    private const int ReadIntervalMs = 200;

    private readonly Rs485Service _rs485Service;
    private readonly Dispatcher _dispatcher;
    private volatile bool _running;
    private Thread? _thread;
    private SerialPort? _port;

    public Rs485BackgroundService(Rs485Service rs485Service)
    {
        _rs485Service = rs485Service ?? throw new ArgumentNullException(nameof(rs485Service));
        _dispatcher = Application.Current?.Dispatcher ?? throw new InvalidOperationException("No WPF Dispatcher.");
    }

    /// <summary>啟動背景讀取。若 COM 無法開啟則不更新，不拋錯。</summary>
    public void Start()
    {
        if (_running) return;
        _running = true;
        _thread = new Thread(RunReadLoop) { IsBackground = true };
        _thread.Start();
    }

    /// <summary>停止背景讀取並釋放 COM 埠。</summary>
    public void Stop()
    {
        _running = false;
        try
        {
            _port?.Close();
            _port?.Dispose();
        }
        catch
        {
            // 忽略關閉錯誤
        }
        _port = null;
    }

    private void RunReadLoop()
    {
        try
        {
            if (!TryOpenPort())
                return;

            var buffer = new StringBuilder();
            var buf = new byte[256];
            while (_running && _port?.IsOpen == true)
            {
                try
                {
                    int count = _port.Read(buf, 0, buf.Length);
                    if (count <= 0) continue;
                    string chunk = Encoding.ASCII.GetString(buf, 0, count);
                    buffer.Append(chunk);
                    var str = buffer.ToString();
                    int idx = str.IndexOfAny(new[] { '\r', '\n' });
                    while (idx >= 0)
                    {
                        var line = str.Substring(0, idx).Trim();
                        int take = idx + 1;
                        if (idx + 1 < str.Length && str[idx] == '\r' && str[idx + 1] == '\n')
                            take = idx + 2;
                        buffer.Remove(0, take);
                        if (line.Length > 0)
                            ParseAndUpdate(line);
                        str = buffer.ToString();
                        idx = str.IndexOfAny(new[] { '\r', '\n' });
                    }
                }
                catch (TimeoutException)
                {
                    // 正常，無資料
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (IOException)
                {
                    break;
                }

                Thread.Sleep(ReadIntervalMs);
            }
        }
        catch
        {
            // COM 無法使用時靜默結束
        }
        finally
        {
            try { _port?.Close(); _port?.Dispose(); } catch { }
            _port = null;
        }
    }

    private bool TryOpenPort()
    {
        try
        {
            var names = SerialPort.GetPortNames();
            string portName = names.Contains(DefaultPort) ? DefaultPort : (names.Length > 0 ? names[0] : DefaultPort);
            _port = new SerialPort(portName, BaudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _port.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void ParseAndUpdate(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;
        var upper = line.Trim().ToUpperInvariant();

        bool? opt = null;
        bool? plc = null;
        int? speed = null;
        bool? error = null;

        if (upper.Contains("ACTIVE+")) opt = true;
        else if (upper.Contains("ACTIVE-")) opt = false;
        if (upper.Contains("PLC+")) plc = true;
        else if (upper.Contains("PLC-")) plc = false;
        if (upper.StartsWith("RPM:", StringComparison.OrdinalIgnoreCase) && int.TryParse(upper.AsSpan(4).Trim(), out var s))
            speed = s;
        if (upper.Contains("ERROR+")) error = true;
        else if (upper.Contains("ERROR-")) error = false;

        _dispatcher.InvokeAsync(() =>
        {
            if (opt.HasValue) _rs485Service.IsOptActive = opt.Value;
            if (plc.HasValue) _rs485Service.IsPlcActive = plc.Value;
            if (speed.HasValue) _rs485Service.Speed = speed.Value;
            if (error.HasValue) _rs485Service.HasError = error.Value;
        }, DispatcherPriority.Normal);
    }
}
