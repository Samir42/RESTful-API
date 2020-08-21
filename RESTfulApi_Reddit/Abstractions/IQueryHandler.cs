using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Abstractions
{
    public interface IQueryHandler<TQuery,TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}
