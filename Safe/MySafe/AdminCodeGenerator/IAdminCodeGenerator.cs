namespace MySafe.AdminCodeGenerator;

public interface IAdminCodeGenerator
{
    public (int lowerLimit, int upperLimits) CalcLimits(int x);
    public string CalculateAdminCode(string password);
}