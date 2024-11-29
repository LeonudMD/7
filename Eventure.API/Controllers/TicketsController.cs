using Eventure.Domain.Absctrations;
using Eventure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Eventure.Application.Contracts;
using FluentValidation;
using Eventure.Application.Validators;
using FluentValidation.Results;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;


namespace Eventure.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<TicketsController> _logger;
        private readonly IMapper _mapper;
        private readonly TicketRequestValidator _validator;
        private readonly ITicketService _ticketService;
        public TicketsController(ITicketService ticketsService, TicketRequestValidator validator, IMapper mapper, IDistributedCache distributedCache, ILogger<TicketsController> logger)
        {
            _ticketService = ticketsService;
            _validator = validator;
            _mapper = mapper;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<List<TicketResponse>>> GetTickets()
        {
            string cacheKey = "allTickets";

            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            //Возврат билетов из кэша
            if (cachedData != null) 
            {
                var responseCached = JsonSerializer.Deserialize<CachedTickets>(cachedData);
                if (responseCached != null && !responseCached.IsStale)
                {
                    _logger.LogInformation("Билеты были получены из кэша");
                    return Ok(responseCached?.Tickets);
                }
            }

            var tickets = await _ticketService.GetAllTickets();

            var response = tickets.Select(
                _mapper.Map<TicketResponse>).ToList();

            var newCachedTickets = new CachedTickets()
            {
                Tickets = response,
                IsStale = false //актуальные (не устаревшие)
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(newCachedTickets), cacheOptions);

            _logger.LogInformation("Обновленные билеты добавлены в кэш");

            return Ok(response);
        }


        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<TicketResponse>> GetTicket(int id)
        {
            string cacheKey = $"Ticket_{id}";

            var cachedData = await _distributedCache.GetStringAsync(cacheKey);

            //Возврат билетов из кэша
            if (cachedData != null)
            {
                var TicketCached = JsonSerializer.Deserialize<CachedTicket>(cachedData);
                if(TicketCached != null && !TicketCached.IsStale)
                {
                    _logger.LogInformation($"Билет {id} был получен из кэша");
                    return Ok(TicketCached?.Ticket);
                }
            }

            var t = await _ticketService.GetTicketById(id);

            var response = _mapper.Map<TicketResponse>(t);

            var newCachedTicket = new CachedTicket()
            {
                Ticket = response,
                IsStale = false //актуальные (не устаревшие)
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(newCachedTicket), cacheOptions);

            _logger.LogInformation($"Обновленный билет {id} добавлен в кэш");

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> CreateTicket([FromBody] TicketRequest request)
        {
            // Асинхронная валидация и выбрасывание исключения при невалидных данных
            await _validator.ValidateOrThrowAsync(request);

            var ticket = _mapper.Map<Ticket>(request);
            var id = await _ticketService.CreateTicket(ticket);
            await MarkCacheAsStale();
            await MarkTicketCacheAsStale(id);
            return Ok(id);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> UpdateTicket(int id, [FromBody] TicketRequest request)
        {
            await _validator.ValidateOrThrowAsync(request);
            var ticketId = await _ticketService.UpdateTicket(id, request.name, request.location, request.date, request.freeSeats, request.price);
            await MarkCacheAsStale();
            await MarkTicketCacheAsStale(id);
            return Ok(ticketId);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> DeleteTicket(int id)
        {
            var ticketId = await _ticketService.DeleteTicket(id);
            await MarkCacheAsStale();
            await MarkTicketCacheAsStale(id);
            return Ok(ticketId);
        }

        private async Task MarkCacheAsStale()
        {
            string cacheKey = "allTickets";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                var responseCached = JsonSerializer.Deserialize<CachedTickets>(cachedData);
                if (responseCached != null)
                {
                    responseCached.IsStale = true; // Помечаем как устаревший
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(responseCached)); // Обновляем кэш
                }
            }
        }

        private async Task MarkTicketCacheAsStale(int id)
        {
            string cacheKey = $"Ticket_{id}";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                var ticketCached = JsonSerializer.Deserialize<CachedTicket>(cachedData);
                if (ticketCached != null)
                {
                    ticketCached.IsStale = true; // Помечаем как устаревший
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ticketCached)); // Обновляем кэш
                }
            }
        }
    }
}
