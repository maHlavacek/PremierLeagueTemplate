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
        /// <summary>
        /// Get the Team with the most shot goals
        /// </summary>
        /// <returns>team and shot goals</returns>
        public (Team team, int goals) TeamWithTheMostShotGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.HomeGames.Sum(hg => hg.HomeGoals) + t.AwayGames.Sum(ag => ag.GuestGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();     
        }

        /// <summary>
        /// Get all teams with the games
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Team> GetAllWithGames()
        {
            return _dbContext.Teams.Include(t => t.HomeGames).Include(t => t.AwayGames).ToList();
        }
        /// <summary>
        /// Get all teams
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Team> GetAll()
        {
            return _dbContext.Teams.OrderBy(t => t.Name).ToList();
        }
        /// <summary>
        /// Add range of teams
        /// </summary>
        /// <param name="teams"></param>
        public void AddRange(IEnumerable<Team> teams)
        {
            _dbContext.Teams.AddRange(teams);
        }
        /// <summary>
        /// Get the team with the passed id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public Team Get(int teamId)
        {
            return _dbContext.Teams.Find(teamId);
        }
        /// <summary>
        /// Add a team
        /// </summary>
        /// <param name="team"></param>
        public void Add(Team team)
        {
            _dbContext.Teams.Add(team);
        }
        /// <summary>
        /// Get the team with the most shot away goals
        /// </summary>
        /// <returns>team and goals</returns>
        public (Team team, int goals) TeamWithTheMostShotAwayGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.AwayGames.Sum(ag => ag.GuestGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();
        }
        /// <summary>
        /// Get the team with the most shot home goals
        /// </summary>
        /// <returns>team and goals</returns>
        public (Team team, int goals) TeamWithTheMostShotHomeGoals()
        {
            return _dbContext.Teams.Select(t => ValueTuple.Create(t, t.HomeGames.Sum(hg => hg.HomeGoals)))
                .OrderByDescending(t => t.Item2)
                .FirstOrDefault();
        }
        /// <summary>
        /// Get the team with the best goals rate
        /// </summary>
        /// <returns>team and rate</returns>
        public (Team team, int rate) TeamWithTheBestGoalsRate()
        {
            return _dbContext.Teams.Select(t =>
            ValueTuple.Create(t, (t.HomeGames.Sum(hg => hg.HomeGoals) + t.AwayGames.Sum(ag => ag.GuestGoals)) 
                             - (t.HomeGames.Sum(hg => hg.GuestGoals) + t.AwayGames.Sum(ag => ag.HomeGoals))))
                             .OrderByDescending(t => t.Item2)
                             .FirstOrDefault();
        }
        /// <summary>
        /// Get the statistic of every team
        /// </summary>
        /// <returns>IEnumerable<TeamStatisticDto></returns>
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

        /// <summary>
        /// Get the team table of the teams
        /// </summary>
        /// <returns>Ienumerable<TeamTableDto></returns>
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
            ).OrderByDescending(o => o.Points)
             .ThenByDescending(ga => ga.GoalDifference)
             .ToArray();

            // Fill in the Rank
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