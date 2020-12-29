using Microsoft.AspNetCore.Mvc;
using MissionsApi.Dtos;
using MissionsApi.Services;
using System;
using System.Threading.Tasks;

namespace MissionsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IAgentRepository _agentRepository;

        public MissionsController(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello, agent";
        }

        [Route("Mission")]
        [HttpPost]
        public async Task<Guid> Mission(Mission mission)
        {
            return await _agentRepository.SaveMissionAsync(mission);
        }

        [Route("CountriesByIsolation")]
        public async Task<string> GetCountriesByIsolation()
        {
            return await _agentRepository.GetMostIsolatedCountry();
        }

        [Route("FindClosest")]
        [HttpPost]
        public async Task<Mission> FindClosest(MissionSearch search)
        {
            return await _agentRepository.FindClosest(search);  
        }
    }
}
