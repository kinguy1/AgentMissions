using MissionsApi.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MissionsApi.Services
{
    public interface IAgentRepository
    {
        Task<Guid> SaveMissionAsync(Mission mission);

        Task<List<Mission>> GetAllAsync();

        Task<string> GetMostIsolatedCountry();

        Task<Mission> FindClosest(MissionSearch search);

    }
}
