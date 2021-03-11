﻿using Domain.Mappers;
using Domain.Pagination;
using System.Linq;

namespace Mit_Oersted.Mappers
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
