using Rahpele.Models;
using Rahpele.Services.Interfaces;
using Rahpele.ViewModels.Authentication;

namespace Rahpele.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISenderService _senderService;
        private readonly IFileUploader _fileUploader;

        public AuthenticationService(IUserManager userManager,
            IPasswordHasher passwordHasher,
            ISenderService senderService,
            IFileUploader fileUploader)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _senderService = senderService;
            _fileUploader = fileUploader;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterViewModel model)
        {
            var validUsername = _userManager.IsValidUsername(model.Username);
            if (!validUsername)
            {
                return new AuthenticationResult(false, "نام کاربری نامعتبر است. نام کاربری فقط می‌تواند شامل حروف انگلیسی (بزرگ یا کوچک) و اعداد باشد.");
            }

            var passwordStrengthResult = _userManager.VerifyPasswordStrength(model.Password);
            if (!passwordStrengthResult)
            {
                return new AuthenticationResult(false, "گذرواژه‌ای که وارد کردید به اندازه کافی قوی نیست.");
            }

            var isUserNameUnic = _userManager.IsUnicUserName(model.Username);
            if (!isUserNameUnic)
            {
                return new AuthenticationResult(false, "نام کاربری قبلا ثبت شده است.");
            }

            const int minutesToWaitBetweenAttempts = 1;

            var existingUser = await _userManager.GetUserByPhoneNumberOrUserName(model.PhoneNumber, model.Username);

            // Generate a unique verification code
            const string chars = "0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            if (existingUser != null && existingUser.IsPhoneNumberConfirmed == false)
            {
                var lastSMSSentDateTime = existingUser.PhoneNumberConfirmationSentDateTime;
                var timeElapsedSinceLastSMS = DateTime.Now.Subtract(lastSMSSentDateTime.Value);

                if (timeElapsedSinceLastSMS.TotalMinutes < minutesToWaitBetweenAttempts)
                {
                    return new AuthenticationResult(false, "لطفا بعد از یک دقیقه دوباره تلاش کنید.");
                }
                else
                {
                    // Update the user entity with the new verification code and sent datetime
                    existingUser.PhoneNumberConfirmationCode = verificationCode;
                    existingUser.PhoneNumberConfirmationSentDateTime = DateTime.Now;
                    existingUser.UpdatedDate = DateTime.Now;
                    existingUser.HashedPassword = _passwordHasher.HashPassword(model.Password);
                    existingUser.PhoneNumber = model.PhoneNumber;
                    existingUser.UserName = model.Username;

                    bool resultSendSMS = await _senderService.SendSMSAsync(model.PhoneNumber, model.Username, verificationCode, 1);
                    if (!resultSendSMS)
                    {
                        return new AuthenticationResult(false, "متاسفانه ارسال پیامک تایید هویت، با خطا مواجه شد. لطفا دوباره سعی کنید.");
                    }

                    await _userManager.UpdateUserAsync(existingUser);

                    return new AuthenticationResult(true, "پیامک تایید هویت با موفقیت ارسال شد. لطفا پیامک های خود را بررسی کنید.");
                }
            }
            else if (existingUser != null && existingUser.IsPhoneNumberConfirmed == true)
            {
                return new AuthenticationResult(false, "شماره تلفن همراه قبلا ثبت شده است.");
            }

            var hashedPassword = _passwordHasher.HashPassword(model.Password);

            var user = new User
            {
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                HashedPassword = hashedPassword,
                RegistrationDate = DateTime.Now,
                IsPhoneNumberConfirmed = false,
                IsDeleted = false,
                PhoneNumberConfirmationCode = verificationCode,
                PhoneNumberConfirmationSentDateTime = DateTime.Now,
                Slug = _userManager.GenerateSlug(model.Username),
            };

            bool resultSendSMS2 = await _senderService.SendSMSAsync(model.PhoneNumber, model.Username, verificationCode, 1);
            if (!resultSendSMS2)
            {
                return new AuthenticationResult(false, "متاسفانه ارسال پیامک تایید هویت، با خطا مواجه شد. لطفا دوباره سعی کنید.");
            }
            await _userManager.AddUserAsync(user);

            // Generate and save the Gravatar image
            string userNameHash = _userManager.GetUserNameHash(model.Username);
            string gravatarUrl = $"https://www.gravatar.com/avatar/{userNameHash}?d=identicon";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync(gravatarUrl))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string avatarFileName = _userManager.GetAvatarFileName(user.RegistrationDate.Value, user.UserName);
                            string avatarFilePath = Path.Combine(_userManager.GetAvatarDirectoryPath(), avatarFileName);
                            var imageStream = await response.Content.ReadAsStreamAsync();
                            using (var fileStream = new FileStream(avatarFilePath, FileMode.Create))
                            {
                                await imageStream.CopyToAsync(fileStream);
                            }

                            user.AvatarStandard = avatarFileName;
                            user.AvatarThumbnail = avatarFileName;
                            await _userManager.UpdateUserAsync(user);
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    user.AvatarStandard = "DefaultAvatar.png";
                    user.AvatarThumbnail = "DefaultAvatar.png";
                    await _userManager.UpdateUserAsync(user);
                    //return new AuthenticationResult(false, "مشکلی در دریافت آواتار اتفاق افتاد. لطفاً بعداً دوباره تلاش کنید.");
                }
            }

            return new AuthenticationResult(true, "پیامک تایید هویت با موفقیت ارسال شد. لطفا پیامک های خود را بررسی کنید.");
        }




    

        public AuthenticationResult ConfrimPhoneNumber(PhoneNumberConfirmationViewModel model)
        {
            var existingUser = _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;

            if (existingUser == null)
            {
                return new AuthenticationResult(false, "کاربر با این شماره تلفن وجود ندارد.");
            }

            if (existingUser.IsPhoneNumberConfirmed.Value == true)
            {
                return new AuthenticationResult(false, "شماره تلفن همراه قبلاً تایید شده است.");
            }

            if (existingUser.PhoneNumberConfirmationCode == null || existingUser.PhoneNumberConfirmationCode != model.VerificationCode)
            {
                return new AuthenticationResult(false, "کد تایید وارد شده نادرست است.");
            }

            var currentTime = DateTime.Now;
            var timeElapsedSinceSMS = currentTime.Subtract(existingUser.PhoneNumberConfirmationSentDateTime.Value);

            const int minutesToWaitForConfirmation = 3;
            if (timeElapsedSinceSMS.TotalMinutes > minutesToWaitForConfirmation)
            {
                return new AuthenticationResult(false, "مدت زمان اعتبار کد تایید به پایان رسیده است. لطفاً مجدداً تلاش کنید.");
            }

            existingUser.IsPhoneNumberConfirmed = true;
            existingUser.PhoneNumberConfirmationCode = null;
            existingUser.PhoneNumberConfirmationSentDateTime = null;
            _userManager.UpdateUserAsync(existingUser);

            return new AuthenticationResult(true, "شماره تلفن همراه با موفقیت تایید شد.");
        }


        public AuthenticationResult ResendPhoneNumberVerificationCode(ResendPhoneNumberVerificationCodeViewModel model)
        {
            var existingUser = _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;
            if (existingUser == null)
            {
                return new AuthenticationResult(false, "کاربر با این شماره تلفن وجود ندارد.");
            }
            if (existingUser.IsPhoneNumberConfirmed.Value)
            {
                return new AuthenticationResult(false, "شماره تلفن همراه قبلاً تایید شده است.");
            }
            if(existingUser.PhoneNumberConfirmationSentDateTime != null)
            {
                var currentTime = DateTime.Now;
                var timeElapsedSinceLastSMS = currentTime.Subtract(existingUser.PhoneNumberConfirmationSentDateTime.Value);
                if (timeElapsedSinceLastSMS.TotalMinutes < 1)
                {
                    return new AuthenticationResult(false, "لطفا بعد از یک دقیقه دوباره تلاش کنید.");
                }
            }
    

            const string chars = "0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

            bool resultSendSMS = _senderService.SendSMS(model.PhoneNumber, "کاربر", verificationCode, 1);
            if (!resultSendSMS)
            {
                return new AuthenticationResult(false, "متاسفانه ارسال پیامک تایید هویت، با خطا مواجه شد. لطفا دوباره سعی کنید.");
            }

            existingUser.PhoneNumberConfirmationCode = verificationCode;
            existingUser.PhoneNumberConfirmationSentDateTime = DateTime.Now;
            _userManager.UpdateUserAsync(existingUser);

            return new AuthenticationResult(true, "پیامک تایید هویت با موفقیت ارسال شد. لطفا پیامک های خود را بررسی کنید.");
        }


        public AuthenticationResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var existingUser =  _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;
            if (existingUser == null)
            {
                return new AuthenticationResult(false, "کاربر با این شماره تلفن وجود ندارد.");
            }

            const string chars = "0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());


            existingUser.PhoneNumberConfirmationCode = verificationCode;
            existingUser.PhoneNumberConfirmationSentDateTime = DateTime.Now;
            
            bool resaultSendSMS = _senderService.SendSMS(existingUser.PhoneNumber, existingUser.UserName, verificationCode, 2);
            if (!resaultSendSMS)
            {
                return new AuthenticationResult(false, "متاسفانه ارسال پیامک تایید هویت، با خطا مواجه شد. لطفا دوباره سعی کنید.");
            }
            _userManager.UpdateUserAsync(existingUser);

            return new AuthenticationResult( true, "کد فراموشی رمز عبور با موفقیت ارسال شد. لطفاً پیامک های خود را بررسی کنید." );
        }


        public AuthenticationResult ResetPassword(ResetPasswordViewModel model)
        {
            var existingUser = _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;
            if (existingUser == null)
            {
                return new AuthenticationResult(false, "کاربر با این شماره تلفن وجود ندارد.");
            }

            if (existingUser.PhoneNumberConfirmationCode == null || existingUser.PhoneNumberConfirmationCode != model.VerificationCode)
            {
                return new AuthenticationResult(false, "کد فراموشی رمز عبور نادرست است." );
            }

            var currentTime = DateTime.Now;
            var timeElapsedSinceSMS = currentTime.Subtract(existingUser.PhoneNumberConfirmationSentDateTime.Value);
            if (timeElapsedSinceSMS.TotalMinutes > 3)
            {
                return new AuthenticationResult(false, "مدت زمان اعتبار کد تایید به پایان رسیده است. لطفاً مجدداً تلاش کنید.");
            }

            bool isStrongPassword = _userManager.VerifyPasswordStrength(model.Password);
            if (!isStrongPassword)
            {
                return new AuthenticationResult(false, "گذرواژه‌ای که وارد کردید به اندازه کافی قوی نیست." );
            }

            // ذخیره رمز عبور جدید برای کاربر
            existingUser.HashedPassword = _passwordHasher.HashPassword(model.Password);
            existingUser.PhoneNumberConfirmationCode = null;
            existingUser.PhoneNumberConfirmationSentDateTime = null;
             _userManager.UpdateUserAsync(existingUser);

            return new AuthenticationResult(true, "رمز عبور با موفقیت تغییر یافت." );
        }



        public AuthenticationResult ResendForgotPasswordVerificationCode(ResendPhoneNumberVerificationCodeViewModel model)
        {
            var existingUser = _userManager.GetUserByPhoneNumber(model.PhoneNumber).Result;
            if (existingUser == null)
            {
                return new AuthenticationResult(false, "کاربر با این شماره تلفن وجود ندارد.");
            }
            if (existingUser.IsPhoneNumberConfirmed.Value)
            {
                return new AuthenticationResult(false, "شماره تلفن همراه قبلاً تایید شده است.");
            }

            var lastSMSSentDateTime = existingUser.PhoneNumberConfirmationSentDateTime;
            var timeElapsedSinceLastSMS = DateTime.Now.Subtract(lastSMSSentDateTime.Value);
            if (timeElapsedSinceLastSMS.TotalMinutes < 1)
            {
                return new AuthenticationResult(false, "لطفا بعد از یک دقیقه دوباره تلاش کنید.");
            }

            const string chars = "0123456789";
            var random = new Random();
            var verificationCode = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

            bool resultSendSMS = _senderService.SendSMS(model.PhoneNumber, "کاربر", verificationCode, 2);
            if (!resultSendSMS)
            {
                return new AuthenticationResult(false, "متاسفانه ارسال پیامک تایید هویت، با خطا مواجه شد. لطفا دوباره سعی کنید.");
            }

            existingUser.PhoneNumberConfirmationCode = verificationCode;
            existingUser.PhoneNumberConfirmationSentDateTime = DateTime.Now;
            _userManager.UpdateUserAsync(existingUser);

            return new AuthenticationResult(true, "پیامک تایید هویت با موفقیت ارسال شد. لطفا پیامک های خود را بررسی کنید.");
        }



     




    }
}