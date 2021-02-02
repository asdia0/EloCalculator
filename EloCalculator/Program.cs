namespace EloCalculator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using OfficeOpenXml;

    public class Program
    {
        public static List<Player> playerList = new List<Player>();

        public static List<Game> gameList = new List<Game>();

        public static List<Game> newGames = new List<Game>();

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting application...");

            playerList = JsonConvert.DeserializeObject<List<Player>>(File.ReadAllText("resources/players.json"));
            gameList = JsonConvert.DeserializeObject<List<Game>>(File.ReadAllText("resources/games.json"));

            FileInfo fi = new FileInfo("resources/newGames.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelWorksheet ws = excelPackage.Workbook.Worksheets[0];

                var start = ws.Dimension.Start.Row;
                var end = ws.Dimension.End.Row;

                for (int row = start; row <= end; row++)
                {
                    if (ws.Cells[row, 1].Value == null)
                    {
                        break;
                    }
                    string p1Name = ws.Cells[row, 1].Value.ToString();
                    string p2Name = ws.Cells[row, 2].Value.ToString();
                    string result = ws.Cells[row, 3].Value.ToString();
                    string date = ws.Cells[row, 4].Value.ToString();
#nullable enable
                    Player? p1 = null;
                    Player? p2 = null;
#nullable disable
                    foreach (Player p in playerList)
                    {
                        if (p.name == p1Name)
                        {
                            p1 = p;
                        }
                        if (p.name == p2Name)
                        {
                            p2 = p;
                        }
                    }

                    if (p1 == null)
                    {
                        p1 = new Player(p1Name);
                    }

                    if (p2 == null)
                    {
                        p2 = new Player(p2Name);
                    }

                    newGames.Add(new Game(p1, p2, result, date));
                }

                excelPackage.Save();
            }

            string pjson = JsonConvert.SerializeObject(playerList, Formatting.Indented);
            using (StreamWriter outputFile = new StreamWriter("resources/players.json"))
            {
                outputFile.Write(pjson);
            }
            string gjson = JsonConvert.SerializeObject(gameList, Formatting.Indented);
            using (StreamWriter outputFile = new StreamWriter("resources/games.json"))
            {
                outputFile.Write(gjson);
            }

            string visual = string.Empty;
            foreach (Player p in playerList)
            {
                visual += $"{p.title},{p.name},{p.rating},,{p.wins},{p.draws},{p.losses}\n";
            }

            using (StreamWriter outputFile = new StreamWriter("resources/googleSheets.csv"))
            {
                outputFile.Write(visual);
            }

            Console.WriteLine("Complete.");
        }
    }
}
