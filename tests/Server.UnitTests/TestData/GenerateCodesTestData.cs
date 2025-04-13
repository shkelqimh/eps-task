using System.Collections;

namespace Server.UnitTests.TestData;

public class GenerateCodesTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 10, 8, true };
        yield return new object[] { 50, 7, true };
        yield return new object[] { 101, 8, true };
        yield return new object[] { 10, 5, false };
        yield return new object[] { 10, 11, false };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}