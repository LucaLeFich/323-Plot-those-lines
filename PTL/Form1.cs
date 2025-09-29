using ScottPlot;
using ScottPlot.WinForms;
using System.Data;

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
            string csvPath = @"D:\323- programation fonctionnelle\Projet\323-Plot-those-lines\PTL\data_LeMans_race_winners.csv";
            csvData = LoadCsvToDataTable(csvPath);

            string columns = string.Join(", ", csvData.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            MessageBox.Show("Colonnes disponibles : " + columns);

            double[] years = csvData.AsEnumerable().Select(r => Convert.ToDouble(r["Year"])).ToArray();
            double[] speeds = csvData.AsEnumerable().Select(r => Convert.ToDouble(r["Average_speed_kmh"])).ToArray();

            var scatter = formsPlot1.Plot.Add.Scatter(years, speeds);
            formsPlot1.Plot.Axes.Title.Label.Text = "Vitesse moyenne des vainqueurs au Mans";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Année";
            formsPlot1.Plot.Axes.Left.Label.Text = "Vitesse moyenne (km/h)";

            formsPlot1.Refresh();
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
    }
}
