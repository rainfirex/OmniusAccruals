using OmniusAccruals.Models;
using System;
using System.Windows;

namespace OmniusAccruals.Windows
{
    public partial class SettingWindow : Window
    {
        private Config _cfg;

        public SettingWindow(Config cfg)
        {
            this._cfg = cfg;

            InitializeComponent();
        }

        private void settingForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = String.Format("{0} - Настройка", _cfg.ProgrammName);
            txtBlockRelease.Text = String.Format("Релиз от даты: {0}. Версия {1}. Разработчик: {2}", _cfg.DateRelease.ToShortDateString(), _cfg.Version, _cfg.Programmist);

            txtHost.Text = this._cfg.HOST;
            txtDB.Text = this._cfg.DB;
            txtUser.Text = this._cfg.USER;
            txtPassword.Password = this._cfg.PASSWORD;
            txtTRUSTED.Text = this._cfg.TRUSTED_CONNECTION;
            txtSMTPHost.Text = this._cfg.SMTP_HOST;
            txtSMTPPort.Text = this._cfg.SMTP_PORT.ToString();
            txtSMTPUser.Text = this._cfg.SMTP_USER;
            txtSMTPPassword.Password = this._cfg.SMTP_PASSWORD;
            txtSMTPRECEIVER.Text = this._cfg.RECEIVER_MAIL;
            isFormateUTF8.IsChecked = this._cfg.EXPORT_UTF8;
        }

        private void settingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._cfg.HOST = txtHost.Text.Trim();
            this._cfg.DB = txtDB.Text.Trim();
            this._cfg.USER = txtUser.Text.Trim();
            this._cfg.PASSWORD = txtPassword.Password.Trim();
            this._cfg.TRUSTED_CONNECTION = txtTRUSTED.Text.Trim();
            this._cfg.SMTP_HOST = txtSMTPHost.Text.Trim();
            this._cfg.SMTP_PORT = int.Parse(txtSMTPPort.Text.Trim());
            this._cfg.SMTP_USER = txtSMTPUser.Text.Trim();
            this._cfg.SMTP_PASSWORD = txtSMTPPassword.Password.Trim();
            this._cfg.RECEIVER_MAIL = txtSMTPRECEIVER.Text.Trim();
            this._cfg.EXPORT_UTF8 = (isFormateUTF8.IsChecked == true) ? true : false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
