namespace CursorTestApp.Models.Tab3;

public sealed class PlcCommChannelDto
{
    public required string ChannelRole { get; init; }
    public required string PortName { get; init; }
    public int BaudRate { get; init; }
    public int ByteSize { get; init; }
    public required string Parity { get; init; }
    public int StopBits { get; init; }
}
