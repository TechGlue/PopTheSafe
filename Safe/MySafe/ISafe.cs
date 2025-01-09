namespace Safe;

public interface ISafe
{
    void Open();
    void Close();
    void PressReset();
    void PressLock();
    void EnterCode(string password);
    string Describe();
}