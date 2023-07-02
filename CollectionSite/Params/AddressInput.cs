using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CollectionSite
{
    public class AddressInput : IValidatableObject
    {
        const string _paramName = "addresses";

        string _targets = string.Empty;

        [FromQuery(Name = _paramName)]
        public string? Addresses { get; set; }

        public string GetAddress()
            => _targets;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Addresses) || !IsNumbersOnly())
            {
                yield return new ValidationResult(
                    $"Set addresses '{Addresses}'");
                yield break;
            }
            else if (!IsAllIpFormat(Addresses))
            {
                yield return new ValidationResult(
                     $"Set addresses '{Addresses}'");
                yield break;
            }
            else
            {
                _targets = Addresses;
                yield break;
            }
        }

        bool IsNumbersOnly()
        {
            Addresses = Addresses.Trim(' ');
            foreach (char c in Addresses)
                if ((c < '0' || c > '9') && c != '.' && c != ' ')
                    return false;

            return true;
        }

        bool IsAllIpFormat(string str)
        {
            var arr = str.Split(' ');
            if (arr == null || !arr.Any())
                return false;

            foreach (var i in arr)
                if (!IsIpFormat(i))
                    return false;

            return true;
        }

        bool IsIpFormat(string str)
        {
            var ip = new Regex("^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");

            return ip.IsMatch(str);
        }
    }
}