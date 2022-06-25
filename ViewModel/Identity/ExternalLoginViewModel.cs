using System.ComponentModel.DataAnnotations;

namespace MROCoatching.DataObjects.ViewModel.Identity
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
