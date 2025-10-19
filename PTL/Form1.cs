using System;
using System.Data;
using ScottPlot;
using ScottPlot.Statistics;
using ScottPlot.TickGenerators.TimeUnits;
using ScottPlot.WinForms;

namespace PTL
{
    public partial class Form1 : Form
    {
        private DataTable csvData;

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            string csvPath = @"H:\323-programation fonctionnelle\Projet\323-Plot-those-lines\PTL\data_LeMans_race_winners.csv";
            csvData = LoadCsvToDataTable(csvPath);

            string columns = string.Join(", ", csvData.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            MessageBox.Show("Colonnes disponibles : " + columns);

            // Conversion avec culture invariante (sans ça il confond "." et ",")
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            //------------------------------------------ Données CSV -------------------------------------------//
            double[] years = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Year"].ToString(), culture))
                .ToArray();

            double[] laps = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Laps"].ToString(), culture))
                .ToArray();

            double[] kms = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Km"].ToString(), culture))
                .ToArray();

            double[] miles = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Mi"].ToString(), culture))
                .ToArray();

            double[] avgSpeedsKmh = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Average_speed_kmh"].ToString(), culture))
                .ToArray();

            double[] avgSpeedsMph = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Average_speed_mph"].ToString(), culture))
                .ToArray();

            double[] avgLapTimes = csvData.AsEnumerable()
                .Select(r => Convert.ToDouble(r["Average_lap_time"].ToString(), culture))
                .ToArray();

            string?[] drivers = csvData.AsEnumerable()
                .Select(r => r["Drivers"].ToString())
                .ToArray();

            string?[] Class = csvData.AsEnumerable()
                .Select(r => r["Class"].ToString())
                .ToArray();

            string?[] team = csvData.AsEnumerable()
                .Select(r => r["Team"].ToString())
                .ToArray();

            string?[] car = csvData.AsEnumerable()
                .Select(r => r["Car"].ToString())
                .ToArray();

            string?[] tyre = csvData.AsEnumerable()
               .Select(r => r["Tyre"].ToString())
               .ToArray();

            string?[] series = csvData.AsEnumerable()
               .Select(r => r["Series"].ToString())
               .ToArray();

            string?[] driver_nationality = csvData.AsEnumerable()
               .Select(r => r["Driver_nationality"].ToString())
               .ToArray();

            string?[] team_nationality = csvData.AsEnumerable()
               .Select(r => r["Team_nationality"].ToString())
               .ToArray();
            //---------------------------------------------------------------------------------------------------//


            formsPlot1.Plot.Add.Scatter(years, avgSpeedsKmh);
            formsPlot1.Plot.Axes.Title.Label.Text = "Vitesse moyenne des vainqueurs au Mans";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Vitesse moyenne (km/h)";
            formsPlot1.Refresh();

            formsPlot2.Plot.Add.Scatter(drivers, years);
            formsPlot2.Plot.Axes.Title.Label.Text = "Victoire de chaque pilote par année";
            formsPlot2.Plot.Axes.Bottom.Label.Text = "Pilotes";
            formsPlot2.Plot.Axes.Left.Label.Text = "Années";
            formsPlot2.Refresh();
        }

        private DataTable LoadCsvToDataTable(string csvPath)
        {
            var dt = new DataTable();
            var lines = File.ReadAllLines(csvPath);

            if (lines.Length > 0)
            {
                var headers = lines[0].Split(',');
                foreach (var header in headers)
                    dt.Columns.Add(header);

                for (int i = 1; i < lines.Length; i++)
                {
                    var row = lines[i].Split(',');
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }

        private void formsPlot2_Load(object sender, EventArgs e)
        {

        }
    }
}
