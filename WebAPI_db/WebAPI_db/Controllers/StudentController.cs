﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI_db.Models;
using WebAPI_db.Services;

namespace WebAPI_db.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [Authorize]
        [HttpGet("all")]
        public IActionResult QueryAll()
        {
            List<Student> students = StudentService.Instance.GetAllStudent();
            return Ok(students);
        }

        [AllowAnonymous]
        [HttpGet("signup")]
        public IActionResult Signup([FromBody] Student student)
        {
            string? errMsg = StudentService.Instance.Create(student);
            if (errMsg == null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }

        [Authorize]
        [HttpGet("info/{id}")]
        public IActionResult QueryByAccount([FromRoute] int id)
        {
            Student? student = StudentService.Instance.FindStudentById(id);
            return Ok(student);
        }

        [Authorize]
        [HttpGet("info")]
        public IActionResult QueryByName([FromQuery] string name)
        {
            List<Student> students = StudentService.Instance.FindStudentByName(name);
            return Ok(students);
        }

        [Authorize]
        [HttpPost("info")]
        public IActionResult Create([FromBody] Student student)
        {
            string? errMsg = StudentService.Instance.Create(student);
            if (errMsg ==  null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }

        [Authorize]
        [HttpPut("info/{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Student student)
        {
            string? errMsg = StudentService.Instance.Update(id, student);
            if (errMsg == null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }

        [Authorize]
        [HttpDelete("info/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            string? errMsg = StudentService.Instance.Delete(id);
            if (errMsg == null)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest(errMsg);
            }
        }
    }
}
