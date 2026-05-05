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
    public partial class AdminCars : Form
    {
        public AdminCars()
        {
            InitializeComponent();
        }

        // ✅ LOAD
        private async void AdminCars_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadCars();
        }

        // 🔥 FULL GRID STYLE (FIXED)
        private void StyleGrid()
        {
            dgvCars.EnableHeadersVisualStyles = false;

            // HEADER
            dgvCars.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvCars.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCars.ColumnHeadersHeight = 40;

            // ROWS
            dgvCars.DefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);
            dgvCars.DefaultCellStyle.ForeColor = Color.White;

            // ❗ REMOVE WHITE ROWS
            dgvCars.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);

            // SELECTION
            dgvCars.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvCars.DefaultCellStyle.SelectionForeColor = Color.Black;

            // GRID LOOK
            dgvCars.GridColor = Color.FromArgb(30, 41, 59);
            dgvCars.BackgroundColor = Color.FromArgb(15, 23, 42);

            // REMOVE EXTRA UI
            dgvCars.BorderStyle = BorderStyle.None;
            dgvCars.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCars.RowHeadersVisible = false;

            // 🔥 IMPORTANT FIXES
            dgvCars.AllowUserToAddRows = false; // removes empty white row
            dgvCars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // OPTIONAL (better spacing)
            dgvCars.RowTemplate.Height = 35;
        }

        // 🔥 LOAD DATA
        private async Task LoadCars()
        {
            try
            {
                var cars = await ApiService.Get<List<Car>>("cars");
                dgvCars.DataSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cars: " + ex.Message);
            }
        }

        // 🔥 ADD
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var car = new Car
                {
                    brand = txtBrand.Text,
                    model = txtModel.Text,
                    year = int.Parse(txtYear.Text),
                    price = double.Parse(txtPrice.Text),
                    status = txtStatus.Text,
                    stock = 0
                };

                var success = await ApiService.Post("cars", car);

                if (success)
                {
                    await LoadCars();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Failed to add car");
                }
            }
            catch
            {
                MessageBox.Show("Invalid input");
            }
        }

        // 🔥 UPDATE
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCars.CurrentRow == null) return;

            var car = (Car)dgvCars.CurrentRow.DataBoundItem;

            try
            {
                car.brand = txtBrand.Text;
                car.model = txtModel.Text;
                car.year = int.Parse(txtYear.Text);
                car.price = double.Parse(txtPrice.Text);
                car.status = txtStatus.Text;

                var success = await ApiService.Put("cars/" + car.id, car);

                if (success)
                {
                    await LoadCars();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Update failed");
                }
            }
            catch
            {
                MessageBox.Show("Invalid input");
            }
        }

        // 🔥 DELETE
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCars.CurrentRow == null) return;

            var car = (Car)dgvCars.CurrentRow.DataBoundItem;

            var confirm = MessageBox.Show("Delete this car?", "Confirm", MessageBoxButtons.YesNo);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    await ApiService.Delete("cars/" + car.id);
                    await LoadCars();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message);
                }
            }
        }

        // 🔥 CLICK ROW
        private void dgvCars_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var car = (Car)dgvCars.Rows[e.RowIndex].DataBoundItem;

            txtBrand.Text = car.brand;
            txtModel.Text = car.model;
            txtYear.Text = car.year.ToString();
            txtPrice.Text = car.price.ToString("N0"); // formatted
            txtStatus.Text = car.status;
        }

        // 🔥 SEARCH
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var cars = await ApiService.Get<List<Car>>("cars");

                var filtered = cars
                    .Where(c => (c.brand + " " + c.model)
                    .ToLower()
                    .Contains(txtSearch.Text.ToLower()))
                    .ToList();

                dgvCars.DataSource = filtered;
            }
            catch
            {
                // silent
            }
        }

        // 🔥 CLEAR
        private void ClearFields()
        {
            txtBrand.Clear();
            txtModel.Clear();
            txtYear.Clear();
            txtPrice.Clear();
            txtStatus.Clear();
        }
    }
}