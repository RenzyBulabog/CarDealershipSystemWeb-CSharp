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
    public partial class AdminSales : Form
    {
        public AdminSales()
        {
            InitializeComponent();
        }

        // ✅ LOAD (FIXED)
        private async void AdminSales_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadSales();
        }

        // 🔥 FULL GRID STYLE (CONSISTENT)
        private void StyleGrid()
        {
            dgvSales.EnableHeadersVisualStyles = false;

            // HEADER
            dgvSales.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvSales.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSales.ColumnHeadersHeight = 40;

            // ROWS
            dgvSales.DefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);
            dgvSales.DefaultCellStyle.ForeColor = Color.White;

            // ❗ REMOVE WHITE ROWS
            dgvSales.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);

            // SELECTION
            dgvSales.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvSales.DefaultCellStyle.SelectionForeColor = Color.Black;

            // GRID LOOK
            dgvSales.GridColor = Color.FromArgb(30, 41, 59);
            dgvSales.BackgroundColor = Color.FromArgb(15, 23, 42);

            // CLEAN UI
            dgvSales.BorderStyle = BorderStyle.None;
            dgvSales.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvSales.RowHeadersVisible = false;

            // 🔥 IMPORTANT
            dgvSales.AllowUserToAddRows = false;
            dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSales.RowTemplate.Height = 35;
        }

        // 🔥 LOAD SALES (FIXED)
        private async Task LoadSales()
        {
            try
            {
                var sales = await ApiService.Get<List<Sale>>("sales");

                var pending = sales
                    .Where(s => (s.status ?? "").ToLower() == "pending")
                    .ToList();

                dgvSales.DataSource = pending;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales: " + ex.Message);
            }
        }

        // 🔥 DIRECT SELL
        private async void btnSell_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCarId.Text, out int carId))
            {
                MessageBox.Show("Invalid Car ID");
                return;
            }

            try
            {
                var car = await ApiService.Get<Car>("cars/" + carId);

                if (car == null || car.stock <= 0)
                {
                    MessageBox.Show("No stock available");
                    return;
                }

                // create sale
                var success = await ApiService.Post("sales", new
                {
                    carId = carId,
                    customerName = txtCustomer.Text,
                    status = "Approved"
                });

                if (!success)
                {
                    MessageBox.Show("Sale failed");
                    return;
                }

                // deduct stock
                car.stock--;
                await ApiService.Put("cars/" + carId, car);

                MessageBox.Show("Sale completed!");
                await LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔥 APPROVE SELECTED
        private async void btnApprove_Click(object sender, EventArgs e)
        {
            if (dgvSales.CurrentRow == null) return;

            var sale = (Sale)dgvSales.CurrentRow.DataBoundItem;

            if ((sale.status ?? "").ToLower() == "approved")
            {
                MessageBox.Show("Already approved");
                return;
            }

            try
            {
                var car = await ApiService.Get<Car>("cars/" + sale.carId);

                if (car == null || car.stock <= 0)
                {
                    MessageBox.Show("No stock available");
                    return;
                }

                // update sale
                sale.status = "Approved";
                await ApiService.Put("sales/" + sale.id, sale);

                // deduct stock
                car.stock--;
                await ApiService.Put("cars/" + car.id, car);

                MessageBox.Show("Sale approved!");
                await LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}