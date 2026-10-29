// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVaultAPI.Data;
using ArcaneVaultAPI.DTOs;
using ArcaneVaultAPI.Helpers;
using ArcaneVaultAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ArcaneVaultAPI.Controllers
{
    /// <summary>
    /// Handles user account operations including registration, login, and CRUD.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all non-deleted users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.ArcaneVaultUsers
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDto
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    IsDeleted = u.IsDeleted,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.RoleName : ""
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Gets a specific user by username.
        /// </summary>
        /// <param name="userName">The username to look up.</param>
        [HttpGet("{userName}")]
        public async Task<ActionResult<UserDto>> GetUser(string userName)
        {
            var user = await _context.ArcaneVaultUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null || user.IsDeleted)
                return NotFound(new ErrorResponseDto { Message = $"User '{userName}' not found." });

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                IsDeleted = user.IsDeleted,
                RoleId = user.RoleId,
                RoleName = user.Role?.RoleName ?? ""
            });
        }

        /// <summary>
        /// Registers a new user account with PBKDF2 password hashing.
        /// </summary>
        /// <param name="dto">Registration details.</param>
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(dto.UserName))
                    errors.Add("Username is required.");
                if (string.IsNullOrWhiteSpace(dto.Email))
                    errors.Add("Email is required.");
                if (string.IsNullOrWhiteSpace(dto.Password))
                    errors.Add("Password is required.");
                if (dto.Password != dto.ConfirmPassword)
                    errors.Add("Passwords do not match.");

                // Validate email format
                if (!string.IsNullOrWhiteSpace(dto.Email) && !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(dto.Email))
                    errors.Add("Invalid email format.");

                if (errors.Count > 0)
                    return BadRequest(new ErrorResponseDto { Message = "Validation failed.", Errors = errors });

                // Check for duplicate username (case-insensitive)
                var existingUser = await _context.ArcaneVaultUsers
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.UserName.ToLower());
                if (existingUser != null)
                    return BadRequest(new ErrorResponseDto { Message = "Username already exists." });

                // Check for duplicate email (case-insensitive)
                var existingEmail = await _context.ArcaneVaultUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
                if (existingEmail != null)
                    return BadRequest(new ErrorResponseDto { Message = "Email already registered." });

                var (hash, salt) = PasswordHelper.HashPassword(dto.Password);

                var user = new ArcaneVaultUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    IsDeleted = false,
                    RoleId = dto.RoleId,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };

                _context.ArcaneVaultUsers.Add(user);
                await _context.SaveChangesAsync();

                var role = await _context.ArcaneVaultUserRoles.FindAsync(user.RoleId);

                return CreatedAtAction(nameof(GetUser), new { userName = user.UserName }, new UserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    IsDeleted = user.IsDeleted,
                    RoleId = user.RoleId,
                    RoleName = role?.RoleName ?? ""
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred during registration.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Authenticates a user and returns username + role. Enforces account lockout after 5 failed attempts.
        /// </summary>
        /// <param name="dto">Login credentials.</param>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest(new ErrorResponseDto { Message = "Username and password are required." });

                var user = await _context.ArcaneVaultUsers
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == dto.UserName.ToLower());

                if (user == null || user.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = "Invalid username or password." });

                // Check for account lockout
                var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);
                var recentFailedAttempts = await _context.LoginAttempts
                    .Where(a => a.UserName == user.UserName && !a.Successful && a.AttemptedAt >= tenMinutesAgo)
                    .CountAsync();

                if (recentFailedAttempts >= 5)
                    return BadRequest(new ErrorResponseDto { Message = "Account is locked due to too many failed login attempts. Please try again in 10 minutes." });

                bool isValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);

                // Record the login attempt
                _context.LoginAttempts.Add(new LoginAttempt
                {
                    UserName = user.UserName,
                    AttemptedAt = DateTime.UtcNow,
                    Successful = isValid
                });
                await _context.SaveChangesAsync();

                if (!isValid)
                {
                    var remainingAttempts = 4 - recentFailedAttempts;
                    var msg = remainingAttempts > 0
                        ? $"Invalid username or password. {remainingAttempts} attempt(s) remaining before lockout."
                        : "Invalid username or password. Account is now locked for 10 minutes.";
                    return BadRequest(new ErrorResponseDto { Message = msg });
                }

                return Ok(new LoginResponseDto
                {
                    UserName = user.UserName,
                    RoleName = user.Role?.RoleName ?? ""
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred during login.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Updates a user's details.
        /// </summary>
        /// <param name="userName">The username to update.</param>
        /// <param name="dto">Updated user data.</param>
        [HttpPut("{userName}")]
        public async Task<IActionResult> UpdateUser(string userName, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var user = await _context.ArcaneVaultUsers.FindAsync(userName);
                if (user == null || user.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = $"User '{userName}' not found." });

                // Check for duplicate email
                var existingEmail = await _context.ArcaneVaultUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.UserName != userName);
                if (existingEmail != null)
                    return BadRequest(new ErrorResponseDto { Message = "Email already registered to another user." });

                user.Email = dto.Email;
                user.RoleId = dto.RoleId;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Soft-deletes a user by setting IsDeleted to true.
        /// </summary>
        /// <param name="userName">The username to delete.</param>
        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            try
            {
                var user = await _context.ArcaneVaultUsers.FindAsync(userName);
                if (user == null || user.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = $"User '{userName}' not found." });

                user.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Checks if an email is already registered. Used for remote validation.
        /// </summary>
        /// <param name="dto">Email and optional current username to exclude.</param>
        [HttpPost("check-email")]
        public async Task<ActionResult<bool>> CheckEmail([FromBody] CheckEmailDto dto)
        {
            var exists = await _context.ArcaneVaultUsers
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()
                    && (dto.CurrentUserName == null || u.UserName != dto.CurrentUserName));
            return Ok(!exists); // Returns true if email is available
        }

        /// <summary>
        /// Requests a password reset token. The token is hashed and stored with a 15-minute expiry.
        /// </summary>
        /// <param name="dto">The email to send the reset token for.</param>
        [HttpPost("password-reset-request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            try
            {
                var user = await _context.ArcaneVaultUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower() && !u.IsDeleted);

                // Always return OK to prevent user enumeration
                if (user == null)
                    return Ok(new { Message = "If that email is registered, a reset link has been generated." });

                // Generate a secure token
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var rawToken = Convert.ToBase64String(tokenBytes);

                // Store hashed version
                var tokenHash = Convert.ToBase64String(
                    Rfc2898DeriveBytes.Pbkdf2(rawToken, new byte[16], 10000, HashAlgorithmName.SHA256, 32)
                );

                var resetToken = new PasswordResetToken
                {
                    UserName = user.UserName,
                    TokenHash = tokenHash,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // In production, send email. For this project, return the token.
                return Ok(new { Message = "Password reset token generated.", Token = rawToken, UserName = user.UserName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }

        /// <summary>
        /// Resets a password using a valid, unexpired reset token.
        /// </summary>
        /// <param name="dto">Token, username, and new password.</param>
        [HttpPost("password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            try
            {
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(dto.Token)) errors.Add("Token is required.");
                if (string.IsNullOrWhiteSpace(dto.UserName)) errors.Add("Username is required.");
                if (string.IsNullOrWhiteSpace(dto.NewPassword)) errors.Add("New password is required.");
                if (dto.NewPassword != dto.ConfirmPassword) errors.Add("Passwords do not match.");

                if (errors.Count > 0)
                    return BadRequest(new ErrorResponseDto { Message = "Validation failed.", Errors = errors });

                // Hash the provided token to compare
                var tokenHash = Convert.ToBase64String(
                    Rfc2898DeriveBytes.Pbkdf2(dto.Token, new byte[16], 10000, HashAlgorithmName.SHA256, 32)
                );

                var resetToken = await _context.PasswordResetTokens
                    .FirstOrDefaultAsync(t => t.TokenHash == tokenHash
                        && t.UserName == dto.UserName
                        && !t.IsUsed
                        && t.ExpiresAt > DateTime.UtcNow);

                if (resetToken == null)
                    return BadRequest(new ErrorResponseDto { Message = "Invalid or expired reset token." });

                var user = await _context.ArcaneVaultUsers.FindAsync(dto.UserName);
                if (user == null || user.IsDeleted)
                    return NotFound(new ErrorResponseDto { Message = "User not found." });

                var (hash, salt) = PasswordHelper.HashPassword(dto.NewPassword);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                // Invalidate the token
                resetToken.IsUsed = true;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Password has been reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Message = "An error occurred.", Errors = new List<string> { ex.Message } });
            }
        }
    }
}
