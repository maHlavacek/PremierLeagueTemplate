using Microsoft.EntityFrameworkCore;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PremierLeague.Persistence
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (Team team, int goals) GetTheTeamWithTheMostSchootedGoals()
        {

            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.HomeGames.Sum(h => h.HomeGoals) + t.AwayGames.Sum(a => a.GuestGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();

            //return _dbContext.Teams.Select(s => new
            //{
            //    Goals = s.HomeGames.Sum(w => w.HomeGoals) + s.AwayGames.Sum(w => w.GuestGoals),
            //    Team = s.Name
            //}
            //);
                        
        }


        public IEnumerable<Team> GetAllWithGames()
        {
            return _dbContext.Teams.Include(t => t.HomeGames).Include(t => t.AwayGames).ToList();
        }

        public IEnumerable<Team> GetAll()
        {
            return _dbContext.Teams.OrderBy(t => t.Name).ToList();
        }

        public void AddRange(IEnumerable<Team> teams)
        {
            _dbContext.Teams.AddRange(teams);
        }

        public Team Get(int teamId)
        {
            return _dbContext.Teams.Find(teamId);
        }

        public void Add(Team team)
        {
            _dbContext.Teams.Add(team);
        }
    }
}