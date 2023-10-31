using System.IO.Compression;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.Json;
using API.Dtos;
using API.Entities;
using API.Payload.Filter;
using API.Payload.Global;
using API.Payload.Pagination;
using API.Payload.Search;
using API.Payload.Sort;
using API.Repository.Interface;
using API.Utils;
using Microsoft.EntityFrameworkCore;
namespace API.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(DataContext dataContext, ILogger<StudentRepository> logger)
        {
            this._dataContext = dataContext;
            this._logger = logger;
        }
        public async Task<bool> CreateStudentAsync(Student Student)
        {
            await _dataContext.Students.AddAsync(Student);
            return await Save();
        }

        public async Task<ListWithPagination<ICollection<Student>>> GetStudentsAsync(SearchParams searchParam)
        {
            List<ColumnFilter> columnFilters = new List<ColumnFilter>();
            if (!String.IsNullOrEmpty(searchParam.ColumnFilters))
            {
                try
                {
                    var colFilters = JsonSerializer.Deserialize<List<ColumnFilter>>(searchParam.ColumnFilters);
                    if(colFilters != null) columnFilters.AddRange(colFilters);
                }
                catch (Exception)
                {
                    columnFilters = new List<ColumnFilter>();
                }
            }

            List<ColumnSorting> columnSorting = new List<ColumnSorting>();
            if (!String.IsNullOrEmpty(searchParam.OrderBy))
            {
                try
                {
                    var orderFilters = JsonSerializer.Deserialize<List<ColumnSorting>>(searchParam.OrderBy);
                    if(orderFilters != null) columnSorting.AddRange(orderFilters);
                }
                catch (Exception)
                {
                    columnSorting = new List<ColumnSorting>();
                }
            }

            Expression<Func<Student, bool>>? filters = null;
            //First, we are checking our SearchTerm. If it contains information we are creating a filter.
            var searchTerm = "";
            if (!string.IsNullOrEmpty(searchParam.SearchTerm))
            {
                searchTerm = searchParam.SearchTerm.Trim().ToLower();
                filters = x => (x.FirstName != null && x.FirstName.ToLower().Contains(searchTerm)) ||
                (x.LastName != null && x.LastName.ToLower().Contains(searchTerm));
            }
            // Then we are overwriting a filter if columnFilters has data.
            if (columnFilters.Count > 0)
            {
                var customFilter = CustomExpressionFilter<Student>.CustomFilter(columnFilters, "students");
                filters = customFilter;
            }

            var query = filters == null ? _dataContext.Students : _dataContext.Students.AsQueryable().CustomQuery(filters);
            var filteredData = await (
                    columnSorting.Count> 0 ?
                        query
                            .Where(Stu => !Stu.IsDeleted)
                            .SortBy(columnSorting)
                            .CustomPagination(searchParam.PageNumber, searchParam.PageSize)
                            .ToListAsync():
                        query
                            .Where(Stu => !Stu.IsDeleted)
                            .OrderBy(Stu => Stu.id)
                            .CustomPagination(searchParam.PageNumber, searchParam.PageSize)
                            .ToListAsync()
                );
            var count = query.Count();
            var totalPage = Math.Floor((Decimal)count / searchParam.PageSize);
            _logger.LogInformation("Query {@Query}", query);
            PaginationResponse paginationResponse = new PaginationResponse(searchParam.PageNumber, searchParam.PageSize, (int) totalPage, count);
            return new ListWithPagination<ICollection<Student>> { List = filteredData, Pagination = paginationResponse };
            // return await _dataContext.Students.ToListAsync();
        }

        public async Task<ICollection<Student>> GetDeletedStudentsAsync()
        {
            return await _dataContext.Students.Where(Stu => Stu.IsDeleted).ToListAsync();
        }

        public async Task<Student> GetStudentByIDAsync(int StudentId)
        {
            return await _dataContext.Students.FirstAsync(Stu => Stu.id == StudentId);
        }

        public async Task<ICollection<Student>> GetAllStudentsAsync()
        {
            return await _dataContext.Students.ToListAsync();
        }

        public async Task<bool> HardDeleteStudentAsync(Student Student)
        {
            _dataContext.Remove(Student);
            return await Save();
        }

        public async Task<bool> SoftDeleteStudentAsync(int StudentId)
        {
            var _exisitngCompany = await GetStudentByIDAsync(StudentId);

            if (_exisitngCompany != null)
            {
                _exisitngCompany.IsDeleted = true;
                return await Save();
            }
            return false;
        }

        public async Task<bool> StudentExistAsync(int StudentId)
        {
            return await _dataContext.Students.AnyAsync(Stu => Stu.id == StudentId);
        }
        public async Task<bool> UpdateStudentAsync(Student Student)
        {
            _dataContext.Students.Update(Student);
            return await Save();
        }
        private async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() >= 0 ? true : false;
        }
    }

}