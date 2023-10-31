using API.Dtos;
using API.Payload.Search;
using API.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StudentController : ControllerBase
    {
         private readonly IStudentService _studentService;
         private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            this._studentService = studentService;
            this._logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StudentDto>))]
        public async Task<IActionResult> GetAll([FromQuery] SearchParams searchParams)
        {
            var students = await _studentService.GetStudentsAsync(searchParams);
            _logger.LogInformation("Success {@StudentList}", students);
            return Ok(students);
        }

        [HttpGet("{StudentId:int}", Name = "GetBystudentID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<StudentDto>> GetBystudentID(int StudentId)
        {
            if (StudentId <= 0)
            {
                return BadRequest(StudentId);
            }
            var studentFound = await _studentService.GetByIdAsync(StudentId);

            if (studentFound.Data == null)
            {
                _logger.LogWarning("Not Found {StudentId}", StudentId);
                return NotFound();
            }
            _logger.LogInformation("Success {@Response}", studentFound);
            return Ok(studentFound);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> Createstudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null)
            {
                _logger.LogWarning("Empty Request Body {@CreateStudentObj}", createStudentDto);
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var _newStudent = await _studentService.AddStudentAsync(createStudentDto);


            if (_newStudent.Success == false && _newStudent.Message == "RepoError")
            {
                _logger.LogError("RepoError {@CreateStudentObj}",  $"Some thing went wrong in respository layer when adding student {createStudentDto}");
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding student {createStudentDto}");
                return StatusCode(500, ModelState);
            }

            if (_newStudent.Success == false && _newStudent.Message == "Error")
            {
                _logger.LogError("Error {@CreateStudentObj}",  $"Some thing went wrong in service layer when adding student {createStudentDto}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding student {createStudentDto}");
                return StatusCode(500, ModelState);
            }
            _logger.LogInformation("Success {@Student}", _newStudent);
            //Return new student created
            return CreatedAtRoute("GetBystudentID", new { StudentId = _newStudent?.Data?.id }, _newStudent);

        }
        [HttpPatch("{StudentId:int}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Updatestudent(int StudentId, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (updateStudentDto == null || updateStudentDto.id != StudentId)
            {
                _logger.LogWarning("Empty Request Body or Empty Id {}", updateStudentDto);
                return BadRequest(ModelState);
            }


            var _updateStudent = await _studentService.UpdateStudentAsync(updateStudentDto);

            if (_updateStudent.Success == false && _updateStudent.Message == "NotFound")
            {
                _logger.LogWarning("Not Found: {@UpdateStudentObj}", updateStudentDto);
                return Ok(_updateStudent);
            }

            if (_updateStudent.Success == false && _updateStudent.Message == "RepoError")
            {
                _logger.LogError("RepoError {@UpdateStudentObj}", $"Some thing went wrong in respository layer when updating student {updateStudentDto}");
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating student {updateStudentDto}");
                return StatusCode(500, ModelState);
            }

            if (_updateStudent.Success == false && _updateStudent.Message == "Error")
            {
                _logger.LogError("Error {@UpdateStudentObj}",  $"Some thing went wrong in service layer when updating student {updateStudentDto}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating student {updateStudentDto}");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Success {@Response}", _updateStudent);
            return Ok(_updateStudent);
        }
        [HttpDelete("{StudentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] //Not found
        [ProducesResponseType(StatusCodes.Status409Conflict)] //Can not be removed 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletestudent(int StudentId)
        {

            var _deletestudent = await _studentService.SoftDeleteStudentAsync(StudentId);


            if (_deletestudent.Success == false && _deletestudent.Data == "NotFound")
            {
                _logger.LogWarning("Not Found {@DeleteStudent}", _deletestudent);
                ModelState.AddModelError("", "student Not found");
                return StatusCode(404, ModelState);
            }

            if (_deletestudent.Success == false && _deletestudent.Data == "RepoError")
            {
                _logger.LogError("RepoError {@DeleteStudent}", $"Some thing went wrong in Repository when deleting student {_deletestudent}");
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting student");
                return StatusCode(500, ModelState);
            }

            if (_deletestudent.Success == false && _deletestudent.Data == "Error")
            {
                _logger.LogError("RepoError {@DeleteStudent}", $"Some thing went wrong in service layer when deleting student {_deletestudent}");
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting student");
                return StatusCode(500, ModelState);
            }
            _logger.LogInformation("Success {Msg}", "deleted");
            return NoContent();

        }
        
    }
}