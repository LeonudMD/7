using System.Text;
using AuthService.Application.Contracts;
using System.Text.Json;
using AuthService.Domain.Models;
using Eventure.API.Tests.Factory;

namespace Eventure.API.Tests;

public class EventureTests
{
    private readonly HttpClient _httpClient;

    public EventureTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
    }
    
    
    [Fact]
    public async Task RegisterUser_NewUser_ReturnsOk()
    {
        // Arrange
        var registerRequest = EventureFactory.GetRegisterUserRequest();
        
        var content = new StringContent(JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("/auth/register", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task LoginUser_RegisteredUser_ReturnsOk()
    {
        // Arrange
        var loginRequest = EventureFactory.GetLoginUserRequest();
        
        var registerRequest = EventureFactory.GetRegisterUserRequest();
        
        var contentForReg = new StringContent(JsonSerializer.Serialize(registerRequest),
            Encoding.UTF8,
            "application/json");
        
        var contentForLog = new StringContent(JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");
        
        await _httpClient.PostAsync("/auth/register", contentForReg);

        // Act
        var responseLog = await _httpClient.PostAsync("/auth/login", contentForLog);

        // Assert
        responseLog.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetAllUsers_ReturnsUsersList()
    {
        // Arrange
        var registerRequests = EventureFactory.GetRegisterUsersRequest();

        await _httpClient.PostAsync("/auth/register",
            new StringContent(JsonSerializer.Serialize(registerRequests[0]), 
            Encoding.UTF8,
            "application/json"));
        await _httpClient.PostAsync("/auth/register",
            new StringContent(JsonSerializer.Serialize(registerRequests[1]), 
            Encoding.UTF8,
            "application/json"));

        // Act
        var response = await _httpClient.GetAsync("/auth");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = JsonSerializer.Deserialize<List<User>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(users);
        Assert.True(users.Count >= 2);
    }
    
    /*[Fact]
    public async Task RefreshToken_ValidTokens_ReturnsNewTokens()
    {
        try
        {
            // Arrange
            var registerRequest = new RegisterUserRequest("testuser", "Password123!", "testuser@example.com");
            await _httpClient.PostAsync("/auth/register", new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

            var loginRequest = new LoginUserRequest("testuser@example.com", "Password123!");
            var loginResponse = await _httpClient.PostAsync("/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

            loginResponse.EnsureSuccessStatusCode();
            var loginContent = JsonSerializer.Deserialize<Dictionary<string, string>>(await loginResponse.Content.ReadAsStringAsync());
            Assert.NotNull(loginContent);
        
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"tasty-cookies={loginContent["AccessToken"]}; refresh-cookies={loginContent["RefreshToken"]}");

            // Act
            var response = await _httpClient.PostAsync("/auth/refresh-token", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(tokens);
            Assert.NotEmpty(tokens["AccessToken"]);
            Assert.NotEmpty(tokens["RefreshToken"]);
        }
        catch (Exception ex)
        {
            Assert.Null(ex);
        }
    }*/
    
    [Fact]
    public async Task LogoutUser_DeletesTokens()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest("testuser", "Password123!", "testuser@example.com");
        await _httpClient.PostAsync("/auth/register", new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

        var loginRequest = new LoginUserRequest("testuser@example.com", "Password123!");
        var loginResponse = await _httpClient.PostAsync("/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

        loginResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _httpClient.PostAsync("/auth/logout", null);

        // Assert
        response.EnsureSuccessStatusCode();
    }


}