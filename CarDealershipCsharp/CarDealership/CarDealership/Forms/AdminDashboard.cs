using CarDealership.Models;
using CarDealership.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarDealership.Forms
{
    public partial class AdminDashboard : Form
    {
        // 🔥 STORE DASHBOARD CONTROLS
        List<Control> dashboardControls = new List<Control>();

        public AdminDashboard()
        {
            InitializeComponent();
            this.Load += AdminDashboard_Load;
        }

        // 🔥 FORM LOAD
        private async void AdminDashboard_Load(object sender, EventArgs e)
        {
            // 🔥 SAVE ORIGINAL DASHBOARD UI
            foreach (Control ctrl in panelBody.Controls)
            {
                dashboardControls.Add(ctrl);
            }

            await LoadStats();
        }

        // 🔥 LOAD STATS
        private async Task LoadStats()
        {
            try
            {
                var cars = await ApiService.Get<List<Car>>("cars");
                var sales = await ApiService.Get<List<Sale>>("sales");

                var approved = sales
                    .Where(s => (s.status ?? "").ToLower() == "approved")
                    .ToList();

                int totalCars = cars.Count;
                int totalStock = cars.Sum(c => c.stock);
                int sold = approved.Count;

                double revenue = 0;

                foreach (var s in approved)
                {
                    var car = cars.FirstOrDefault(c => c.id == s.carId);
                    if (car != null)
                        revenue += car.price;
                }

                lblTotalCars.Text = totalCars.ToString();
                lblAvailable.Text = totalStock.ToString();
                lblSold.Text = sold.ToString();
                lblRevenue.Text = "₱" + revenue.ToString("N0");
                lblTotalSales.Text = sold.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard: " + ex.Message);
            }
        }

        // 🔥 LOAD FORM INTO PANEL
        private void LoadForm(Form form)
        {
            panelBody.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            panelBody.Controls.Add(form);
            form.Show();
        }

        // 🔥 SIDEBAR BUTTONS

        private void btnCars_Click(object sender, EventArgs e)
        {
            LoadForm(new AdminCars());
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            LoadForm(new AdminInventory());
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            LoadForm(new AdminSales());
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            LoadForm(new AdminSalesHistory());
        }

        // 🔥 DASHBOARD BUTTON (FIXED)
        private async void btnDashboard_Click(object sender, EventArgs e)
        {
            panelBody.Controls.Clear();

            // 🔥 RESTORE DASHBOARD UI
            foreach (Control ctrl in dashboardControls)
            {
                panelBody.Controls.Add(ctrl);
            }

            await LoadStats();
        }

        // 🔥 LOGOUT
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }
    }
}