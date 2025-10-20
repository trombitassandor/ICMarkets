using ICMarkets.Domain;
using MediatR;

namespace ICMarkets.Application.Commands;

public record GetHistoryQuery(string Chain, int Limit = 100) : 
    IRequest<List<BlockchainSnapshot>>;