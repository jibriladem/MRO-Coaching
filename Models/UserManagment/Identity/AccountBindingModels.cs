using MROCoatching.DataObjects.Models.Others;
using FluentValidation;
using System.ComponentModel.DataAnnotations;


namespace MROCoatching.DataObjects.Models.UserManagment.Identity
{
    // Models used as parameters to AccountController actions.
    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }



    public class ChangePasswordResBO : ValidationErrorBO
    {

    }
    public class ChangePasswordBindingModelValidator : AbstractValidator<ChangePasswordBindingModel>
    {
        public ChangePasswordBindingModelValidator()
        {
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Old password is required");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required");
        }
    }


    public class ForgotPasswordViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
    }

    public class ForgetPasswordBindingModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgetPasswordBindingModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("User name is required");
            //RuleFor(x => x.Email).EmailAddress().WithMessage("Thats not a valid email address.");
        }
    }

    public class ForgetPasswordRes : ValidationErrorBO
    {

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("User name is required.");
            //RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required");
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
        }
    }
    public class ResetPasswordRes : ValidationErrorBO
    {

    }
    public class ConfirmEmailViewModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

    public class ConfirmEmailRes : ValidationErrorBO
    { }

    public class LoginBindingModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FullName { get; set; }
    }

    //BEGIN NEWLY ADDED BO'S
    public class RegisterResBO : ValidationErrorBO
    {

    }
    public class RegisterBindingModelValidator : AbstractValidator<RegisterBindingModel>
    {
        public RegisterBindingModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required");
        }
    }
    //END NEWLY ADDED BO'S
    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class Constants
    {
        public const string CourseView = "CourseView";
    }
}
