using API.Dtos;
using API.Entities;
using API.Payload.Pagination;

namespace API.Payload.Global{
    public class ListWithPagination<T>{
        public T? List { get; set; }
        public PaginationResponse? Pagination { get; set; }
    }
}