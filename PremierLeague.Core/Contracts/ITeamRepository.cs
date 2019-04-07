using PremierLeague.Core.DataTransferObjects;
using PremierLeague.Core.Entities;
using System;
using System.Collections.Generic;

namespace PremierLeague.Core.Contracts
{
    public interface ITeamRepository
    {
        IEnumerable<Team> GetAllWithGames();
        (Team team,int goals) TeamWithTheMostSchootedGoals();

        (Team team, int goals) TeamWithTheMostSchootedAwayGoals();

        (Team team, int goals) TeamWithTheMostSchootedHomeGoals();

        (Team team, int rate) TeamWithTheBestGoalsRate();

        IEnumerable<TeamStatisticDto> AvgStatistic();
        IEnumerable<Team> GetAll();
        void AddRange(IEnumerable<Team> teams);
        Team Get(int teamId);
        void Add(Team team);
    }
}