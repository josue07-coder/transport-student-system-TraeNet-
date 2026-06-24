using FluentAssertions;
using Transport.Application.Common.Pagination;

namespace Transport.Application.Tests.Common;

public class PaginationRequestTests
{
    [Fact]
    public void PageSize_WhenTooLarge_IsClampedToOneHundred()
    {
        var request = new PaginationRequest
        {
            PageNumber = 1,
            PageSize = 500
        };

        request.PageSize.Should().Be(100);
    }

    [Fact]
    public void PageNumberAndPageSize_WhenInvalid_AreNormalized()
    {
        var request = new PaginationRequest
        {
            PageNumber = -3,
            PageSize = 0
        };

        request.PageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
    }
}
