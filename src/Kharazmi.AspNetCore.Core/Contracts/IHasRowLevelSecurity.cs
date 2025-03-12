namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface IHasRowLevelSecurity
    {
        long UserId { get; set; }
    }
}