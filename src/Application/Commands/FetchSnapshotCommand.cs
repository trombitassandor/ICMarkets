using MediatR;

namespace ICMarkets.Application.Commands;

public record FetchSnapshotCommand(string Chain) : IRequest<bool>;