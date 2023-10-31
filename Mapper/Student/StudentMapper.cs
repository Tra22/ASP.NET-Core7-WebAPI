using API.Dtos;
using API.Entities;
using AutoMapper;

namespace API.Mapper{
    public class StudentMapper : Profile{
        public StudentMapper()
        {
            CreateMap <Student, StudentDto> ();
        }
    }
}