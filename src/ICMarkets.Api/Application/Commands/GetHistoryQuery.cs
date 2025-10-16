using ICMarkets.Api.Domain;
using MediatR;

namespace ICMarkets.Api.Application.Commands;

public record GetHistoryQuery(string Chain, int Limit = 100) : IRequest<List<BlockchainSnapshot>>;