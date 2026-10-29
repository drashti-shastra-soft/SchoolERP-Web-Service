using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Utilities
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT for the authenticated user.
        /// Claims aligned with TDD 12.7 UserSessionModel.
        /// </summary>
        public string GenerateToken(string username, string roleName, int userId, int userTypeId, int defaultRoleId, string? userTypeName = null,int? staffId=null, StudentRoleContextDto? studentRoleContextDto=null)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SchoolERP_Default_Key_1234567890"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,       username),
                new Claim(ClaimTypes.Role,       roleName),
                // Used by _Layout sidebar permission loader
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("UserId",              userId.ToString()),                
                new Claim("UserTypeId",          userTypeId.ToString()),
                new Claim("DefaultRoleId",       defaultRoleId.ToString()),
                new Claim("RoleId",       defaultRoleId.ToString()),
                new Claim("StaffID",              staffId.ToString()),
            };

            if (!string.IsNullOrWhiteSpace(userTypeName))
            {
                claims.Add(new Claim("UserTypeName", userTypeName));
            }

            if(studentRoleContextDto != null) 
            {
                if (studentRoleContextDto.StudentID.HasValue)
                    claims.Add(new Claim("StudentID", studentRoleContextDto.StudentID.Value.ToString()));

                if (studentRoleContextDto.ClassID.HasValue)
                    claims.Add(new Claim("ClassID", studentRoleContextDto.ClassID.Value.ToString()));

                if (studentRoleContextDto.SectionID.HasValue)
                    claims.Add(new Claim("SectionID", studentRoleContextDto.SectionID.Value.ToString()));

                if (studentRoleContextDto.CompanyID.HasValue)
                    claims.Add(new Claim("CompanyID", studentRoleContextDto.CompanyID.Value.ToString()));

                if (studentRoleContextDto.SessionID.HasValue)
                    claims.Add(new Claim("SessionID", studentRoleContextDto.SessionID.Value.ToString()));
                
                if (studentRoleContextDto?.StudentPhoto !=null)
                    claims.Add(new Claim("StudentPhoto", studentRoleContextDto.StudentPhoto.ToString()));
            }
           

            var token = new JwtSecurityToken(
                issuer:            _configuration["Jwt:Issuer"],
                audience:          _configuration["Jwt:Audience"],
                claims:            claims,
                expires:           DateTime.UtcNow.AddMinutes(
                                       Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
