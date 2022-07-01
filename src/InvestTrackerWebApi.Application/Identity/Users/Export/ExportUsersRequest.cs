namespace InvestTrackerWebApi.Application.Identity.Users;

using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
using AutoMapper;
using InvestTrackerWebApi.Application.Exporters;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ExportUsersRequest : OrderableFilterBase, IRequest<Stream>
{
    [ToLowerContainsComparison]
    public string? UserName { get; set; }

    [ToLowerContainsComparison]
    public string? FirstName { get; set; }

    [ToLowerContainsComparison]
    public string? LastName { get; set; }

    [ToLowerContainsComparison]
    public string? Email { get; set; }

    [ToLowerContainsComparison]
    public string? PhoneNumber { get; set; }
}

public class ExportUsersRequestHandler : IRequestHandler<ExportUsersRequest, Stream>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IExcelWriter excelWriter;
    private readonly IMapper mapper;

    public ExportUsersRequestHandler(UserManager<ApplicationUser> userManager, IExcelWriter excelWriter, IMapper mapper)
    {
        this.userManager = userManager;
        this.excelWriter = excelWriter;
        this.mapper = mapper;
    }

    public async Task<Stream> Handle(ExportUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await this.userManager.Users.ApplyFilter(request).ToListAsync();

        return this.excelWriter.WriteToStream(this.mapper.Map<List<UserDto>>(users));
    }
}
