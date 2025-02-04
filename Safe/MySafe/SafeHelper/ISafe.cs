namespace MySafe.SafeHelper;

public interface ISafe
{
    SafeResponse Open();
    SafeResponse Close();
    SafeResponse PressReset();
    SafeResponse PressLock();
    SafeResponse SetCode(string password, Action<SafeResponse> resultsHandler);
    string Describe();
}