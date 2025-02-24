using Business.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("ABC", false)]
    [InlineData("1234567890", false)]
    [InlineData("1234567890123", false)]
    [InlineData("123456789012", true)]
    
    public void IsIsinValid_ShouldReturnCorrectResult(string? isin, bool expected)
    {
        // Act
        bool actual = isin.IsIsinValid();

        // Assert
        Assert.Equal(expected, actual);
    }
}