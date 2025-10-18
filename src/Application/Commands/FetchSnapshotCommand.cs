using MediatR;

namespace Application.Commands;

public record FetchSnapshotCommand(string Chain) : IRequest<bool>;