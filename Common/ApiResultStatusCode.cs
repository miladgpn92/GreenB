using System.ComponentModel.DataAnnotations;

namespace Common
{
    public enum ApiResultStatusCode
    {
        [Display(Name = "عملیات با موفقیت انجام شد")]
        Success = 0,

        [Display(Name = "خطایی در سرور رخ داده است")]
        ServerError = 1,

        [Display(Name = "پارامتر های ارسالی معتبر نیستند")]
        BadRequest = 2,

        [Display(Name = "یافت نشد")]
        NotFound = 3,

        [Display(Name = "لیست خالی است")]
        ListEmpty = 4,

        [Display(Name = "خطایی در پردازش رخ داد")]
        LogicError = 5,

        [Display(Name = "خطای احراز هویت")]
        UnAuthorized = 6,

        [Display(Name = "شماره موبایل تایید شده نیست")]
        PhoneNotConfirm = 7,

        [Display(Name = "برای این نام کاربری رمز عبوری ثبت نشده است.لطفا به صفحه انتخاب رمز عبور مراجعه کنید")]
        PasswordNotFound = 8,

        [Display(Name = "شما قبلا ثبت نام کرده اید.لطفا به صفحه ورود به سایت مراجعه فرمایید")]
        Registered = 9,

        [Display(Name = "خطای احراز هویت")]
        TokenIsExpired = 10,
    }
}
