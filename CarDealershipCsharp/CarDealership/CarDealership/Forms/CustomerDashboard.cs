using CarDealership.Models;
using CarDealership.Services;
using CarDealership.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarDealership.Forms
{
    public partial class CustomerDashboard : Form
    {
        // 🔥 CACHE FOR SEARCH
        List<Car> allCars = new List<Car>();

        public CustomerDashboard()
        {
            InitializeComponent();
        }

        // ✅ LOAD
        private async void CustomerDashboard_Load(object sender, EventArgs e)
        {
            StyleGrid();
            btnBuy.Enabled = false;

            // 🔥 CONNECT SEARCH EVENT ONLY
            txtSearch.TextChanged += txtSearch_TextChanged;

            await LoadCars();
        }

        // 🔥 GRID STYLE
        private void StyleGrid()
        {
            dgvCars.EnableHeadersVisualStyles = false;

            dgvCars.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvCars.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCars.ColumnHeadersHeight = 40;

            dgvCars.DefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);
            dgvCars.DefaultCellStyle.ForeColor = Color.White;

            dgvCars.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);

            dgvCars.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvCars.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvCars.GridColor = Color.FromArgb(30, 41, 59);
            dgvCars.BackgroundColor = Color.FromArgb(15, 23, 42);

            dgvCars.BorderStyle = BorderStyle.None;
            dgvCars.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCars.RowHeadersVisible = false;

            dgvCars.AllowUserToAddRows = false;
            dgvCars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCars.RowTemplate.Height = 35;
        }

        // 🔥 LOAD CARS
        private async Task LoadCars()
        {
            try
            {
                var cars = await ApiService.Get<List<Car>>("cars");

                allCars = cars
                    .Where(c => c.stock > 0)
                    .ToList();

                dgvCars.DataSource = allCars;

                btnBuy.Enabled = false;
                txtCarId.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cars: " + ex.Message);
            }
        }

        // 🔥 SELECT ROW (UNCHANGED)
        private void dgvCars_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var car = (Car)dgvCars.Rows[e.RowIndex].DataBoundItem;
            txtCarId.Text = car.id.ToString();

            btnBuy.Enabled = true;
        }

        // 🔥 SEARCH (ADDED ONLY)
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var filtered = allCars
                .Where(c =>
                    (c.brand + " " + c.model)
                    .ToLower()
                    .Contains(txtSearch.Text.ToLower())
                )
                .ToList();

            dgvCars.DataSource = filtered;
        }

        // 🔥 BUY
        private async void btnBuy_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCarId.Text, out int carId))
            {
                MessageBox.Show("Select a car first");
                return;
            }

            try
            {
                var success = await ApiService.Post("sales", new
                {
                    carId = carId,
                    customerName = Session.Username,
                    status = "Pending"
                });

                if (success)
                {
                    MessageBox.Show("Purchase request sent!");
                    await LoadCars();
                }
                else
                {
                    MessageBox.Show("Request failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔥 LOGOUT
        private void btnLogout_Click(object sender, EventArgs e)
        {
            new LoginForm().Show();
            this.Close();
        }
    }
}