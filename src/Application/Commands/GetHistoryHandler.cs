using ICMarkets.Domain;
using MediatR;

namespace ICMarkets.Application.Commands;

public class GetHistoryHandler(IRepository repository) : 
    IRequestHandler<GetHistoryQuery, List<BlockchainSnapshot>>
{
    public async Task<List<BlockchainSnapshot>> Handle(GetHistoryQuery query, CancellationToken ct)
    {
        return await repository.GetHistoryAsync(query.Chain, query.Limit, ct); 
    }
}