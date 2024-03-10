﻿using ApplicationCore.DTOs.Request;
using ApplicationCore.DTOs.Response;
using ApplicationCore.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenRepository _authenRepository;
        public AuthController(IAuthenRepository authenRepository)
        {
            _authenRepository = authenRepository;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegUser(UserRegistrationDto userdto)
        {
            if (ModelState.IsValid)
            {
                var createuser = await _authenRepository.RegisterUser(userdto);
                if (createuser.Success)
                {
                    return Ok(new UserRegistrationResponse
                    {
                        Message = createuser.Message,
                        Success = createuser.Success
                    });

                }
                else
                {
                    return BadRequest(new UserRegistrationResponse
                    {
                        Message = createuser.Message,
                        Success = createuser.Success
                    });
                }

            }
            else
            {
                return BadRequest("Wronge data");
            }
        }


        [HttpGet("getallusers")]
        public async Task<IActionResult> allusers()
        {
            var users = await _authenRepository.getAllUsers();
            return Ok(users);
                
        }
    }
}
