using System.ComponentModel.DataAnnotations;

namespace API.Dtos{
    public class UpdateStudentDto{
        public int id {get;set;}
        [Required]
        [StringLength(200, MinimumLength =2, ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long.")]
        public required string FirstName {get;set;}
        [Required]
        [StringLength(200, MinimumLength =2, ErrorMessage ="The {0} must be at lease {2} and at max {1} characters long.")]
        public required string LastName {get;set;}
        public bool IsDeleted {get;set;} = true;
    }
}