using API.Entities;
using API.Payload.Global;
using API.Payload.Search;

namespace API.Repository.Interface{
    public interface IStudentRepository{
        Task<ICollection<Student>> GetAllStudentsAsync();
        Task<ListWithPagination<ICollection<Student>>> GetStudentsAsync(SearchParams searchParams);
        Task<ICollection<Student>> GetDeletedStudentsAsync();
        Task<Student> GetStudentByIDAsync(int StudentId);
        Task<bool> StudentExistAsync(int StudentId);
        Task<bool> CreateStudentAsync(Student Student);
        Task<bool> UpdateStudentAsync(Student Student);
        Task<bool> SoftDeleteStudentAsync(int StudentId);
        Task<bool> HardDeleteStudentAsync(Student Student);
    }

}