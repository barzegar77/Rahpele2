using Microsoft.EntityFrameworkCore;
using NomadicMVC.ViewModels.Pagination;
using Rahpele.Common;
using Rahpele.Models;
using Rahpele.Models.Data;
using Rahpele.Services.Interfaces;
using Rahpele.ViewModels;
using Rahpele.ViewModels.User;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Rahpele.Services
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserManager(ApplicationDbContext context,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User> GetUserByPhoneNumberOrUserName(string phoneNumber, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber || u.UserName == username);
        }

        public Guid GetUserIdByPhoneNumber(string phoneNumber)
        {
            return _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber).Id;
        }


        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task<BaseFilterViewModel<ListUsersForManageViewModel>> GetUsersListForManageAsync(int pageIndex, int pageLength, string searchString)
        {
            var result = new BaseFilterViewModel<ListUsersForManageViewModel>();

            IQueryable<User> usersQuery = _context.Users
                .Where(x => !x.IsDeleted.Value)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery
                    .Where(x => x.UserName.Contains(searchString) || x.PhoneNumber.Contains(searchString) || x.Email.Contains(searchString));
            }

            int totalUsers = await usersQuery.CountAsync();
            int filteredUsers = totalUsers;
            var startingRowId = (pageIndex - 1) * pageLength + 1;
            var filteredUsersList = usersQuery
                .OrderByDescending(x => x.RegistrationDate)
                .Skip((pageIndex - 1) * pageLength)
                .Take(pageLength)
                .AsEnumerable()
                .Select((x, index) => new ListUsersForManageViewModel
                {
                    Id = x.Id,
                    RowId = startingRowId + index,
                    AvatarThumbnail = x.AvatarThumbnail,
                    Email = (x.Email != null ? x.Email : "---"),
                    IsDeleted = (x.IsDeleted != null ? x.IsDeleted : false).Value,
                    IsEmailConfirmed = (x.IsEmailConfirmed != null ? x.IsEmailConfirmed : false).Value,
                    IsPhoneNumberConfirmed = (x.IsPhoneNumberConfirmed != null ? x.IsPhoneNumberConfirmed : false).Value,
                    PhoneNumber = (x.PhoneNumber != null ? x.PhoneNumber : "---"),
                    RegistrationDate = MyDateTime.GetShamsiDateFromGregorian(x.RegistrationDate.Value),
                    UserName = (x.UserName != null ? x.UserName : "---")
                })
                .ToList();

            result.RecordsTotal = totalUsers;
            result.RecordsFiltered = filteredUsers;
            result.Entities = filteredUsersList;

            return result;
        }



        public async Task<FunctionResult> AddUserInAdminPanel(ManageUserViewModel model)
        {
            if(model!= null)
            {
                bool validUsernameResult = IsValidUsername(model.UserName);
                if(!validUsernameResult)
                {
                    return new FunctionResult(false, "نام کاربری نامعتبر است. نام کاربری فقط می‌تواند شامل حروف انگلیسی (بزرگ یا کوچک) و اعداد باشد.");
                }
                bool isUnicPhoneNumber = IsUnicPhoneNumber(model.PhoneNumber);
                if (!isUnicPhoneNumber)
                {
                    return new FunctionResult(false,"شماره موبایل تکراریست");
                }
                var isUserNameUnic = IsUnicUserName(model.UserName);
                if (!isUserNameUnic)
                {
                    return new FunctionResult(false, "نام کاربری قبلا ثبت شده است.");
                }

                User newUser = new User
                {
                    Biography = model.Biography,
                    BusinessPhoneNumber = model.BusinessPhoneNumber,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    WebSiteLink = model.WebSiteLink,
                    FullName = model.FullName,
                    HashedPassword = _passwordHasher.HashPassword(model.Password),
                    IsDeleted = model.IsDeleted,
                    IsEmailConfirmed = model.IsEmailConfirmed,
                    IsPhoneNumberConfirmed = model.IsPhoneNumberConfirmed,
                    RegistrationDate = DateTime.Now,
                    UserName = model.UserName,
                    Slug = GenerateSlug(model.UserName),
                };

                // Generate and save the Gravatar image
                string userNameHash = GetUserNameHash(model.UserName);
                string gravatarUrl = $"https://www.gravatar.com/avatar/{userNameHash}?d=identicon";
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        using (var response = await httpClient.GetAsync(gravatarUrl))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                string avatarFileName = GetAvatarFileName(DateTime.Now, model.UserName);
                                string avatarFilePath = Path.Combine(GetAvatarDirectoryPath(), avatarFileName);
                                var imageStream = await response.Content.ReadAsStreamAsync();
                                using (var fileStream = new FileStream(avatarFilePath, FileMode.Create))
                                {
                                    await imageStream.CopyToAsync(fileStream);
                                }

                                newUser.AvatarStandard = avatarFileName;
                                newUser.AvatarThumbnail = avatarFileName;
                                //await UpdateUserAsync(newUser);
                            }
                        }
                    }
                    catch (HttpRequestException)
                    {
                        newUser.AvatarStandard = "DefaultAvatar.png";
                        newUser.AvatarThumbnail = "DefaultAvatar.png";
                        //await UpdateUserAsync(newUser);
                    }
                }

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return new FunctionResult(true, "اطلاعات کاربری با موفقیت ثبت شد");

            }
            return new FunctionResult(false, "لطفا اطلاعات کاربری را وارد کنید");
        }


        public ManageUserViewModel GetUserForManage(Guid userId)
        {
            return _context.Users.Where(x => x.Id == userId)
                .Select(x => new ManageUserViewModel
                {
                    Id = x.Id,
                    Biography = x.Biography,
                    BusinessPhoneNumber = x.BusinessPhoneNumber,
                    Email = x.Email,
                    FullName = x.FullName,
                    IsEmailConfirmed = x.IsEmailConfirmed.Value,
                    IsPhoneNumberConfirmed = x.IsPhoneNumberConfirmed.Value,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    WebSiteLink = x.WebSiteLink
                }).FirstOrDefault();
        }

        public async Task<FunctionResult> UpdateUserInAdminPanel(ManageUserViewModel model)
        {
            if (model != null)
            {
                bool validUsernameResult = IsValidUsername(model.UserName);
                if (!validUsernameResult)
                {
                    return new FunctionResult(false, "نام کاربری نامعتبر است. نام کاربری فقط می‌تواند شامل حروف انگلیسی (بزرگ یا کوچک) و اعداد باشد.");
                }

                bool isUnicPhoneNumber = IsUnicPhoneNumberForUpdate(model.PhoneNumber, model.Id.Value);
                if (!isUnicPhoneNumber)
                {
                    return new FunctionResult(false, "شماره موبایل تکراریست");
                }
                var isUserNameUnic = IsUnicUserNameForUpdate(model.UserName, model.Id.Value);
                if (!isUserNameUnic)
                {
                    return new FunctionResult(false, "نام کاربری قبلا ثبت شده است.");
                }

                var currentUser = _context.Users.Find(model.Id);
                if(currentUser != null)
                {
                    currentUser.Biography = model.Biography;
                    currentUser.BusinessPhoneNumber = model.BusinessPhoneNumber;
                    currentUser.Email = model.Email;
                    currentUser.PhoneNumber = model.PhoneNumber;
                    currentUser.WebSiteLink = model.WebSiteLink;
                    currentUser.FullName = model.FullName;
                    currentUser.IsEmailConfirmed = model.IsEmailConfirmed;
                    currentUser.IsPhoneNumberConfirmed = model.IsPhoneNumberConfirmed;
                    currentUser.UserName = model.UserName;

                    _context.Users.Update(currentUser);
                    _context.SaveChanges();
                    return new FunctionResult(true, "اطلاعات کاربری با موفقیت ثبت شد");
                }

            }
            return new FunctionResult(false, "لطفا اطلاعات کاربری را وارد کنید");
        }



        public FunctionResult ManageUserRoles(ManageRoleToUserViewModel model)
        {
            if (model == null)
            {
                return new FunctionResult(false, "خطا در برقراری ارتباط با سرور");
            }

            var currentUserRoles = _context.UserRoles.Where(x => x.UserId == model.UserId);

            if (currentUserRoles.Any())
            {
                _context.UserRoles.RemoveRange(currentUserRoles);
            }

            if (model.RoleIds != null && model.RoleIds.Any())
            {
                List<UserRole> newUserRoles = model.RoleIds.Select(roleId => new UserRole
                {
                    RoleId = roleId,
                    UserId = model.UserId,
                }).ToList();

                _context.UserRoles.AddRange(newUserRoles);
            }

            _context.SaveChanges();

            return new FunctionResult(true, "نقش‌ها به حساب کاربری انتساب داده شد");
        }


        public List<Guid?>? GetListRolesByUserIdForManage(Guid? userId)
        {
            if(userId != null)
            {
                return _context.UserRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
            }
            return null;
        }

        public bool IsUserHaveAnyRole(Guid? userId)
        {
            return _context.UserRoles.Any(x => x.UserId == userId);
        }



        public string GetUserNameHash(string username)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(username.ToLower()));
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
            }
            return sb.ToString().ToLower();
        }


        public string GetAvatarFileName(DateTime registerDate, string userName)
        {
            return $"{registerDate:yyyy-MM-dd}-{userName.Replace("-", "").Replace(" ", "").Replace(".", "").ToLower()}.png";
        }

        public string GetAvatarDirectoryPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "appdata", "useravatar");
        }

        public bool IsValidEmail(string email)
        {
            const string emailRegexPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            var regex = new Regex(emailRegexPattern);
            return regex.IsMatch(email);
        }


        public bool IsValidUsername(string username)
        {
            const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            foreach (char c in username)
            {
                if (!ValidCharacters.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        public bool VerifyPasswordStrength(string password)
        {
            const int RequiredLength = 6;
            const int RequiredUniqueChars = 3;
            const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";

            if (password.Length < RequiredLength)
            {
                return false;
            }

            if (password.Distinct().Count() < RequiredUniqueChars)
            {
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            if (!password.Any(c => ValidChars.Contains(c)))
            {
                return false;
            }

            return true;
        }


        public bool IsUnicPhoneNumber(string phoneNumber)
        {
            var user = _context.Users.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
            if (user != null)
            {
                return false;
            }
            return true;
        }

        public bool IsUnicPhoneNumberForUpdate(string phoneNumber, Guid currentUserId)
        {
            var userWithSamePhoneNumber = _context.Users.FirstOrDefault(x => x.PhoneNumber == phoneNumber && x.Id != currentUserId);

            if (userWithSamePhoneNumber != null)
            {
                return false; 
            }

            return true;
        }

        public bool IsUnicUserName(string userName)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                return false;
            }
            return true;
        }

        public bool IsUnicUserNameForUpdate(string userName, Guid currentUserId)
        {
            var userWithSameUserName = _context.Users.FirstOrDefault(x => x.UserName == userName && x.Id != currentUserId);
            if (userWithSameUserName != null)
            {
                return false;
            }

            return true;
        }



        public string GenerateSlug(string text)
        {
            text = Regex.Replace(text, @"[^a-zA-Z0-9]", " ");
            text = text.ToLower();
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Replace(" ", "-");
            text = Regex.Replace(text, "-{2,}", "-");
            const int maxLength = 50;
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }
            return text;
        }

    }
}