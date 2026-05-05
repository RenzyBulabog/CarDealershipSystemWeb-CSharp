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
    public partial class AdminInventory : Form
    {
        public AdminInventory()
        {
            InitializeComponent();
        }

        // ✅ LOAD (FIXED)
        private async void AdminInventory_Load(object sender, EventArgs e)
        {
            StyleGrid();
            await LoadInventory();
        }

        // 🔥 FULL GRID STYLE (SAME AS CARS – FIXED)
        private void StyleGrid()
        {
            dgvInventory.EnableHeadersVisualStyles = false;

            // HEADER
            dgvInventory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 41, 59);
            dgvInventory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInventory.ColumnHeadersHeight = 40;

            // ROWS
            dgvInventory.DefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);
            dgvInventory.DefaultCellStyle.ForeColor = Color.White;

            // ❗ REMOVE WHITE ROWS
            dgvInventory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(2, 6, 23);

            // SELECTION
            dgvInventory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(56, 189, 248);
            dgvInventory.DefaultCellStyle.SelectionForeColor = Color.Black;

            // GRID LOOK
            dgvInventory.GridColor = Color.FromArgb(30, 41, 59);
            dgvInventory.BackgroundColor = Color.FromArgb(15, 23, 42);

            // CLEAN LOOK
            dgvInventory.BorderStyle = BorderStyle.None;
            dgvInventory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvInventory.RowHeadersVisible = false;

            // 🔥 IMPORTANT
            dgvInventory.AllowUserToAddRows = false;
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.RowTemplate.Height = 35;
        }

        // 🔥 LOAD INVENTORY (FIXED)
        private async Task LoadInventory()
        {
            try
            {
                var cars = await ApiService.Get<List<Car>>("cars");
                dgvInventory.DataSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory: " + ex.Message);
            }
        }

        // 🔥 CLICK ROW → AUTO FILL STOCK
        private void dgvInventory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var car = (Car)dgvInventory.Rows[e.RowIndex].DataBoundItem;
            txtStock.Text = car.stock.ToString();
        }

        // 🔥 UPDATE STOCK
        private async void btnUpdateStock_Click(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow == null) return;

            var car = (Car)dgvInventory.CurrentRow.DataBoundItem;

            if (!int.TryParse(txtStock.Text, out int newStock))
            {
                MessageBox.Show("Invalid stock value");
                return;
            }

            try
            {
                car.stock = newStock;

                var success = await ApiService.Put("cars/" + car.id, car);

                if (success)
                {
                    MessageBox.Show("Stock updated!");
                    await LoadInventory();
                }
                else
                {
                    MessageBox.Show("Update failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message);
            }
        }
    }
}