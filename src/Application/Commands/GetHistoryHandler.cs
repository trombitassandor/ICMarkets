using ICMarkets.Domain;
using MediatR;

namespace ICMarkets.Application.Commands;

public class GetHistoryHandler : IRequestHandler<GetHistoryQuery, List<BlockchainSnapshot>>
{
    private readonly IRepository _repository;

    public GetHistoryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BlockchainSnapshot>> Handle(GetHistoryQuery query, CancellationToken ct)
    {
        return await _repository.GetHistoryAsync(query.Chain, query.Limit, ct); 
    }
}