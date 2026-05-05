using CarDealership.Services;
using System;
using System.Windows.Forms;

namespace CarDealership.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            CenterCard();

            // 🔥 DEFAULT ROLE
            cmbRole.SelectedIndex = 0; // customer default
        }

        private void CenterCard()
        {
            panelCard.Left = (this.ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = (this.ClientSize.Height - panelCard.Height) / 2;
        }

        // 🔥 REGISTER
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            if (string.IsNullOrEmpty(cmbRole.Text))
            {
                MessageBox.Show("Select a role");
                return;
            }

            var payload = new
            {
                username = txtUsername.Text,
                password = txtPassword.Text,
                role = cmbRole.Text, // 🔥 dynamic role
                fullname = txtFullname.Text,
                email = txtEmail.Text
            };

            try
            {
                var success = await ApiService.Post("auth/register", payload);

                if (success)
                {
                    MessageBox.Show("Registered Successfully!");
                    this.Hide();
                    new LoginForm().Show();
                }
                else
                {
                    MessageBox.Show("Error registering user");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("API ERROR: " + ex.Message);
            }
        }

        // 🔙 Back to Login
        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }
    }
}