using OmniusAccruals.Models;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System;

namespace OmniusAccruals.Modules
{
    class OmniusDB
    {
        private static string HOST;
        private static string DB;
        private static string USER;
        private static string PASSWORD;
        private static string TRUSTED_CONNECTION;

        internal static void setConfig(Config cfg)
        {
            //Integrated Security=True;
            HOST = cfg.HOST;
            DB = cfg.DB;
            USER = cfg.USER;
            PASSWORD = cfg.PASSWORD;
            TRUSTED_CONNECTION = cfg.TRUSTED_CONNECTION;
        }

        public static DataTable getDataDivisions()
        {
            DataTable table = new DataTable("divisions");
            string query = "SELECT LINK, C_Name FROM ORL_RV_Divisions_List";
            string tns = $"server={HOST};Trusted_Connection={TRUSTED_CONNECTION};DataBase={DB};User ID={USER};Password={PASSWORD};";
            SqlConnection connection = new SqlConnection(tns);
            connection.Open();
            SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;
            connection.Close();
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(table);
            connection.Close();
            return table;
        }

        public static BindingList<Division> getDivisions()
        {
            BindingList<Division> list = new BindingList<Division>();
            string query = "SELECT LINK, C_Name FROM ORL_RV_Divisions_List";
            string tns = $"server={HOST};Trusted_Connection={TRUSTED_CONNECTION};DataBase={DB};User ID={USER};Password={PASSWORD};";
            SqlConnection connection = new SqlConnection(tns);
            connection.Open();
            SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;

            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Division(reader.GetInt32(0), reader.GetString(1)));
            }
            connection.Close();
            return list;
        }

        public static BindingList<Accrual> getAccruals(Division division, string period)
        {
            BindingList<Accrual> list = new BindingList<Accrual>();

            string query;
            if (division.LINK == -1)
            {
                query = "SELECT	I.SD_Subscr_CodeNumber,	I.C_FIO, I._Bound_F_Debts, I.C_Doc_Linked, I._Bound_F_Doc_Types, convert(varchar,I.D_Date,104) AS D_Date, I.C_Number, I.N_Number," +
        "I.FD_Documents_C_Period_Name, I._Bound_F_Status, convert(varchar,I.D_Post_Date,104) AS D_Post_Date, I.C_DT_Period, I.N_Quantity,I._Bound_F_Calc_Algorithms, I.N_AmountWithoutTax," +
        "I.N_Tax_Amount, I.N_Amount, convert(varchar,I.D_Date_Due,104) AS D_Date_Due, I.N_Amount_Open, I.N_Amount_Decrease - I.N_Tax_Amount_Decrease AS N_Cost_Decrease, I.N_Tax_Amount_Decrease," +
        $"I.N_Amount_Decrease, I.N_Amount_Increase - I.N_Tax_Amount_Increase AS N_Cost_Increase, I.N_Tax_Amount_Increase, I.N_Amount_Increase FROM EE.UI_FV_Invoices AS I WHERE I.N_Period = {period} AND I.F_Division IN (16, 17, 18, 19, 20)";
            }
            else
            {
                query = "SELECT	I.SD_Subscr_CodeNumber,	I.C_FIO, I._Bound_F_Debts, I.C_Doc_Linked, I._Bound_F_Doc_Types, convert(varchar,I.D_Date,104) AS D_Date, I.C_Number, I.N_Number," +
        "I.FD_Documents_C_Period_Name, I._Bound_F_Status, convert(varchar,I.D_Post_Date,104) AS D_Post_Date, I.C_DT_Period, I.N_Quantity,I._Bound_F_Calc_Algorithms, I.N_AmountWithoutTax," +
        "I.N_Tax_Amount, I.N_Amount, convert(varchar,I.D_Date_Due,104) AS D_Date_Due, I.N_Amount_Open, I.N_Amount_Decrease - I.N_Tax_Amount_Decrease AS N_Cost_Decrease, I.N_Tax_Amount_Decrease," +
        $"I.N_Amount_Decrease, I.N_Amount_Increase - I.N_Tax_Amount_Increase AS N_Cost_Increase, I.N_Tax_Amount_Increase, I.N_Amount_Increase FROM EE.UI_FV_Invoices AS I WHERE I.N_Period = {period} AND I.F_Division = {division.LINK}";
            }
            
            string tns = $"server={HOST};Trusted_Connection={TRUSTED_CONNECTION};DataBase={DB};User ID={USER};password={PASSWORD};";
            SqlConnection connection = new SqlConnection(tns);
            connection.Open();
            SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = query;

            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {                
                object O_SD_Subscr_CodeNumber =  reader.GetValue(reader.GetOrdinal("SD_Subscr_CodeNumber"));
                decimal SD_Subscr_CodeNumber = (!O_SD_Subscr_CodeNumber.ToString().Equals("")) ? decimal.Parse(O_SD_Subscr_CodeNumber.ToString()) : 0;
                object O_C_FIO =  reader.GetValue(reader.GetOrdinal("C_FIO"));
                object O__Bound_F_Debts =  reader.GetValue(reader.GetOrdinal("_Bound_F_Debts"));           
                object O_C_Doc_linked =  reader.GetValue(reader.GetOrdinal("C_Doc_linked"));               
                object O__Bound_F_Doc_Types =  reader.GetValue(reader.GetOrdinal("_Bound_F_Doc_Types"));               
                object O_D_Date =  reader.GetValue(reader.GetOrdinal("D_Date"));               
                object O_C_Number =  reader.GetValue(reader.GetOrdinal("C_Number"));               
                object O_N_Number =  reader.GetValue(reader.GetOrdinal("N_Number"));
                int N_Number = (!O_N_Number.ToString().Equals("")) ? int.Parse(O_N_Number.ToString()) : 0;
                object O_FD_Documents_C_Period_Name = reader.GetValue(reader.GetOrdinal("FD_Documents_C_Period_Name"));
                object O__Bound_F_Status = reader.GetValue(reader.GetOrdinal("_Bound_F_Status"));
                object O_D_Post_Date = reader.GetValue(reader.GetOrdinal("D_Post_Date"));
                object O_C_DT_Period = reader.GetValue(reader.GetOrdinal("C_DT_Period"));
                object O_N_Quantity = reader.GetValue(reader.GetOrdinal("N_Quantity"));
                decimal N_Quantity = (!O_N_Quantity.ToString().Equals("")) ? decimal.Parse(O_N_Quantity.ToString()) : 0;
                object O__Bound_F_Calc_Algorithms = reader.GetValue(reader.GetOrdinal("_Bound_F_Calc_Algorithms"));
                object O_N_AmountWithoutTax = reader.GetValue(reader.GetOrdinal("N_AmountWithoutTax"));
                decimal N_AmountWithoutTax = (!O_N_AmountWithoutTax.ToString().Equals("")) ? decimal.Parse(O_N_AmountWithoutTax.ToString()) : 0;
                object O_N_Tax_Amount = reader.GetValue(reader.GetOrdinal("N_Tax_Amount"));
                decimal N_Tax_Amount = (!O_N_Tax_Amount.ToString().Equals("")) ? decimal.Parse(O_N_Tax_Amount.ToString()) : 0;
                object O_N_Amount = reader.GetValue(reader.GetOrdinal("N_Amount"));
                decimal N_Amount = (!O_N_Amount.ToString().Equals("")) ? decimal.Parse(O_N_Amount.ToString()) : 0;
                object O_D_Date_Due = reader.GetValue(reader.GetOrdinal("D_Date_Due"));
                object O_N_Amount_Open = reader.GetValue(reader.GetOrdinal("N_Amount_Open"));
                decimal N_Amount_Open = (!O_N_Amount_Open.ToString().Equals("")) ? decimal.Parse(O_N_Amount_Open.ToString()) : 0;
                object O_N_Cost_Decrease = reader.GetValue(reader.GetOrdinal("N_Cost_Decrease"));
                decimal N_Cost_Decrease = (!O_N_Cost_Decrease.ToString().Equals("")) ? decimal.Parse(O_N_Cost_Decrease.ToString()) : 0;
                object O_N_Tax_Amount_Decrease = reader.GetValue(reader.GetOrdinal("N_Tax_Amount_Decrease"));
                decimal N_Tax_Amount_Decrease = (!O_N_Tax_Amount_Decrease.ToString().Equals("")) ? decimal.Parse(O_N_Tax_Amount_Decrease.ToString()) : 0;
                object O_N_Amount_Decrease = reader.GetValue(reader.GetOrdinal("N_Amount_Decrease"));
                decimal N_Amount_Decrease = (!O_N_Amount_Decrease.ToString().Equals("")) ? decimal.Parse(O_N_Amount_Decrease.ToString()) : 0;
                object O_N_Cost_Increase = reader.GetValue(reader.GetOrdinal("N_Cost_Increase"));
                decimal N_Cost_Increase = (!O_N_Cost_Increase.ToString().Equals("")) ? decimal.Parse(O_N_Cost_Increase.ToString()) : 0;
                object O_N_Tax_Amount_Increase = reader.GetValue(reader.GetOrdinal("N_Tax_Amount_Increase"));
                decimal N_Tax_Amount_Increase = (!O_N_Tax_Amount_Increase.ToString().Equals("")) ? decimal.Parse(O_N_Tax_Amount_Increase.ToString()) : 0;
                object O_N_Amount_Increase = reader.GetValue(reader.GetOrdinal("N_Amount_Increase"));
                decimal N_Amount_Increase = (!O_N_Amount_Increase.ToString().Equals("")) ? decimal.Parse(O_N_Amount_Increase.ToString()) : 0;

                list.Add(new Accrual {
                    SD_Subscr_CodeNumber = SD_Subscr_CodeNumber,
                    C_FIO = O_C_FIO.ToString(),
                    _Bound_F_Debts = O__Bound_F_Debts.ToString(),
                    C_Doc_linked = O_C_Doc_linked.ToString(),
                    _Bound_F_Doc_Types = O__Bound_F_Doc_Types.ToString(),
                    D_Date = O_D_Date.ToString(),
                    C_Number = O_C_Number.ToString(),
                    N_Number = N_Number,
                    FD_Documents_C_Period_Name = O_FD_Documents_C_Period_Name.ToString(),
                    _Bound_F_Status = O__Bound_F_Status.ToString(),
                    D_Post_Date = O_D_Post_Date.ToString(),
                    C_DT_Period = O_C_DT_Period.ToString(),
                    N_Quantity = Math.Round(N_Quantity, 2),
                    _Bound_F_Calc_Algorithms = O__Bound_F_Calc_Algorithms.ToString(),
                    N_AmountWithoutTax = Math.Round(N_AmountWithoutTax, 2),
                    N_Tax_Amount = Math.Round(N_Tax_Amount, 2),
                    N_Amount = Math.Round(N_Amount, 2),
                    D_Date_Due = O_D_Date_Due.ToString(),
                    N_Amount_Open = Math.Round(N_Amount_Open, 2),
                    N_Cost_Decrease = Math.Round(N_Cost_Decrease, 2),
                    N_Tax_Amount_Decrease = Math.Round(N_Tax_Amount_Decrease, 2),
                    N_Amount_Decrease = Math.Round(N_Amount_Decrease, 2),
                    N_Cost_Increase = Math.Round(N_Cost_Increase, 2),
                    N_Tax_Amount_Increase = Math.Round(N_Tax_Amount_Increase, 2),
                    N_Amount_Increase = Math.Round(N_Amount_Increase, 2)
                });
            }
            connection.Close();
            return list;
        }
    }
}
