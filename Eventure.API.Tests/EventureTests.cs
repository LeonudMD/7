using System.Text;
using AuthService.Application.Contracts;
using System.Text.Json;

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
    public async Task GetTickets_TicketsCreated_ReturnsOk()
    {
        // Arrange
        var registerRequest = new RegisterUserRequest
        (
            "testuser",
            "Password123!",
            "testuser@example.com"
        );
        
        var content = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync("/api/users/register", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}