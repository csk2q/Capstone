using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace ServerBee.Tests
{
    public class AuthTests : TestContext
    {
        public AuthTests()
        {
            // Default test context setup
        }
        
        [Fact]
        public void Auth_ShouldDisplayUserNameWhenAuthenticated()
        {
            // Arrange
            var username = "TestUser";
            
            // Set up authentication state with a test user
            var authContext = new TestAuthorizationContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }, "TestAuthentication"))
            };
            
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(true));
            Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(authContext.User));
            
            // Act
            var cut = RenderComponent<ServerBee.Auth>();
            
            // Assert
            Assert.Contains("You are authenticated", cut.Markup);
            Assert.Contains($"Hello {username}", cut.Markup);
        }
        
        [Fact]
        public void Auth_ShouldRedirectWhenNotAuthenticated()
        {
            // Arrange - set up unauthenticated user
            var authContext = new TestAuthorizationContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())  // No claims = not authenticated
            };
            
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(false));
            Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(authContext.User));
            
            // Act & Assert - component should throw an exception due to authorization failure
            var exception = Assert.Throws<InvalidOperationException>(() => RenderComponent<ServerBee.Auth>());
            Assert.Contains("Authorization failed", exception.Message);
        }
        
        [Fact]
        public void Auth_ShouldHandleNullUserIdentity()
        {
            // Arrange - authenticated user but with null identity name
            var authContext = new TestAuthorizationContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[0], "TestAuthentication"))
            };
            
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(true));
            Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(authContext.User));
            
            // Act
            var cut = RenderComponent<ServerBee.Auth>();
            
            // Assert
            Assert.Contains("You are authenticated", cut.Markup);
            Assert.Contains("Hello ", cut.Markup); // Should show "Hello " without a name
        }
        
        [Fact]
        public void Auth_ShouldHaveCorrectPageTitle()
        {
            // Arrange - authenticated user
            var authContext = new TestAuthorizationContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser")
                }, "TestAuthentication"))
            };
            
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(true));
            Services.AddSingleton<AuthenticationStateProvider>(new TestAuthStateProvider(authContext.User));
            
            // Act
            var cut = RenderComponent<ServerBee.Auth>();
            
            // Assert
            var pageTitle = cut.Find("PageTitle");
            Assert.Equal("Auth", pageTitle.TextContent);
        }
    }
    
    // Helper classes for authentication testing (reuse from previous tests)
    
    public class TestAuthorizationContext
    {
        public ClaimsPrincipal User { get; set; }
    }
    
    public class TestAuthorizationService : IAuthorizationService
    {
        private readonly bool _allowAccess;
        
        public TestAuthorizationService(bool allowAccess)
        {
            _allowAccess = allowAccess;
        }
        
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(_allowAccess ? AuthorizationResult.Success() : AuthorizationResult.Failed(new[] { new AuthorizationFailure { Message = "Authorization failed" } }));
        }
        
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            return Task.FromResult(_allowAccess ? AuthorizationResult.Success() : AuthorizationResult.Failed(new[] { new AuthorizationFailure { Message = "Authorization failed" } }));
        }
    }
    
    public class TestAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal _user;
        
        public TestAuthStateProvider(ClaimsPrincipal user)
        {
            _user = user;
        }
        
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_user));
        }
    }

    // Added this class to support the test
    public class AuthorizationFailure : IAuthorizationRequirement
    {
        public string Message { get; set; }
    }
}
