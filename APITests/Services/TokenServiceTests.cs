using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.Entities;
using API.Services;

namespace API.Tests.Services
{
    [TestClass]
    public class TokenServiceTests
    {
        private TokenService _tokenService = null!;
        private IConfiguration _configuration = null!;
        private const string ValidTokenKey = "super_secret_test_key_that_is_at_least_64_characters_long_for_testing_purposes_1234567890";

        [TestInitialize]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "TokenKey", ValidTokenKey }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(_configuration);
        }

        [TestMethod]
        public void CreateToken_WithValidUser_ReturnsValidJwtToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsTrue(token.Length > 0);
            Assert.IsTrue(IsValidJwtFormat(token));
        }

        [TestMethod]
        public void CreateToken_ContainsCorrectClaims()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser123",
                KnownAs = "Test User",
                Gender = "female",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1985-05-15"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var nameIdentifierClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            Assert.IsNotNull(nameIdentifierClaim);
            Assert.AreEqual("testuser123", nameIdentifierClaim.Value);
        }

        [TestMethod]
        public void CreateToken_HasCorrectExpiration()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };
            var beforeCreation = DateTime.UtcNow;

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var expectedExpiration = beforeCreation.AddDays(7);
            var actualExpiration = jwtToken.ValidTo;
            
            // Allow for a small time difference (1 minute) due to execution time
            var timeDifference = Math.Abs((expectedExpiration - actualExpiration).TotalMinutes);
            Assert.IsTrue(timeDifference < 1, $"Token expiration should be 7 days from creation. Expected: {expectedExpiration}, Actual: {actualExpiration}");
        }

        [TestMethod]
        public void CreateToken_IsSignedWithCorrectKey()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ValidTokenKey));
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Don't validate expiration for this test
            };

            // This should not throw if the signature is valid
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            
            Assert.IsNotNull(principal);
            Assert.IsNotNull(validatedToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateToken_WithMissingTokenKey_ThrowsArgumentNullException()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder().Build();
            var tokenService = new TokenService(emptyConfig);
            
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            tokenService.CreateToken(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateToken_WithShortTokenKey_ThrowsArgumentException()
        {
            // Arrange
            var shortKeyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { { "TokenKey", "short_key" } })
                .Build();
            var tokenService = new TokenService(shortKeyConfig);
            
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            tokenService.CreateToken(user);
        }

        [TestMethod]
        public void CreateToken_WithDifferentUsers_GeneratesDifferentTokens()
        {
            // Arrange
            var user1 = new AppUser
            {
                Id = 1,
                UserName = "user1",
                KnownAs = "User One",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            var user2 = new AppUser
            {
                Id = 2,
                UserName = "user2",
                KnownAs = "User Two",
                Gender = "female",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1985-05-15"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token1 = _tokenService.CreateToken(user1);
            var token2 = _tokenService.CreateToken(user2);

            // Assert
            Assert.AreNotEqual(token1, token2);
            
            // Verify both tokens contain different usernames
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken1 = tokenHandler.ReadJwtToken(token1);
            var jwtToken2 = tokenHandler.ReadJwtToken(token2);
            
            var claim1 = jwtToken1.Claims.First(c => c.Type == "nameid");
            var claim2 = jwtToken2.Claims.First(c => c.Type == "nameid");

            Assert.AreEqual("user1", claim1.Value);
            Assert.AreEqual("user2", claim2.Value);
        }

        [TestMethod]
        public void CreateToken_WithSameUser_CalledMultipleTimes_GeneratesDifferentTokens()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token1 = _tokenService.CreateToken(user);
            Thread.Sleep(1100); // Sleep for over 1 second to ensure different Unix timestamps
            var token2 = _tokenService.CreateToken(user);

            // Assert
            Assert.AreNotEqual(token1, token2, "Tokens should be different even for the same user due to different creation times");
        }

        [TestMethod]
        public void CreateToken_UsesHmacSha512Algorithm()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            Assert.AreEqual("HS512", jwtToken.Header.Alg);
        }

        [TestMethod]
        public void CreateToken_WithNullUser_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<NullReferenceException>(() => _tokenService.CreateToken(null!));
        }

        [TestMethod]
        public void CreateToken_WithUserHavingNullUserName_ThrowsException()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = null!, // Intentionally set to null
                KnownAs = "Test User",
                Gender = "male",
                IsPredator = false,
                DateOfBirth = DateOnly.Parse("1990-01-01"),
                City = "Test City",
                Country = "Test Country"
            };

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _tokenService.CreateToken(user));
        }

        private static bool IsValidJwtFormat(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var parts = token.Split('.');
            return parts.Length == 3; // JWT has 3 parts: header.payload.signature
        }
    }
}
