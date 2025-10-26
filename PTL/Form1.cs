using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
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
        private double[] laps = Array.Empty<double>();
        private double[] kms = Array.Empty<double>();
        private double[] miles = Array.Empty<double>();
        private string[] teams = Array.Empty<string>();
        private bool showingKmh = true;

        // label d'affichage de la plage (fully-qualified to avoid ambiguity)
        private System.Windows.Forms.Label lblRange = null!;

        public Form1()
        {
            InitializeComponent();
        }

        // Chargement du formulaire : lecture CSV, initialisation des TrackBars et ComboBox
        private void Form1_Load(object sender, EventArgs e)
        {
            //--------------------------------------- !! A CHANGER SELON LA MACHINE !! ---------------------------------------\\
            string csvPath = @"H:\323-programation fonctionnelle\Projet\323-Plot-those-lines\PTL\data_LeMans_race_winners.csv";
            csvData = LoadCsvToDataTable(csvPath);

            string columns = string.Join(", ", csvData.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            MessageBox.Show("Colonnes disponibles : " + columns);

            // Conversion avec culture invariante (sans ça il confond "." et ",")
            var culture = CultureInfo.InvariantCulture;

            //------------------------------------------ Données CSV -------------------------------------------//
            // stocke en champs pour pouvoir réutiliser dans les différents graphiques
            years = ToDoubleArray(csvData, "Year", culture);
            laps = ToDoubleArray(csvData, "Laps", culture);
            kms = ToDoubleArray(csvData, "Km", culture);
            miles = ToDoubleArray(csvData, "Mi", culture);
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

            // initialise les trackbars (trackBar2 = min, trackBar1 = max)
            var validYears = years.Where(y => !double.IsNaN(y)).Select(y => (int)Math.Floor(y)).ToArray();
            int minYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Min();
            int maxYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Max();
            int tick = Math.Max(1, (maxYear - minYear) / 10);

            // configure trackBar gauche (min)
            trackBar2.Minimum = minYear;
            trackBar2.Maximum = maxYear;
            trackBar2.Value = minYear;
            trackBar2.TickFrequency = tick;
            trackBar2.SmallChange = 1;
            trackBar2.LargeChange = 1;
            trackBar2.ValueChanged += RangeTrackBar_ValueChanged;

            // configure trackBar droite (max)
            trackBar1.Minimum = minYear;
            trackBar1.Maximum = maxYear;
            trackBar1.Value = maxYear;
            trackBar1.TickFrequency = tick;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;
            trackBar1.ValueChanged += RangeTrackBar_ValueChanged;

            // label d'affichage de la plage (positionné au-dessus des trackbars)
            lblRange = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Location = new Point(603, trackBar2.Location.Y),
                Text = $"{trackBar2.Value} — {trackBar1.Value}"
            };
            this.Controls.Add(lblRange);
            lblRange.BringToFront();

            // --- ComboBox pour choisir le type de graphique ---
            // Utiliser le ComboBox ajouté dans le concepteur (ex : `comboBox1` ou renommez-le en `comboBoxGraph`)
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[]
            {
                "Vitesse moyenne (km/h)",
                "Vitesse moyenne (mph)",
                "Tours (laps)",
                "Distance parcourue (km)"
            });
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBoxGraph_SelectedIndexChanged;

            // initial plot (évalue la ComboBox et les TrackBars)
            UpdatePlot();
        }

        // Gestionnaire pour le changement de sélection de la ComboBox
        private void ComboBoxGraph_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdatePlot();
        }

        private void RangeTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            // empêche inversion : si start > end, on force l'autre valeur
            if (trackBar2.Value > trackBar1.Value)
            {
                if (sender == trackBar2)
                    trackBar1.Value = trackBar2.Value;
                else
                    trackBar2.Value = trackBar1.Value;
            }

            // met à jour le label et le plot
            if (lblRange != null)
                lblRange.Text = $"{trackBar2.Value} — {trackBar1.Value}";

            UpdatePlot();
        }

        // Méthode centrale qui choisit quel graphique afficher selon la ComboBox
        private void UpdatePlot()
        {
            if (years == null) return;

            int selected = comboBox1?.SelectedIndex ?? 0;
            switch (selected)
            {
                case 0: // km/h
                    showingKmh = true;
                    PlotSpeed();
                    break;
                case 1: // mph
                    showingKmh = false;
                    PlotSpeed();
                    break;
                case 2: // laps
                    PlotLaps();
                    break;
                case 3: // km
                    PlotDistanceKm();
                    break;
                default:
                    PlotSpeed();
                    break;
            }
        }

        // Trace la vitesse moyenne filtrée par la plage d'années
        private void PlotSpeed()
        {
            if ((avgSpeedsKmh == null && avgSpeedsMph == null) || years == null) return;
            var speeds = showingKmh ? avgSpeedsKmh : avgSpeedsMph;

            int n = Math.Min(years.Length, speeds.Length);
            var xs = new List<double>(n);
            var ys = new List<double>(n);

            int startYear = trackBar2?.Value ?? int.MinValue;
            int endYear = trackBar1?.Value ?? int.MaxValue;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            for (int i = 0; i < n; i++)
            {
                double yearVal = years[i];
                double speedVal = speeds[i];
                if (double.IsNaN(yearVal) || double.IsNaN(speedVal)) continue;

                int yearInt = (int)Math.Floor(yearVal);
                if (yearInt < startYear || yearInt > endYear) continue;

                xs.Add(yearVal);
                ys.Add(speedVal);
            }

            formsPlot1.Plot.Clear();
            if (xs.Count > 0)
                formsPlot1.Plot.Add.Scatter(xs.ToArray(), ys.ToArray());
            formsPlot1.Plot.Axes.Title.Label.Text = "Vitesse moyenne des vainqueurs au Mans";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = showingKmh ? "Vitesse moyenne (km/h)" : "Vitesse moyenne (mph)";
            formsPlot1.Refresh();
        }

        // Trace le nombre de tours (laps) par année, filtré par plage
        private void PlotLaps()
        {
            if (laps == null || years == null) return;

            int n = Math.Min(years.Length, laps.Length);
            var xs = new List<double>(n);
            var ys = new List<double>(n);

            int startYear = trackBar2?.Value ?? int.MinValue;
            int endYear = trackBar1?.Value ?? int.MaxValue;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            for (int i = 0; i < n; i++)
            {
                double yearVal = years[i];
                double lapVal = laps[i];
                if (double.IsNaN(yearVal) || double.IsNaN(lapVal)) continue;

                int yearInt = (int)Math.Floor(yearVal);
                if (yearInt < startYear || yearInt > endYear) continue;

                xs.Add(yearVal);
                ys.Add(lapVal);
            }

            formsPlot1.Plot.Clear();
            if (xs.Count > 0)
                formsPlot1.Plot.Add.Bars(xs.ToArray(), ys.ToArray()); // bar chart pour visualiser les tours
            formsPlot1.Plot.Axes.Title.Label.Text = "Tours (laps) par année";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Nombre de tours";
            formsPlot1.Refresh();
        }

        // Trace la distance parcourue en kilomètres par année
        private void PlotDistanceKm()
        {
            if (kms == null || years == null) return;

            int n = Math.Min(years.Length, kms.Length);
            var xs = new List<double>(n);
            var ys = new List<double>(n);

            int startYear = trackBar2?.Value ?? int.MinValue;
            int endYear = trackBar1?.Value ?? int.MaxValue;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            for (int i = 0; i < n; i++)
            {
                double yearVal = years[i];
                double kmVal = kms[i];
                if (double.IsNaN(yearVal) || double.IsNaN(kmVal)) continue;

                int yearInt = (int)Math.Floor(yearVal);
                if (yearInt < startYear || yearInt > endYear) continue;

                xs.Add(yearVal);
                ys.Add(kmVal);
            }

            formsPlot1.Plot.Clear();
            if (xs.Count > 0)
                formsPlot1.Plot.Add.Bars(xs.ToArray(), ys.ToArray());
            formsPlot1.Plot.Axes.Title.Label.Text = "Distance parcourue par année";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Distance (km)";
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
    }
}
