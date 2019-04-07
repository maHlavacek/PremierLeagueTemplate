using Microsoft.EntityFrameworkCore;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.DataTransferObjects;
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

        public (Team team, int goals) TeamWithTheMostSchootedGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.HomeGames.Sum(hg => hg.HomeGoals) + t.AwayGames.Sum(ag => ag.GuestGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();     
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

        public (Team team, int goals) TeamWithTheMostSchootedAwayGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.AwayGames.Sum(ag => ag.GuestGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();
        }

        public (Team team, int goals) TeamWithTheMostSchootedHomeGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.HomeGames.Sum(hg => hg.HomeGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();
        }

        public (Team team, int rate) TeamWithTheBestGoalsRate()
        {
            return _dbContext.Teams.Select(t =>
            ValueTuple.Create(t, (t.HomeGames.Sum(hg => hg.HomeGoals) + t.AwayGames.Sum(ag => ag.GuestGoals)) - (t.HomeGames.Sum(hg => hg.GuestGoals) + t.AwayGames.Sum(ag => ag.HomeGoals))))
            .OrderByDescending(t => t.Item2)
            .FirstOrDefault();

        }

        public IEnumerable<TeamStatisticDto> AvgStatistic()
        {
            return _dbContext.Teams.Select(t => new TeamStatisticDto
            {
                Name = t.Name,
                AvgGoalsShotAtHome = t.HomeGames.Average(hg => hg.HomeGoals),
                AvgGoalsShotOutwards = t.AwayGames.Average(ag => ag.GuestGoals),               
                AvgGoalsShotInTotal = (t.AwayGames.Average(ag => ag.GuestGoals) + t.HomeGames.Average(hg => hg.HomeGoals)) / 2,
                AvgGoalsGotAtHome = t.HomeGames.Average(hg => hg.GuestGoals),
                AvgGoalsGotOutwards = t.AwayGames.Average(ag => ag.HomeGoals),
                AvgGoalsGotInTotal = (t.AwayGames.Average(ag => ag.HomeGoals) + t.HomeGames.Average(hg => hg.GuestGoals)) / 2
            }
            ).OrderByDescending(o => o.AvgGoalsShotInTotal);
        }

        public IEnumerable<TeamTableRowDto> TeamTable()
        {
            var teamTable = _dbContext.Teams.Select(t => new TeamTableRowDto
            {
                Id = t.Id,
                Name = t.Name,
                Matches = t.HomeGames.Count() + t.AwayGames.Count(),
                Won = t.AwayGames.Where(ag => ag.GuestGoals > ag.HomeGoals).Count() + t.HomeGames.Where(hg => hg.HomeGoals > hg.GuestGoals).Count(),
                Lost = t.AwayGames.Where(ag => ag.GuestGoals < ag.HomeGoals).Count() + t.HomeGames.Where(hg => hg.HomeGoals < hg.GuestGoals).Count(),
                GoalsFor = t.HomeGames.Sum(hg => hg.HomeGoals) + t.AwayGames.Sum(ag => ag.GuestGoals),
                GoalsAgainst = t.HomeGames.Sum(hg => hg.GuestGoals) + t.AwayGames.Sum(ag => ag.HomeGoals),
            }
            ).OrderByDescending(o => o.Points).ThenByDescending(ga => ga.GoalDifference).ToArray();
            int i = 1;
            foreach (var team in teamTable)
            {
                team.Rank = i;
                i++;
            }
            return teamTable;
        }
    }
}