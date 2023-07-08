using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Data;
using System.Data.Sql;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BK_Restore
{
	public partial class FormLogin : XtraForm
	{
		//private bool isLoadServers = false;

		public FormLogin()
		{
			InitializeComponent();
			txtPassword.Properties.PasswordChar = '•';
			cboServers.Text = "HIEUDV\\HIEUDO";
		}

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
			Program.ServerName = cboServers.Text;
			Program.UserName = txtLogin.Text;
			Program.Password = txtPassword.Text;

			if (Program.Connect() == true)
			{
				FormMain formMain = new FormMain();
				this.Hide();
				formMain.Closed += (s, args) =>
				{
					this.Show();
				};
				formMain.Show();
			}
			else
			{
				txtLogin.Text = "";
				txtPassword.Text = "";
				XtraMessageBox.Show("Đăng nhập thất bại", "ERROR",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void btn_Exit_Click_1(object sender, EventArgs e)
        {
			this.Close();
		}
    }
}
