using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using ScottPlot;
using ScottPlot.Statistics;
using ScottPlot.TickGenerators;
using ScottPlot.TickGenerators.TimeUnits;
using ScottPlot.WinForms;

namespace PTL
{
    public partial class Form1 : Form
    {
        private DataTable csvData;

        // champs pour conserver les données et état d'unité
        private double[] years = Array.Empty<double>();
        private double[] avgSpeedsKmh = Array.Empty<double>();
        private double[] avgSpeedsMph = Array.Empty<double>();
        private string[] teams = Array.Empty<string>();
        private bool showingKmh = true;

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
            var culture = CultureInfo.InvariantCulture;

            //------------------------------------------ Données CSV -------------------------------------------//
            years = ToDoubleArray(csvData, "Year", culture);
            var laps = ToDoubleArray(csvData, "Laps", culture);
            var kms = ToDoubleArray(csvData, "Km", culture);
            var miles = ToDoubleArray(csvData, "Mi", culture);
            avgSpeedsKmh = ToDoubleArray(csvData, "Average_speed_kmh", culture);
            avgSpeedsMph = ToDoubleArray(csvData, "Average_speed_mph", culture);
            var avgLapTimes = ToDoubleArray(csvData, "Average_lap_time", culture);

            teams = csvData.AsEnumerable()
                .Select(r => r["Team"]?.ToString() ?? string.Empty)
                .ToArray();

            string?[] drivers = csvData.AsEnumerable()
                .Select(r => r["Drivers"]?.ToString() ?? string.Empty)
                .ToArray();

            string?[] classes = csvData.AsEnumerable()
                .Select(r => r["Class"]?.ToString() ?? string.Empty)
                .ToArray();

            string?[] car = csvData.AsEnumerable()
                .Select(r => r["Car"]?.ToString() ?? string.Empty)
                .ToArray();

            string?[] tyre = csvData.AsEnumerable()
               .Select(r => r["Tyre"]?.ToString() ?? string.Empty)
               .ToArray();

            string?[] series = csvData.AsEnumerable()
               .Select(r => r["Series"]?.ToString() ?? string.Empty)
               .ToArray();

            string?[] driver_nationality = csvData.AsEnumerable()
               .Select(r => r["Driver_nationality"]?.ToString() ?? string.Empty)
               .ToArray();

            string?[] team_nationality = csvData.AsEnumerable()
               .Select(r => r["Team_nationality"]?.ToString() ?? string.Empty)
               .ToArray();
            //---------------------------------------------------------------------------------------------------//

            // initial plot en km/h
            UpdateSpeedPlot();
        }

        private void ToggleSpeedButton_Click(object? sender, EventArgs e)
        {
            // bascule l'état et met à jour l'affichage
            showingKmh = !showingKmh;

            if (sender is Button b)
                b.Text = showingKmh ? "Afficher en mph" : "Afficher en km/h";

            UpdateSpeedPlot();
        }

        private void UpdateSpeedPlot()
        {
            // protège contre données manquantes
            if (years == null || (avgSpeedsKmh == null && avgSpeedsMph == null))
                return;

            // choisit le tableau de vitesses
            var speeds = showingKmh ? avgSpeedsKmh : avgSpeedsMph;

            // s'assure longueurs compatibles
            int n = Math.Min(years.Length, speeds.Length);
            var yearsTrim = years.Take(n).ToArray();
            var speedsTrim = speeds.Take(n).ToArray();

            // remplace le contenu du plot et rafraîchit
            formsPlot1.Plot.Clear();
            formsPlot1.Plot.Add.Scatter(yearsTrim, speedsTrim);
            formsPlot1.Plot.Axes.Title.Label.Text = "Vitesse moyenne des vainqueurs au Mans";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = showingKmh ? "Vitesse moyenne (km/h)" : "Vitesse moyenne (mph)";
            formsPlot1.Refresh();
        }

        private DataTable LoadCsvToDataTable(string csvPath)
        {
            var dt = new DataTable();

            // Utilise TextFieldParser pour gérer correctement les champs entre guillemets et les virgules à l'intérieur des champs
            using (var parser = new TextFieldParser(csvPath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;

                if (!parser.EndOfData)
                {
                    string[] headers = parser.ReadFields();
                    foreach (var header in headers)
                        dt.Columns.Add(header);
                }

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    // si nombre de champs diffère des colonnes, on complète ou tronque pour éviter exceptions
                    if (fields.Length != dt.Columns.Count)
                    {
                        var adjusted = new string[dt.Columns.Count];
                        for (int i = 0; i < dt.Columns.Count; i++)
                            adjusted[i] = i < fields.Length ? fields[i] : string.Empty;
                        dt.Rows.Add(adjusted);
                    }
                    else
                    {
                        dt.Rows.Add(fields);
                    }
                }
            }

            return dt;
        }

        private double[] ToDoubleArray(DataTable dt, string columnName, CultureInfo culture)
        {
            var list = new List<double>(dt.Rows.Count);
            foreach (DataRow r in dt.Rows)
            {
                var obj = r.Table.Columns.Contains(columnName) ? r[columnName] : null;
                var s = obj?.ToString() ?? string.Empty;
                if (double.TryParse(s, NumberStyles.Any, culture, out var v))
                    list.Add(v);
                else
                    list.Add(double.NaN); // conserve la position mais marque invalide
            }
            return list.ToArray();
        }

        private void formsPlot1_Load(object sender, EventArgs e)
        {

        }

        private void formsPlot2_Load(object sender, EventArgs e)
        {

        }
    }
}
