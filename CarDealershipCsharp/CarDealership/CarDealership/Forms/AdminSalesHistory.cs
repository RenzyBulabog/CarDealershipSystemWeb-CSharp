using CarDealership.Models;
using CarDealership.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarDealership.Forms
{
    public partial class AdminSalesHistory : Form
    {
        public AdminSalesHistory()
        {
            InitializeComponent();
        }

        // ✅ LOAD (FIXED)
        private async void AdminSalesHistory_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadHistory();
        }

        // 🔥 FULL GRID STYLE (CONSISTENT)
        private void StyleGrid()
        {
            dgvHistory.EnableHeadersVisualStyles = false;

            // HEADER
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersHeight = 40;

            // ROWS
            dgvHistory.DefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);
            dgvHistory.DefaultCellStyle.ForeColor = Color.White;

            // ❗ REMOVE WHITE ROWS
            dgvHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);

            // SELECTION
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.Black;

            // GRID LOOK
            dgvHistory.GridColor = Color.FromArgb(30, 41, 59);
            dgvHistory.BackgroundColor = Color.FromArgb(15, 23, 42);

            // CLEAN UI
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHistory.RowHeadersVisible = false;

            // 🔥 IMPORTANT
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.RowTemplate.Height = 35;
        }

        // 🔥 LOAD HISTORY (APPROVED ONLY)
        private async Task LoadHistory()
        {
            try
            {
                var sales = await ApiService.Get<List<Sale>>("sales");
                var cars = await ApiService.Get<List<Car>>("cars");

                var approved = sales
                    .Where(s => (s.status ?? "").ToLower() == "approved")
                    .ToList();

                dgvHistory.DataSource = approved;

                // 🔥 TOTALS
                int totalSales = approved.Count;

                double revenue = 0;
                foreach (var s in approved)
                {
                    var car = cars.FirstOrDefault(c => c.id == s.carId);
                    if (car != null)
                        revenue += car.price;
                }

                lblTotalSales.Text = "Total Sales: " + totalSales;
                lblRevenue.Text = "Total Revenue: ₱" + revenue.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading history: " + ex.Message);
            }
        }
    }
}