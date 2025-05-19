namespace MySafe.SafeHelper;

public record SafeResponse
{
    public bool IsSuccessful { get; init; } = false;
    public string IsDetail { get; init; } = "";
    
    public int StateId { get; set; } = 9999; 

    public static SafeResponse Ok( string? info = null, int stateId = -1)
    {
        return new SafeResponse { IsSuccessful = true, IsDetail = info, StateId = stateId};
    }

    public static SafeResponse Fail(string? info = null)
    {
        return new SafeResponse { IsSuccessful = false, IsDetail = info };
    }
}