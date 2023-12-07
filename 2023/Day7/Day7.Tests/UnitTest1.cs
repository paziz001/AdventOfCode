namespace Day7.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("KTJJT", "QQQJA", 1)]
    [InlineData("AJJJJ", "JJJJJ", 1)]
    [InlineData("QQQJA", "T55J5", 1)]
    [InlineData("KKJJA", "KKAAT", 1)]
    [InlineData("6JJ7TA", "5TJTA", 1)]
    [InlineData("AATT6", "JA612", 1)]
    [InlineData("2234J", "J1234", 1)]
    [InlineData("KKKJJ", "JJJJA", 1)]
    [InlineData("T55J5", "2AAAA", 1)]
    public void Comparer_CompareWhenJokerUsed_Successfully(string handA, string handB, int compareResult)
    {
        var sut = new HandComparer(true);

        sut.Compare(handA, handB).Should().Be(compareResult);
    }
}