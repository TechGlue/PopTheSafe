namespace Safe;

public record SafeResponse
{
    public bool isSuccessful { get; init; } = false;
    public string isDetail { get; init; } = "";

    public static SafeResponse Ok(string? info = null)
    {
        return new SafeResponse() { isSuccessful = true, isDetail = info };
    }

    public static SafeResponse Fail(string? info = null)
    {
        return new SafeResponse() { isSuccessful = false, isDetail = info };
    }
}