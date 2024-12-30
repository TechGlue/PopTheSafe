using Stateless;
using Stateless.Graph;

namespace Safe.Models;

public class Safe
{
    public required string SafeName { get; set; }
    
    // note the password can only be 4 digits don't want that being too long
    // don't need it in the constructor
    public int Password { get; set; }
    
    public int AdminPassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // open or closed states - the trigger is a char
    public StateMachine<string, char> SafeState = new StateMachine<string, char>("open");
    
    // initializing a new safe
    public Safe(string safeName, string password)
    {
        SafeName = safeName;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
    
    public void ChangePassword(int password)
    {
        // unit test the following
        
        // do work to ensure the password is of 4 digits. throw exceptions
        
        // do the calculations to calculate admin password you can break that off into another method
        
        // update pass word. 
        
        
    }
}