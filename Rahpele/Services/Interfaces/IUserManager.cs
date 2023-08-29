using NomadicMVC.ViewModels.Pagination;
using Rahpele.Models;
using Rahpele.ViewModels;
using Rahpele.ViewModels.User;

namespace Rahpele.Services.Interfaces
{
    public interface IUserManager
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByPhoneNumber(string phoneNumber);
        Task<User> GetUserByPhoneNumberOrUserName(string phoneNumber, string username);
        Guid GetUserIdByPhoneNumber(string phoneNumber);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);

        Task<BaseFilterViewModel<ListUsersForManageViewModel>> GetUsersListForManageAsync(int pageIndex, int pageLength, string searchString);
        Task<FunctionResult> AddUserInAdminPanel(ManageUserViewModel model);
        Task<FunctionResult> UpdateUserInAdminPanel(ManageUserViewModel model);
        ManageUserViewModel GetUserForManage(Guid userId);
        FunctionResult ManageUserRoles(ManageRoleToUserViewModel model);
        List<Guid?>? GetListRolesByUserIdForManage(Guid? userId);
        bool IsUserHaveAnyRole(Guid? userId);

        string GetUserNameHash(string username);
        string GetAvatarFileName(DateTime registerDate, string userName);
        string GetAvatarDirectoryPath();
        bool IsValidEmail(string email);
        bool IsValidUsername(string username);
        bool VerifyPasswordStrength(string password);
        bool IsUnicUserName(string userName);
        bool IsUnicPhoneNumberForUpdate(string phoneNumber, Guid currentUserId);
        bool IsUnicUserNameForUpdate(string userName, Guid currentUserId);
        string GenerateSlug(string text);
    }
}