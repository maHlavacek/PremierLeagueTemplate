using System.Collections.Generic;
using System.Linq;
using PremierLeague.Core.Entities;
using Utils;

namespace PremierLeague.Core
{
    public static class ImportController
    {
        public static IEnumerable<Game> ReadFromCsv()
        {
            string[][] matrix = MyFile.ReadStringMatrixFromCsv("PremierLeague.csv", false);  // keine Titelzeile
            // Einlesen der Spiele und der Teams
            // Zuerst die Teams
            var teams = matrix
                .Select(line => line[1])
                .Union(matrix.Select(line=>line[2]))
                .Distinct()
                .Select(text => new Team
                {
                    Name = text
                })
                .OrderBy(a => a.Name)
                .ToList();

            var games = matrix
                .Select(line => new Game()
                {
                    Round = int.Parse(line[0]),
                    HomeTeam = teams.Single(t => t.Name == line[1]),
                    GuestTeam = teams.Single(t => t.Name == line[2]),
                    HomeGoals = int.Parse(line[3]),
                    GuestGoals = int.Parse(line[4])
                })
                .ToList();
            return games;
        }

    }
}
