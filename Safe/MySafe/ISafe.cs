namespace Safe;

public interface ISafe
{
    void Open();
    void Close();
    void PressReset();
    void PressLock();
    void SetCode(string password);
    string Describe();
}