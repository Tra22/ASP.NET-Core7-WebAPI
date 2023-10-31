using API.Dtos;
using API.Entities;
using API.Payload;
using API.Payload.Global;
using API.Payload.Search;
using API.Repository.Interface;
using AutoMapper;

namespace API.Services{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            this._studentRepo = studentRepository;
            this._mapper = mapper;
        }
        public async Task<Response<StudentDto>> AddStudentAsync(CreateStudentDto createStudentDto)
        {
            Response<StudentDto> _response = new();
            try
            {
                Student _newStudent = new()
                {

                    FirstName = createStudentDto.FirstName,
                    LastName = createStudentDto.LastName,
                    IsDeleted = false
                };

                //Add new record
                if (!await _studentRepo.CreateStudentAsync(_newStudent))
                {
                    _response.Error = "RepoError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<StudentDto>(_newStudent);
                _response.Message = "Created";

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };

            }
            return _response;
        }

        public async Task<Response<StudentDto>> GetByIdAsync(int StudentId)
        {
            Response<StudentDto> _response = new();

            try
            {

                var _Student = await _studentRepo.GetStudentByIDAsync(StudentId);

                if (_Student == null)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    return _response;
                }

                var _CompanyDto = _mapper.Map<StudentDto>(_Student);

                _response.Success = true;
                _response.Message = "ok";
                _response.Data = _CompanyDto;


            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<Response<ListWithPagination<List<StudentDto>>>> GetStudentsAsync(SearchParams searchParams)
        {
             Response<ListWithPagination<List<StudentDto>>> _response = new();

            try
            {

                var StudentList = await _studentRepo.GetStudentsAsync(searchParams);

                var StudentListDto = new List<StudentDto>();
                if(StudentList.List != null){
                    foreach (var item in StudentList.List)
                    {
                        StudentListDto.Add(_mapper.Map<StudentDto>(item));
                    }
                }
                _response.Success = true;
                _response.Message = "ok";
                _response.Data = new ListWithPagination<List<StudentDto>>{ List = StudentListDto, Pagination = StudentList.Pagination};

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<Response<string>> SoftDeleteStudentAsync(int StudentId)
        {
             Response<string> _response = new();

            try
            {
                //check if record exist
                var _existingStudent = await _studentRepo.StudentExistAsync(StudentId);

                if (_existingStudent == false)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    _response.Data = null;
                    return _response;

                }

                if (!await _studentRepo.SoftDeleteStudentAsync(StudentId))
                {
                    _response.Success = false;
                    _response.Message = "RepoError";
                    return _response;
                }



                _response.Success = true;
                _response.Message = "SoftDeleted";

            }
            catch (Exception ex)
            {

                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }
            return _response;
        }

        public async Task<Response<StudentDto>> UpdateStudentAsync(UpdateStudentDto updateStudentDto)
        {
             Response<StudentDto> _response = new();

            try
            {
                //check if record exist
                var _existingStudent = await _studentRepo.GetStudentByIDAsync(updateStudentDto.id);

                if (_existingStudent == null)
                {
                    _response.Success = false;
                    _response.Message = "NotFound";
                    _response.Data = null;
                    return _response;

                }

                //Update
                _existingStudent.FirstName = updateStudentDto.FirstName;
                _existingStudent.LastName = updateStudentDto.LastName;
                _existingStudent.IsDeleted = updateStudentDto.IsDeleted;

                if (!await _studentRepo.UpdateStudentAsync(_existingStudent))
                {
                    _response.Success = false;
                    _response.Message = "RepoError";
                    _response.Data = null;
                    return _response;
                }

                var _companyDto = _mapper.Map<StudentDto>(_existingStudent);
                _response.Success = true;
                _response.Message = "Updated";
                _response.Data = _companyDto;

            }
            catch (Exception ex)
            {

                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }
            return _response;
        }
    }
}