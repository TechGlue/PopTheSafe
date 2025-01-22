namespace Safe;

public record SafeResponse
{
    public bool IsSuccessful { get; init; } = false;
    public string IsDetail { get; init; } = "";

    public static SafeResponse Ok(string? info = null)
    {
        return new SafeResponse() { IsSuccessful = true, IsDetail = info };
    }

    public static SafeResponse Fail(string? info = null)
    {
        return new SafeResponse() { IsSuccessful = false, IsDetail = info };
    }
}