using ApplicationCore.DTOs.Request;
using ApplicationCore.DTOs.Response;
using ApplicationCore.Services.Interface;
using AutoMapper;
using Infrastructure.Accounts;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services.Repository
{
    public class AuthenRepository : IAuthenRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserReg> _userManager;//to create user
        private readonly SignInManager<UserReg> _signInManager;//  to signin user
        private readonly RoleManager<UserRoles> _roleManager;
        private readonly IMapper _Mapper;
        public AuthenRepository(AppDbContext context, UserManager<UserReg> userManager,
           SignInManager<UserReg> signInManager, RoleManager<UserRoles> roleManager, IMapper Mapper
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager=signInManager;   
            _roleManager=roleManager;
            _Mapper =   Mapper;

        }
        public async Task<IEnumerable<UserRegistrationDto>> getAllUsers()
        {
            var users = await (from user in _userManager.Users
                               join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                               join role in _roleManager.Roles on userRoles.RoleId equals role.Id
                               select new UserRegistrationDto
                               {
                                   UserId = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   PhoneNumber = user.PhoneNumber,
                                   RoleName = _roleManager.Roles.Where(x => x.Id == userRoles.RoleId).Select(x => x.Name).ToList()

                               }

                                  ).ToListAsync();
          
            return (users);
        }

        public async Task<UserRegistrationResponse> RegisterUser(UserRegistrationDto users)
        {
            var newuser = _Mapper.Map<UserReg>(users);
            newuser.NormalizedUserName = users.UserName;
            newuser.NormalizedEmail = users.Email;
            var RS = new UserRegistrationResponse();

            var existed = await _userManager.FindByEmailAsync(users.Email);
            if(existed == null)
            {
                var isCreated = await _userManager.CreateAsync(newuser, users.Password);
                if(isCreated.Succeeded)
                {
                    var roleCheck = await _roleManager.RoleExistsAsync(newuser.UserRole);

                    var role = new UserRoles();
                    role.Name = newuser.UserRole;

                    if(!roleCheck)
                    {
                        await _roleManager.CreateAsync(role);
                    }

                    await _userManager.AddToRoleAsync(newuser, newuser.UserRole);

                    RS.Message = "User created successfully";
                    RS.Success = true;
                }
                else
                {
                    RS.Errors = isCreated.Errors.Select(x => x.Description).ToList();
                    RS.Success = false;
                }
            }
            else
            {
                RS.Errors = new List<string>()
                    {
                        "User already Exist"
                    };
                RS.Message = "User already in use";
                RS.Success = false;
            }



            return RS;
        }
    }
}
