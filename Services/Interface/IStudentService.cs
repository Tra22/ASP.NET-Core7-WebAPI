using API.Dtos;
using API.Payload;
using API.Payload.Global;
using API.Payload.Search;

namespace API.Services{
    public interface IStudentService
    {
        Task<Response<ListWithPagination<List<StudentDto>>>> GetStudentsAsync(SearchParams searchParams);
        Task<Response<StudentDto>> GetByIdAsync(int StudentId);
        Task<Response<StudentDto>> AddStudentAsync(CreateStudentDto createStudentDto);        
        Task<Response<StudentDto>> UpdateStudentAsync(UpdateStudentDto updateStudentDto);
        Task<Response<string>> SoftDeleteStudentAsync(int StudentId);

    }
}