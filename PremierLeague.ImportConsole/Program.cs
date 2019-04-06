using PremierLeague.Core;
using PremierLeague.Core.Contracts;
using PremierLeague.Core.Entities;
using PremierLeague.Persistence;
using Serilog;
using System;
using System.Linq;

namespace PremierLeague.ImportConsole
{
    class Program
    {
        static void Main()
        {
            PrintHeader();
            InitData();
            AnalyzeData();

            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(new String('-', 60));

            Console.WriteLine(
                  @"
            _,...,_
          .'@/~~~\@'.          
         //~~\___/~~\\        P R E M I E R  L E A G U E 
        |@\__/@@@\__/@|             
        |@/  \@@@/  \@|            (inkl. Statistik)
         \\__/~~~\__//
          '.@\___/@.'
            `""""""
                ");

            Console.WriteLine(new String('-', 60));
            Console.WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Importiert die Ergebnisse (csv-Datei >> Datenbank).
        /// </summary>
        private static void InitData()
        {
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                Log.Information("Import der Spiele und Teams in die Datenbank");

                Log.Information("Datenbank löschen");
                // TODO: Datenbank löschen
                unitOfWork.DeleteDatabase();

                Log.Information("Datenbank migrieren");
                // TODO: Datenbank migrieren
                unitOfWork.MigrateDatabase();

                Log.Information("Spiele werden von premierleague.csv eingelesen");
                var games = ImportController.ReadFromCsv().ToArray();
                if (games.Length == 0)
                {
                    Log.Warning("!!! Es wurden keine Spiele eingelesen");
                }
                else
                {
                    Log.Debug($"  Es wurden {games.Count()} Spiele eingelesen!");

                    // TODO: Teams aus den Games ermitteln
                    var teams = games.Select(s => s.HomeTeam).Distinct();// Enumerable.Empty<Team>();
                    Log.Debug($"  Es wurden {teams.Count()} Teams eingelesen!");

                    Log.Information("Daten werden in Datenbank gespeichert (in Context übertragen)");

                    // TODO: Teams/Games in der Datenbank speichern
                    unitOfWork.Games.AddRange(games);
                    unitOfWork.Teams.AddRange(teams);
                    unitOfWork.SaveChanges();
                    Log.Information("Daten wurden in DB gespeichert!");
                }
            }
        }

        private static void AnalyzeData()
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                var best = unitOfWork.Teams.TeamWithTheMostSchootedGoals();
                PrintResult("Team mit den meisten geschossenen Toren", String.Format("{0}: {1} Tore",best.team.Name, best.goals));

                var away = unitOfWork.Teams.TeamWithTheMostSchootedAwayGoals();
                PrintResult("Team mit den meisten geschossenen Auswärtstoren", String.Format("{0}: {1} auswärtstore", away.team.Name, away.goals));

                var home = unitOfWork.Teams.TeamWithTheMostSchootedHomeGoals();
                PrintResult("Team mit den meisten geschossenen Heimtoren", String.Format("{0}: {1} Heimtore", home.team.Name, home.goals));

                var rate = unitOfWork.Teams.TeamWithTheBestGoalsRate();
                PrintResult("Team mit dem besten Torverhältnis", String.Format("{0}: {1} Torverhältnis", rate.team.Name, rate.rate));

            }

        }

        /// <summary>
        /// Erstellt eine Konsolenausgabe
        /// </summary>
        /// <param name="caption">Enthält die Überschrift</param>
        /// <param name="result">Enthält das ermittelte Ergebnise</param>
        private static void PrintResult(string caption, string result)
        {
            Console.WriteLine();

            if (!string.IsNullOrEmpty(caption))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(new String('=', caption.Length));
                Console.WriteLine(caption);
                Console.WriteLine(new String('=', caption.Length));
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(result);
            Console.ResetColor();
            Console.WriteLine();
        }


    }
}
