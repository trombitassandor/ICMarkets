using MediatR;

namespace ICMarkets.Api.Application.Commands;

public record FetchSnapshotCommand(string Chain) : IRequest<bool>;