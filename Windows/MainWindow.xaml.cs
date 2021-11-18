using System;
using System.Windows;
using OmniusAccruals.Models;
using System.ComponentModel;
using OmniusAccruals.Modules;
using Pinger.Modules;

namespace OmniusAccruals.Windows
{
    public partial class MainWindow : Window
    {
        private BindingList<Division> _listData;

        private Config cfg = new Config();

        IniFile INI = new IniFile("config.ini");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = String.Format("{0} v.{1} - ЮР начисления", cfg.ProgrammName, cfg.Version);
            if (INI.KeyExists("host", "main")) cfg.HOST = INI.ReadINI("main", "host");
            if (INI.KeyExists("base", "main")) cfg.DB   = INI.ReadINI("main", "base");
            if (INI.KeyExists("user", "main")) cfg.USER = INI.ReadINI("main", "user");
            if (INI.KeyExists("password", "main")) cfg.PASSWORD = Security.DeCrypt(INI.ReadINI("main", "password"),"test");
            if (INI.KeyExists("trusted_con", "main")) cfg.TRUSTED_CONNECTION = INI.ReadINI("main", "trusted_con");

            if (INI.KeyExists("host", "smtp"))     cfg.SMTP_HOST = INI.ReadINI("smtp", "host");
            if (INI.KeyExists("port", "smtp"))     cfg.SMTP_PORT = int.Parse(INI.ReadINI("smtp", "port"));
            if (INI.KeyExists("user", "smtp"))     cfg.SMTP_USER = INI.ReadINI("smtp", "user");
            if (INI.KeyExists("password", "smtp")) cfg.SMTP_PASSWORD = Security.DeCrypt(INI.ReadINI("smtp", "password"), "test8");
            if (INI.KeyExists("receiver", "mail")) cfg.RECEIVER_MAIL = INI.ReadINI("mail", "receiver");

            if (INI.KeyExists("export-utf8", "data")) cfg.EXPORT_UTF8 = Convert.ToBoolean(INI.ReadINI("data", "export-utf8"));

            OmniusDB.setConfig(cfg);

            pickerPeriod.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            try
            {
                this._listData = OmniusDB.getDivisions();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Ошибка получение данных", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            listDivisions.ItemsSource = _listData;
            listDivisions.DisplayMemberPath = "C_Name";
        }

        private void btnGetDivisions_Click(object sender, RoutedEventArgs e)
        {
            if (listDivisions.SelectedItem != null && pickerPeriod.SelectedDate != null)
            {
                //System.Diagnostics.Debug.WriteLine(((Division)listDivisions.SelectedItem).LINK);

                DateTime d = (DateTime)pickerPeriod.SelectedDate;
                string period = d.ToString("yyyy")+ d.ToString("MM");

                BindingList<Accrual> list = OmniusDB.getAccruals((Division)listDivisions.SelectedItem, period);

                if(list.Count > 0 || ((Division)listDivisions.SelectedItem).LINK == -1)
                {
                    try
                    {
                        btnGet.IsEnabled = false;
                        DataWindow frm = new DataWindow(list, (Division)listDivisions.SelectedItem, period, cfg);
                        frm.ShowDialog();
                        btnGet.IsEnabled = true;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }                    
                }
                else
                {
                    MessageBox.Show("Данные отсутствуют", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Укажите дату и выберите отделение", "Отказ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow frm = new SettingWindow(cfg);
            frm.ShowDialog();
        }

        private void mainForm_Closing(object sender, CancelEventArgs e)
        {
            INI.Write("main", "host", cfg.HOST);
            INI.Write("main", "base", cfg.DB);
            INI.Write("main", "user", cfg.USER);
            INI.Write("main", "password", Security.Crypt(cfg.PASSWORD, "test"));
            INI.Write("main", "trusted_con", cfg.TRUSTED_CONNECTION);

            INI.Write("smtp", "host", cfg.SMTP_HOST);
            INI.Write("smtp", "port", cfg.SMTP_PORT.ToString());
            INI.Write("smtp", "user", cfg.SMTP_USER);
            INI.Write("smtp", "password", Security.Crypt(cfg.SMTP_PASSWORD, "test8"));

            INI.Write("mail", "receiver", cfg.RECEIVER_MAIL);

            INI.Write("data", "export-utf8", cfg.EXPORT_UTF8.ToString());
        }        
    }
}
