using System.Linq;

namespace SecureByDesign.Host.Domain.Model
{
    public class SearchTerm
    {
        public SearchTerm(string term)
        {
            AssertValidTerm(term);

            Value = term;
        }

        public string Value { get; }

        public static bool IsValidTerm(string term)
        {
            return !string.IsNullOrEmpty(term) && term.Length < 20 && (term.All(char.IsLetterOrDigit) || term.All(char.IsWhiteSpace));
        }

        public static void AssertValidTerm(string term)
        {
            if (!IsValidTerm(term))
            {
                throw new InvalidSearchTermArgumentException($"The search term {term} is not valid.");
            }
        }
    }
}