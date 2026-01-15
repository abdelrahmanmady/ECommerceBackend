namespace ECommerce.Business.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> CanManageUserAsync(string targetUserId);
    }
}
