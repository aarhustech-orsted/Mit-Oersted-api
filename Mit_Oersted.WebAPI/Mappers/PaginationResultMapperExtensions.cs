using Mit_Oersted.Domain.Mappers;
using Mit_Oersted.Domain.Pagination;
using System.Linq;

namespace Mit_Oersted.WebApi.Mappers
{
    public static class PaginationResultMapperExtensions
    {
        public static PaginationResult<TTarget> Map<TSource, TTarget>(this PaginationResult<TSource> source, IMapper<TSource, TTarget> mapper)
        {
            return new PaginationResult<TTarget>
            {
                MetaData = source.MetaData,
                Result = source.Result.Select(mapper.Map).ToList()
            };
        }
    }
}
