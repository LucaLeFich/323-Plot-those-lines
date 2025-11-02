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
using ScottPlot.Plottables;

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

        private System.Windows.Forms.Label lblRange = null!;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string csvPath = @"..\..\..\data_LeMans_race_winners.csv";
            csvData = LoadCsvToDataTable(csvPath);

            // Sans ça le programme confond "," et "."
            var culture = CultureInfo.InvariantCulture;

            //------------------------------------------ Données CSV -------------------------------------------//
            // On utilise l'extension de langage ToDoubleArray() définie plus bas //
            years = csvData.ToDoubleArray("Year", culture);
            laps = csvData.ToDoubleArray("Laps", culture);
            kms = csvData.ToDoubleArray("Km", culture);
            miles = csvData.ToDoubleArray("Mi", culture);
            avgSpeedsKmh = csvData.ToDoubleArray("Average_speed_kmh", culture);
            avgSpeedsMph = csvData.ToDoubleArray("Average_speed_mph", culture);
            teams = csvData.ToStringArray("Team");
            //---------------------------------------------------------------------------------------------------//

            var validYears = years.Where(y => !double.IsNaN(y)).Select(y => (int)Math.Floor(y)).ToArray();
            int minYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Min();
            int maxYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Max();
            int tick = Math.Max(1, (maxYear - minYear) / 10);

            trackBar2.Minimum = minYear;
            trackBar2.Maximum = maxYear;
            trackBar2.Value = minYear;
            trackBar2.ValueChanged += RangeTrackBar_ValueChanged;

            trackBar1.Minimum = minYear;
            trackBar1.Maximum = maxYear;
            trackBar1.Value = maxYear;
            trackBar1.ValueChanged += RangeTrackBar_ValueChanged;

            // Plage d'années (Affichage)
            lblRange = new System.Windows.Forms.Label
            {
                AutoSize = true,
                Location = new Point(0, trackBar2.Location.Y),
                Text = $"{trackBar2.Value} — {trackBar1.Value}"
            };
            this.Controls.Add(lblRange);
            lblRange.BringToFront();

            PositionRangeLabel();
            this.Resize += (s, ev) => PositionRangeLabel();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[]
            {
                "Vitesse moyenne (km/h)",
                "Vitesse moyenne (mph)",
                "Tours (laps)",
                "Distance parcourue (km)",
                "Nombre de victoires par équipes"
            });
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBoxGraph_SelectedIndexChanged;

            UpdatePlot();
        }

        private void ComboBoxGraph_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdatePlot();
        }

        private void RangeTrackBar_ValueChanged(object? sender, EventArgs e)
        {
            if (trackBar2.Value > trackBar1.Value)
            {
                if (sender == trackBar2)
                    trackBar1.Value = trackBar2.Value;
                else
                    trackBar2.Value = trackBar1.Value;
            }

            if (lblRange != null)
                lblRange.Text = $"{trackBar2.Value} — {trackBar1.Value}";

            UpdatePlot();
        }

        private void UpdatePlot()
        {
            if (years.Length == 0) return;

            int selected = comboBox1?.SelectedIndex ?? 0;
            switch (selected)
            {
                case 0:
                    showingKmh = true;
                    PlotSpeed();
                    break;
                case 1:
                    showingKmh = false;
                    PlotSpeed();
                    break;
                case 2:
                    PlotLaps();
                    break;
                case 3:
                    PlotDistanceKm();
                    break;
                case 4:
                    PlotTeamWins();
                    break;
                default:
                    PlotSpeed();
                    break;
            }
        }

        private void PlotSpeed()
        {
            if ((avgSpeedsKmh == null && avgSpeedsMph == null) || years.Length == 0) return;

            bool showBoth = checkBox1 != null && checkBox1.Checked;

            var kmh = avgSpeedsKmh;
            var mph = avgSpeedsMph;

            int n = Math.Min(years.Length, Math.Max(kmh.Length, mph.Length));
            int startYear = trackBar2.Value;
            int endYear = trackBar1.Value;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            var points = Enumerable.Range(0, n)
                .Select(i => new
                {
                    Year = i < years.Length ? years[i] : double.NaN,
                    Kmh = i < kmh.Length ? kmh[i] : double.NaN,
                    Mph = i < mph.Length ? mph[i] : double.NaN
                })
                .Where(p => !double.IsNaN(p.Year))
                .Select(p => new { YearInt = (int)Math.Floor(p.Year), p.Year, p.Kmh, p.Mph })
                .Where(p => p.YearInt >= startYear && p.YearInt <= endYear)
                .ToArray();

            var xs = points.Select(p => p.Year).ToArray();
            var ysKmh = points.Select(p => p.Kmh).ToArray();
            var ysMph = points.Select(p => p.Mph).ToArray();

            formsPlot1.Plot.Clear();
            ResetLeftAxisTickGenerator();

            if (showBoth)
            {
                if (xs.Length > 0)
                {
                    var kmSeries = formsPlot1.Plot.Add.Scatter(xs, ysKmh);
                    kmSeries.Label = "km/h";
                    kmSeries.Color = ScottPlot.Color.FromARGB(System.Drawing.Color.FromArgb(0, 0, 255).ToArgb());

                    var mphSeries = formsPlot1.Plot.Add.Scatter(xs, ysMph);
                    mphSeries.Label = "mph";
                    mphSeries.Color = ScottPlot.Color.FromARGB(System.Drawing.Color.FromArgb(255, 69, 0).ToArgb());
                }

                formsPlot1.Plot.ShowLegend();
                formsPlot1.Plot.Axes.AutoScale();
                formsPlot1.Plot.Axes.Left.Label.Text = "Vitesse (km/h et mph)";
            }
            else
            {
                var speeds = showingKmh ? ysKmh : ysMph;
                var validIdx = Enumerable.Range(0, xs.Length).Where(i => !double.IsNaN(speeds[i])).ToArray();
                var validXs = validIdx.Select(i => xs[i]).ToArray();
                var validYs = validIdx.Select(i => speeds[i]).ToArray();

                if (validXs.Length > 0)
                {
                    var s = formsPlot1.Plot.Add.Scatter(validXs, validYs);
                }

                formsPlot1.Plot.Axes.Left.Label.Text = showingKmh ? "Vitesse moyenne (km/h)" : "Vitesse moyenne (mph)";
                formsPlot1.Plot.Axes.AutoScale();
            }

            formsPlot1.Plot.Axes.Title.Label.Text = "Vitesse moyenne des vainqueurs au Mans";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Refresh();
        }

        private void PlotLaps()
        {
            if (laps == null || years.Length == 0) return;

            int n = Math.Min(years.Length, laps.Length);
            int startYear = trackBar2.Value;
            int endYear = trackBar1.Value;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            var points = Enumerable.Range(0, n)
                .Select(i => new { Year = years[i], Value = laps[i] })
                .Where(p => !double.IsNaN(p.Year) && !double.IsNaN(p.Value))
                .Select(p => new { YearInt = (int)Math.Floor(p.Year), p.Year, p.Value })
                .Where(p => p.YearInt >= startYear && p.YearInt <= endYear)
                .ToArray();

            var xs = points.Select(p => p.Year).ToArray();
            var ys = points.Select(p => p.Value).ToArray();

            formsPlot1.Plot.Clear();
            ResetLeftAxisTickGenerator();

            if (xs.Length > 0)
            {
                var s = formsPlot1.Plot.Add.Scatter(xs, ys);
            }

            formsPlot1.Plot.Axes.Title.Label.Text = "Tours (laps) par année";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Nombre de tours";
            formsPlot1.Plot.Axes.AutoScale();
            formsPlot1.Refresh();
        }

        private void PlotDistanceKm()
        {
            if (kms == null || years.Length == 0) return;

            // utiliser la longueur maximale entre kms et miles pour pouvoir tracer les deux séries
            int nAll = Math.Min(years.Length, Math.Max(kms.Length, miles.Length));
            int startYear = trackBar2.Value;
            int endYear = trackBar1.Value;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            // Vérifier si on veut afficher les miles + les kms
            bool showMiles = checkBox2?.Checked == true && miles?.Any(m => !double.IsNaN(m)) == true;

            formsPlot1.Plot.Clear();
            ResetLeftAxisTickGenerator();

            // Afficher seulement les kms
            if (!showMiles)
            {
                var points = Enumerable.Range(0, Math.Min(years.Length, kms.Length))
                    .Select(i => new { Year = years[i], Value = kms[i] })
                    .Where(p => !double.IsNaN(p.Year) && !double.IsNaN(p.Value))
                    .Select(p => new { YearInt = (int)Math.Floor(p.Year), p.Year, p.Value })
                    .Where(p => p.YearInt >= startYear && p.YearInt <= endYear)
                    .ToArray();

                var xs = points.Select(p => p.Year).ToArray();
                var ys = points.Select(p => p.Value).ToArray();

                if (xs.Length > 0)
                {
                    var s = formsPlot1.Plot.Add.Scatter(xs, ys);
                }

                formsPlot1.Plot.Axes.Title.Label.Text = "Distance parcourue par année";
                formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
                formsPlot1.Plot.Axes.Left.Label.Text = "Distance (km)";
                formsPlot1.Plot.Axes.AutoScale();
                formsPlot1.Refresh();
                return;
            }

            // Afficher km et miles simultanément
            var allPoints = Enumerable.Range(0, nAll)
                .Select(i => new
                {
                    Year = i < years.Length ? years[i] : double.NaN,
                    Km = i < kms.Length ? kms[i] : double.NaN,
                    Mi = i < miles.Length ? miles[i] : double.NaN
                })
                .Where(p => !double.IsNaN(p.Year))
                .Select(p => new { YearInt = (int)Math.Floor(p.Year), p.Year, p.Km, p.Mi })
                .Where(p => p.YearInt >= startYear && p.YearInt <= endYear)
                .ToArray();

            var xsAll = allPoints.Select(p => p.Year).ToArray();
            var ysKm = allPoints.Select(p => p.Km).ToArray();
            var ysMi = allPoints.Select(p => p.Mi).ToArray();

            if (xsAll.Length > 0)
            {
                var sKm = formsPlot1.Plot.Add.Scatter(xsAll, ysKm);
                sKm.Label = "km";
                sKm.Color = ScottPlot.Color.FromARGB(System.Drawing.Color.FromArgb(34, 139, 34).ToArgb());

                var sMi = formsPlot1.Plot.Add.Scatter(xsAll, ysMi);
                sMi.Label = "miles";
                sMi.Color = ScottPlot.Color.FromARGB(System.Drawing.Color.FromArgb(255, 140, 0).ToArgb());
            }

            formsPlot1.Plot.ShowLegend();
            formsPlot1.Plot.Axes.Title.Label.Text = "Distance parcourue par année";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Distance (km et miles)";
            formsPlot1.Plot.Axes.AutoScale();
            formsPlot1.Refresh();
        }

        private void PlotTeamWins()
        {
            if (teams == null || teams.Length == 0)
                return;

            // Compter combien de fois chaque équipe apparaît dans le CSV
            var teamCounts = teams
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .GroupBy(t => t)
                .Select(g => new { Team = g.Key, Wins = g.Count() })
                .OrderByDescending(x => x.Wins)
                .ToList();

            // 15 meilleures équipes
            int limit = Math.Min(15, teamCounts.Count);
            string[] teamNames = teamCounts.Take(limit).Select(x => x.Team).ToArray();
            double[] winCounts = teamCounts.Take(limit).Select(x => (double)x.Wins).ToArray();

            formsPlot1.Plot.Clear();

            var bar = formsPlot1.Plot.Add.Bars(winCounts);
            bar.Horizontal = true;

            // Créer les labels sur l'axe Y (noms d’équipes)
            var positions = Enumerable.Range(0, teamNames.Length).Select(i => (double)i).ToArray();
            formsPlot1.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(positions, teamNames);

            formsPlot1.Plot.Axes.Margins(left: 0.4);

            // titres
            formsPlot1.Plot.Axes.Title.Label.Text = "Nombre de victoires par équipe (Top 15)";
            formsPlot1.Plot.Axes.Left.Label.Text = "Équipe";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Nombre de victoires";

            formsPlot1.Refresh();
        }


        private DataTable LoadCsvToDataTable(string csvPath)
        {
            var dt = new DataTable();

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
                    if (fields.Length != dt.Columns.Count)
                    {
                        var adjusted = Enumerable.Range(0, dt.Columns.Count)
                            .Select(i => i < fields.Length ? fields[i] : string.Empty)
                            .ToArray();
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

        // Boutons ---------------------------------------------------------
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Exporter le graphique";
                sfd.Filter = "Image PNG (*.png)|*.png";
                sfd.FileName = "graphique_LeMans";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        formsPlot1.Plot.SavePng(sfd.FileName, 1920, 1080);
                        MessageBox.Show("Graphique exporté avec succès en PNG !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de l'export : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Importer un fichier CSV";
                ofd.Filter = "Fichiers CSV (*.csv)|*.csv";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        csvData = LoadCsvToDataTable(ofd.FileName);

                        var culture = CultureInfo.InvariantCulture;
                        years = csvData.ToDoubleArray("Year", culture);
                        laps = csvData.ToDoubleArray("Laps", culture);
                        kms = csvData.ToDoubleArray("Km", culture);
                        miles = csvData.ToDoubleArray("Mi", culture);
                        avgSpeedsKmh = csvData.ToDoubleArray("Average_speed_kmh", culture);
                        avgSpeedsMph = csvData.ToDoubleArray("Average_speed_mph", culture);
                        teams = csvData.ToStringArray("Team");

                        var validYears = years.Where(y => !double.IsNaN(y)).Select(y => (int)Math.Floor(y)).ToArray();
                        int minYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Min();
                        int maxYear = validYears.DefaultIfEmpty(DateTime.Now.Year).Max();

                        trackBar2.Minimum = minYear;
                        trackBar2.Maximum = maxYear;
                        trackBar2.Value = minYear;

                        trackBar1.Minimum = minYear;
                        trackBar1.Maximum = maxYear;
                        trackBar1.Value = maxYear;

                        lblRange.Text = $"{trackBar2.Value} — {trackBar1.Value}";
                        PositionRangeLabel();

                        UpdatePlot();

                        MessageBox.Show($"Fichier CSV importé avec succès :\n{Path.GetFileName(ofd.FileName)}",
                            "Import réussi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de l'import du fichier : {ex.Message}",
                            "Erreur d'import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnMin_Click(object sender, EventArgs e)
        {
            var vals = GetCurrentPlotValues();
            if (vals == null || vals.Length == 0)
            {
                MessageBox.Show("Aucune donnée visible pour le graphique actuel.", "Statistiques", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selected = comboBox1?.SelectedIndex ?? 0;
            double min = vals.Min();
            string formatted = FormatValue(min, selected);
            string unit = GetValueUnitLabel(selected);
            MessageBox.Show($"Valeur minimale : {formatted}{unit}", "Valeur minimale", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnMax_Click(object sender, EventArgs e)
        {
            var vals = GetCurrentPlotValues();
            if (vals == null || vals.Length == 0)
            {
                MessageBox.Show("Aucune donnée visible pour le graphique actuel.", "Statistiques", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selected = comboBox1?.SelectedIndex ?? 0;
            double max = vals.Max();
            string formatted = FormatValue(max, selected);
            string unit = GetValueUnitLabel(selected);
            MessageBox.Show($"Valeur maximale : {formatted}{unit}", "Valeur minimale", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAvg_Click(object sender, EventArgs e)
        {
            var vals = GetCurrentPlotValues();
            if (vals == null || vals.Length == 0)
            {
                MessageBox.Show("Aucune donnée visible pour le graphique actuel.", "Statistiques", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selected = comboBox1?.SelectedIndex ?? 0;
            double avg = vals.Average();
            string formatted = FormatValue(avg, selected);
            string unit = GetValueUnitLabel(selected);
            MessageBox.Show($"Moyenne : {formatted}{unit}", "Moyenne", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Fin des boutons -------------------------------------------------

        // Récupère les valeurs Y affichées
        private double[] GetCurrentPlotValues()
        {
            int selected = comboBox1?.SelectedIndex ?? 0;

            if (selected == 4)
            {
                if (teams == null || teams.Length == 0) return Array.Empty<double>();

                var teamCounts = teams
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .GroupBy(t => t)
                    .Select(g => new { Team = g.Key, Wins = g.Count() })
                    .OrderByDescending(x => x.Wins)
                    .ToList();

                int limit = Math.Min(15, teamCounts.Count);
                return teamCounts.Take(limit).Select(x => (double)x.Wins).ToArray();
            }

            double[] source;
            switch (selected)
            {
                case 0:
                case 1:
                    source = showingKmh ? avgSpeedsKmh : avgSpeedsMph;
                    break;
                case 2:
                    source = laps;
                    break;
                case 3:
                    source = kms;
                    break;
                default:
                    source = avgSpeedsKmh;
                    break;
            }

            if (years == null || source == null) return Array.Empty<double>();

            int n = Math.Min(years.Length, source.Length);
            int startYear = trackBar2.Value;
            int endYear = trackBar1.Value;
            if (startYear > endYear) (startYear, endYear) = (endYear, startYear);

            var values = Enumerable.Range(0, n)
                .Select(i => new { Y = years[i], V = source[i] })
                .Where(p => !double.IsNaN(p.Y) && !double.IsNaN(p.V))
                .Select(p => new { YearInt = (int)Math.Floor(p.Y), p.V })
                .Where(p => p.YearInt >= startYear && p.YearInt <= endYear)
                .Select(p => p.V)
                .ToArray();

            return values;
        }

        // Formate la valeur afin d'être sûr qu'il n'y ait pas de decimales
        private string FormatValue(double v, int selected)
        {
            if (selected == 4)
                return ((int)Math.Round(v)).ToString(CultureInfo.CurrentCulture);
            return v.ToString("F2", CultureInfo.CurrentCulture);
        }

        private string GetValueUnitLabel(int selected)
        {
            return selected switch
            {
                0 => " km/h",
                1 => " mph",
                2 => " tours",
                3 => " km",
                4 => " victoires",
                _ => string.Empty // retourne une chaîne vide par défaut
            };
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePlot();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePlot();
        }

        private void PositionRangeLabel()
        {
            if (lblRange == null || trackBar2 == null) return;

            lblRange.AutoSize = true;

            // centrer horizontalement
            int x = Math.Max(0, (this.ClientSize.Width - lblRange.Width) / 2);

            lblRange.Location = new Point(x, trackBar2.Location.Y);
        }

        private void ResetLeftAxisTickGenerator()
        {
            // Remet le générateur automatique de valeurs sur l'axe Y
            formsPlot1.Plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
        }
    }

    // Première extension du langage : ajoute ToDoubleArray() qui permet de convertir une colonne DataTable en tableau de double
    public static class DataTableExtensions
    {
        public static double[] ToDoubleArray(this DataTable table, string columnName, CultureInfo culture)
        {
            return table.AsEnumerable()
                .Select(r =>
                {
                    var obj = r.Table.Columns.Contains(columnName) ? r[columnName] : null;
                    var s = obj?.ToString() ?? string.Empty;
                    return double.TryParse(s, NumberStyles.Any, culture, out var v) ? v : double.NaN;
                })
                .ToArray();
        }

        // Deuxième extension du langage : ajoute ToStringArray() qui permet de convertir une colonne DataTable en tableau de string
        public static string[] ToStringArray(this DataTable table, string columnName)
        {
            return table.AsEnumerable()
                .Select(r => r[columnName]?.ToString() ?? string.Empty)
                .ToArray();
        }
    }
}