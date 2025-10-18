using Domain;
using MediatR;

namespace Application.Commands;

public record GetHistoryQuery(string Chain, int Limit = 100) : IRequest<List<BlockchainSnapshot>>;