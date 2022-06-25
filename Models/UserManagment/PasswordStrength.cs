using MROCoatching.DataObjects.Data.Context;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MROCoatching.DataObjects.Models.UserManagment
{
    public enum PasswordStrength
    {
        /// &lt;summary&gt;
        /// Blank Password (empty and/or space chars only)
        /// &lt;/summary&gt;
        Blank = 0,
        /// &lt;summary&gt;
        /// Either too short (less than 5 chars), one-case letters only or digits only
        /// &lt;/summary&gt;
        VeryWeak = 1,
        /// &lt;summary&gt;
        /// At least 5 characters, one strong condition met (&gt;= 8 chars with 1 or more UC letters, LC letters, digits &amp; special chars)
        /// &lt;/summary&gt;
        Weak = 2,
        /// &lt;summary&gt;
        /// At least 5 characters, two strong conditions met (&gt;= 8 chars with 1 or more UC letters, LC letters, digits &amp; special chars)
        /// &lt;/summary&gt;
        Medium = 3,
        /// &lt;summary&gt;
        /// At least 8 characters, three strong conditions met (&gt;= 8 chars with 1 or more UC letters, LC letters, digits &amp; special chars)
        /// &lt;/summary&gt;
        Strong = 4,
        /// &lt;summary&gt;
        /// At least 8 characters, all strong conditions met (&gt;= 8 chars with 1 or more UC letters, LC letters, digits &amp; special chars)
        /// &lt;/summary&gt;
        VeryStrong = 5
    }

    public static class PasswordCheck
    {
        /// &lt;summary&gt;
        /// Generic method to retrieve password strength: use this for general purpose scenarios, 
        /// i.e. when you don't have a strict policy to follow.
        /// &lt;/summary&gt;
        /// &lt;param name="password"&gt;&lt;/param&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static PasswordStrength GetPasswordStrength(string password)
        {
            int score = 0;
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password.Trim())) return PasswordStrength.Blank;
            if (!HasMinimumLength(password, 5)) return PasswordStrength.VeryWeak;
            if (HasMinimumLength(password, 8)) score++;
            if (HasUpperCaseLetter(password) && HasLowerCaseLetter(password)) score++;
            if (HasDigit(password)) score++;
            if (HasSpecialChar(password)) score++;
            return (PasswordStrength)score;
        }

        /// &lt;summary&gt;
        /// Sample password policy implementation:
        /// - minimum 8 characters
        /// - at lease one UC letter
        /// - at least one LC letter
        /// - at least one non-letter char (digit OR special char)
        /// &lt;/summary&gt;
        /// &lt;returns&gt;&lt;/returns&gt;
        public static bool IsStrongPassword(string password)
        {
            return HasMinimumLength(password, 8)
                && HasUpperCaseLetter(password)
                  && HasLowerCaseLetter(password)
                    && (HasDigit(password) || HasSpecialChar(password));
        }

        /// &lt;summary&gt;
        /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
        /// &lt;/summary&gt;
        public static bool IsValidPassword(string password, PasswordOptions opts)
        {
            return IsValidPassword(
                password,
                opts.RequiredLength,
                opts.RequiredUniqueChars,
                opts.RequireNonAlphanumeric,
                opts.RequireLowercase,
                opts.RequireUppercase,
                opts.RequireDigit);
        }


        /// &lt;summary&gt;
        /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
        /// &lt;/summary&gt;
        public static bool IsValidPassword(
            string password,
            int requiredLength,
            int requiredUniqueChars,
            bool requireNonAlphanumeric,
            bool requireLowercase,
            bool requireUppercase,
            bool requireDigit)
        {
            if (!HasMinimumLength(password, requiredLength)) return false;
            if (!HasMinimumUniqueChars(password, requiredUniqueChars)) return false;
            if (requireNonAlphanumeric && !HasSpecialChar(password)) return false;
            if (requireLowercase && !HasLowerCaseLetter(password)) return false;
            if (requireUppercase && !HasUpperCaseLetter(password)) return false;
            if (requireDigit && !HasDigit(password)) return false;
            return true;
        }

        #region Helper Methods

        public static bool HasMinimumLength(string password, int minLength)
        {
            return password.Length >= minLength;
        }

        public static bool HasMinimumUniqueChars(string password, int minUniqueChars)
        {
            return password.Distinct().Count() >= minUniqueChars;
        }

        /// &lt;summary&gt;
        /// Returns TRUE if the password has at least one digit
        /// &lt;/summary&gt;
        public static bool HasDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }

        /// &lt;summary&gt;
        /// Returns TRUE if the password has at least one special character
        /// &lt;/summary&gt;
        public static bool HasSpecialChar(string password)
        {
            // return password.Any(c =&gt; char.IsPunctuation(c)) || password.Any(c =&gt; char.IsSeparator(c)) || password.Any(c =&gt; char.IsSymbol(c));
            return password.IndexOfAny("!@#$%^&amp;*?_~-£().,".ToCharArray()) != -1;
        }

        /// &lt;summary&gt;
        /// Returns TRUE if the password has at least one uppercase letter
        /// &lt;/summary&gt;
        public static bool HasUpperCaseLetter(string password)
        {
            return password.Any(c=> char.IsUpper(c));
        }

        /// &lt;summary&gt;
        /// Returns TRUE if the password has at least one lowercase letter
        /// &lt;/summary&gt;
        public static bool HasLowerCaseLetter(string password)
        {
            return password.Any(c=> char.IsLower(c));
        }
        #endregion
    }
}
