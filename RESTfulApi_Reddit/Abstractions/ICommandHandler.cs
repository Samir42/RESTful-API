using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Abstractions
{
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command);
    }
}
