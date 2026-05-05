using CarDealership.Services;
using CarDealership.Utils;
using System;
using System.Text;
using System.Windows.Forms;

namespace CarDealership.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // 🔥 ENTER KEY = LOGIN
            this.AcceptButton = btnLogin;
        }

        // 🔥 LOGIN BUTTON
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            var user = new
            {
                username = txtUsername.Text,
                password = txtPassword.Text
            };

            try
            {
                // 🔥 CALL API (GET RESPONSE DATA)
                var data = await ApiService.PostWithResponse<dynamic>("Auth/login", user);

                if (data == null)
                {
                    MessageBox.Show("Login Failed");
                    return;
                }

                // 🔥 SAVE SESSION
                Session.Username = data.username;
                Session.Role = data.role;

                // 🔥 REDIRECT BASED ON ROLE
                if (Session.Role == "admin")
                    new AdminDashboard().Show();
                else
                    new CustomerDashboard().Show();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // 🔥 GO TO REGISTER
        private void lblRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            new RegisterForm().Show();
        }

        // 🔥 CLOSE APP
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}