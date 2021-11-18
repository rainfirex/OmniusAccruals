using OmniusAccruals.Models;
using OmniusAccruals.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace OmniusAccruals.Windows
{
    public partial class DataWindow : Window
    {
        private Config _cfg;
        BindingList<Accrual> _list;
        Division _division;
        string _period;

        public DataWindow(BindingList<Accrual> list, Division division, string period, Config cfg)
        {
            this._list = list;
            this._division = division;
            this._period = period;
            this._cfg = cfg;

            InitializeComponent();
        }

        private void dataForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"Начисление за {this._period} по \"{this._division.C_Name.Replace("<", "").Replace(">", "")}\"";
            this.lblEncoding.Content = String.Format("Выбрана кодировка: {0}\nПолучатель: {1}", ((_cfg.EXPORT_UTF8) ? "UTF-8" : "Windows-1251"), _cfg.RECEIVER_MAIL);
            dataGrid.ItemsSource = this._list;
            lblListCount.Content = String.Format("Количество записей: {0} на период {1}. Отделение: \"{2}\", код - \"{3}\"", this._list.Count, this._period, this._division.C_Name, this._division.LINK);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string fileName = String.Format("{0}.{1}_full.csv", this._division.C_Name.Replace("<", "").Replace(">", ""), this._period);
            CSVData.exportFull(this._list, fileName, _cfg.EXPORT_UTF8);
            MessageBox.Show($"Сохранение завершено!\nФайл \"{fileName}\" находиться  на рабочем столе.", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExportSB_Click(object sender, RoutedEventArgs e)
        {
            //string fileName = String.Format("{0}.{1}_SB.csv", this._division.C_Name.Replace("<","").Replace(">",""), this._period);

            string fileName = String.Format("6500000024_40702810150340102662_002_{0}.txt", DateTime.Now.ToString("ddMMyyyy"));

            string path = CSVData.exportSB(this._list, fileName, _cfg.EXPORT_UTF8);

            if(!String.IsNullOrEmpty(path))
            {
                btnExportSB.IsEnabled = false;
                MessageBox.Show($"Сохранение завершено!\nФайл \"{fileName}\" находиться  на рабочем столе.", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                if (MessageBox.Show($"Отправить файл по адресу \"{_cfg.RECEIVER_MAIL}\"?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        new SendMail(_cfg.SMTP_HOST, _cfg.SMTP_PORT, _cfg.SMTP_USER, _cfg.SMTP_PASSWORD)
                            .InitAddress("robot@sakh.dvec.ru", "robot", _cfg.RECEIVER_MAIL)
                            .AddText($"Реестр для СБЕРа – ЮЛ и ИП за {_period}", $"<p>Реестр для СБЕРа – ЮЛ и ИП  {_period}</p>")
                            .AddFile(path)
                            .send();
                        MessageBox.Show("Отправка завершена.", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                        //SendMail.sendOne("robot@sakh.dvec.ru", "robot", "Poplavskiy-AA@sakh.dvec.ru", "TEST", "<h1>!!!</h1>", _cfg.SMTP_HOST, 25, _cfg.SMTP_USER, _cfg.SMTP_PASSWORD);

                        //new SendMail(_cfg.SMTP_HOST, _cfg.SMTP_PORT, _cfg.SMTP_USER, _cfg.SMTP_PASSWORD)
                        //    .InitAddress("robot@sakh.dvec.ru", "robot", _cfg.RECEIVER_MAIL)
                        //    .AddText($"Начисление за {_period}", "<h2>!!!!TEST NEW 4321</h2>")
                        //    .send();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.InnerException.Message, error.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                btnExportSB.IsEnabled = true;
            }
            else 
            {
                MessageBox.Show(String.Format("Нет данных c подходящими условиями:\n[{0},\n{1},\n{2}]\nОперация отменена.", CSVData.FI_TYPE, CSVData.FI_TYPE_RASCHET, CSVData.FI_TYPE_ZADOLZN), "Рузультат", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string textFormat(string text)
        {
            return text.ToLower()
                .Replace(@"\", "").Replace("\"", "")
                .Replace("ооо", "").Replace("оао", "")
                .Replace("ип", "").Replace("снт", "")
                .Replace("гаук", "").Replace("зао", "").Trim();
        }

        private void search()
        {
            string searchText = textFormat(txtSearch.Text.Trim());

            if (String.IsNullOrEmpty(searchText) || searchText.Length <= 3)
            {
                dataGrid.ItemsSource = _list; ;
                return;
            }

            var filtered = _list.Where(item => {
                
                string itemString = textFormat(item.C_FIO);
                bool result = itemString.StartsWith(searchText);

                if(result) return result;

                itemString = item.SD_Subscr_CodeNumber.ToString();
                result = itemString.StartsWith(searchText);
                
                if (result) return result;

                return false;
            });

            dataGrid.ItemsSource = (List<Accrual>)filtered.ToList();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (String.IsNullOrEmpty(searchText))
            {
                dataGrid.ItemsSource = _list; ;
            }
        }

        private void txtSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                search();
        }        

        private void dataGrid_Sorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
        {
            //DataGrid dataGrid = (DataGrid)sender;
            //BindingListCollectionView lcv = (BindingListCollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            IEnumerable<Accrual> sorted;
            ListSortDirection? direction = (e.Column.SortDirection == null) ? ListSortDirection.Ascending : ListSortDirection.Descending;
            e.Handled = true;
            e.Column.SortDirection = direction;
            string column = e.Column.SortMemberPath;

            switch (column)
            {
                case "SD_Subscr_CodeNumber":
                    sorted = (direction == ListSortDirection.Ascending) ? 
                        _list.OrderBy(x => x.SD_Subscr_CodeNumber) :
                        _list.OrderByDescending(x => x.SD_Subscr_CodeNumber);
                    break;
                case "C_FIO":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.C_FIO) :
                        _list.OrderByDescending(x => x.C_FIO);
                    break;
                case "_Bound_F_Debts":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x._Bound_F_Debts) :
                        _list.OrderByDescending(x => x._Bound_F_Debts);
                    break;
                case "C_Doc_linked":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.C_Doc_linked) :
                        _list.OrderByDescending(x => x.C_Doc_linked);
                    break;
                case "_Bound_F_Doc_Types":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x._Bound_F_Doc_Types) :
                        _list.OrderByDescending(x => x._Bound_F_Doc_Types);
                    break;
                case "D_Date":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.D_Date) :
                        _list.OrderByDescending(x => x.D_Date);
                    break;
                case "C_Number":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.C_Number) :
                        _list.OrderByDescending(x => x.C_Number);
                    break;
                case "N_Number":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Number) :
                        _list.OrderByDescending(x => x.N_Number);
                    break;
                case "FD_Documents_C_Period_Name":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.FD_Documents_C_Period_Name) :
                        _list.OrderByDescending(x => x.FD_Documents_C_Period_Name);
                    break;

                case "_Bound_F_Status":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x._Bound_F_Status) :
                        _list.OrderByDescending(x => x._Bound_F_Status);
                    break;
                case "D_Post_Date":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.D_Post_Date) :
                        _list.OrderByDescending(x => x.D_Post_Date);
                    break;
                case "C_DT_Period":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.C_DT_Period) :
                        _list.OrderByDescending(x => x.C_DT_Period);
                    break;
                case "N_Quantity":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Quantity) :
                        _list.OrderByDescending(x => x.N_Quantity);
                    break;
                case "_Bound_F_Calc_Algorithms":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x._Bound_F_Calc_Algorithms) :
                        _list.OrderByDescending(x => x._Bound_F_Calc_Algorithms);
                    break;
                case "N_AmountWithoutTax":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_AmountWithoutTax) :
                        _list.OrderByDescending(x => x.N_AmountWithoutTax);
                    break;
                case "N_Tax_Amount":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Tax_Amount) :
                        _list.OrderByDescending(x => x.N_Tax_Amount);
                    break;
                case "N_Amount":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Amount) :
                        _list.OrderByDescending(x => x.N_Amount);
                    break;
                case "D_Date_Due":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.D_Date_Due) :
                        _list.OrderByDescending(x => x.D_Date_Due);
                    break;
                case "N_Amount_Open":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Amount_Open) :
                        _list.OrderByDescending(x => x.N_Amount_Open);
                    break;
                case "N_Cost_Decrease":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Cost_Decrease) :
                        _list.OrderByDescending(x => x.N_Cost_Decrease);
                    break;
                case "N_Tax_Amount_Decrease":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Tax_Amount_Decrease) :
                        _list.OrderByDescending(x => x.N_Tax_Amount_Decrease);
                    break;
                case "N_Amount_Decrease":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Amount_Decrease) :
                        _list.OrderByDescending(x => x.N_Amount_Decrease);
                    break;
                case "N_Cost_Increase":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Cost_Increase) :
                        _list.OrderByDescending(x => x.N_Cost_Increase);
                    break;
                case "N_Tax_Amount_Increase":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Tax_Amount_Increase) :
                        _list.OrderByDescending(x => x.N_Tax_Amount_Increase);
                    break;
                case "N_Amount_Increase":
                    sorted = (direction == ListSortDirection.Ascending) ?
                        _list.OrderBy(x => x.N_Amount_Increase) :
                        _list.OrderByDescending(x => x.N_Amount_Increase);
                    break;
                default: return;
            }
            dataGrid.ItemsSource = new BindingList<Accrual>(sorted.ToList());
        }
    }
}
