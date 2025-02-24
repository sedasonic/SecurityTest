using System.Reflection.Metadata.Ecma335;

namespace Business.Extensions
{
    public static class StringExtensions
    {
        public static bool IsIsinValid(this string? isin) => isin?.Length == 12;
    }
}
