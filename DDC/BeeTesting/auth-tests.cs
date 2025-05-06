using Bunit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BeeTesting
{
    public class AuthTests : TestContext
    {
        public AuthTests()
        {
            // Add authentication and authorization services
            Services.AddAuthorizationCore();
        }

        [Fact]
        public void Auth_ShouldDisplayUserNameWhenAuthenticated()
        {
            // Arrange - set up authentication manually
            var username = "TestUser";
            
            // Create claims identity and principal
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "TestAuthentication");
            var user = new ClaimsPrincipal(identity);
            
            // Set up authentication state provider
            var authStateProvider = new TestAuthStateProvider(user);
            Services.AddSingleton<AuthenticationStateProvider>(authStateProvider);
            
            // Set up authorization service (always allow access)
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(true));
            
            // Act - render the component wrapped in CascadingAuthenticationState
            var cut = RenderComponent<CascadingAuthenticationState>(parameters => parameters
                .AddChildContent<ServerBee.Components.OldPages.Auth>());
            
            // Uncomment for debugging
            // Console.WriteLine(cut.Markup);
            
            // Assert - check the h1 content exists
            Assert.Contains("You are authenticated", cut.Markup);
            
            // Check for the username
            Assert.Contains($"Hello {username}", cut.Markup);
        }
        
        [Fact]
        public void Auth_ShouldHandleUnauthenticatedUser()
        {
            // Arrange - set up unauthenticated state
            var user = new ClaimsPrincipal(new ClaimsIdentity()); // No authentication
            
            // Set up authentication state provider
            var authStateProvider = new TestAuthStateProvider(user);
            Services.AddSingleton<AuthenticationStateProvider>(authStateProvider);
            
            // Add authorization service - this line was missing
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(false));
            
            // Act - try to render the component
            var cut = RenderComponent<CascadingAuthenticationState>(parameters => parameters
                .AddChildContent<ServerBee.Components.OldPages.Auth>());
            
            // Assert - check for authorization failed message or redirect indicator
            Assert.DoesNotContain("You are authenticated", cut.Markup);
            Assert.DoesNotContain("Hello", cut.Markup);
            // You might want to check for specific unauthorized content instead
            // if your component shows a specific message when unauthorized
        }
        
        [Fact]
        public void Auth_ShouldHaveCorrectHeading()
        {
            // Arrange - set up authenticated state
            var username = "TestUser";
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "TestAuthentication");
            var user = new ClaimsPrincipal(identity);
            
            // Set up authentication state provider
            var authStateProvider = new TestAuthStateProvider(user);
            Services.AddSingleton<AuthenticationStateProvider>(authStateProvider);
            
            // Set up authorization service (always allow access)
            Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService(true));
            
            // Act - render the component wrapped in CascadingAuthenticationState
            var cut = RenderComponent<CascadingAuthenticationState>(parameters => parameters
                .AddChildContent<ServerBee.Components.OldPages.Auth>());
            
            // Assert
            var heading = cut.Find("h1");
            Assert.Equal("You are authenticated", heading.TextContent);
        }
    }
    
    // Custom authentication state provider for testing
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
    
    // Helper class for authorization testing
    public class TestAuthorizationService : IAuthorizationService
    {
        private readonly bool _allowAccess;
        
        public TestAuthorizationService(bool allowAccess)
        {
            _allowAccess = allowAccess;
        }
        
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(_allowAccess ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }
        
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(_allowAccess ? AuthorizationResult.Success() : AuthorizationResult.Failed());
        }
    }
}