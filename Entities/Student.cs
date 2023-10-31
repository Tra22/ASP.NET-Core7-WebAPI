using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
     public class Student{
        [Key]
        public int id {get;set;}
        [Required]
        [StringLength(200, MinimumLength =2, ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long.")]
        public string? FirstName {get;set;}
        [Required]
        [StringLength(200, MinimumLength =2, ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long.")]
        public string? LastName {get;set;}
        public bool IsDeleted {get;set;} = true;

        public static implicit operator Student(Task<List<Student>> v)
        {
            throw new NotImplementedException();
        }
    }
}