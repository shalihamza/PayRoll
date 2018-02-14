//by Ali Hamza

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Payroll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        public bool var_MaintainArea = new bool();
        public bool var_MaintainGroup = new bool();
        public bool var_MaintainZone = new bool();
        public bool var_MaintainDepartment = new bool();
        public bool var_MaintainDesignation = new bool();
        public bool var_MaintainBank = new bool();
        public bool var_MaintainPaymentType = new bool();
        public bool var_MaintainSalaryChange = new bool();
        public bool var_MaintainLeaveType = new bool();
        public bool var_MaintainEOBI = new bool();
        public bool var_MaintainProvidentFund = new bool();
        public bool var_MaintainSocialSecurity = new bool();
        public bool var_Maintain = new bool();
        public bool var_EmpAddEdit = new bool();
        public bool var_Deductions = new bool();
        public bool var_Earnings = new bool();
        public int tab_count = 0;
        public string tbl_name, fields;
        public int i = 0, card_no = 0, loan_id = 0, id_bank = 0;
        public string cafrd_no, zoneName = null;
        String theDate;
        public OleDbCommand cmd = new OleDbCommand();

        #endregion
        OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString());
        public MainWindow()
        {
            con.Open();
            InitializeComponent();
            //tabControl.Visibility = Visibility.Collapsed;
            DateTime currentDate = DateTime.Now;
            theDate = currentDate.ToShortDateString();
            MessageBox.Show("ali hamza change 1");
            MessageBox.Show("ali hamza change 2");
            MessageBox.Show("ali hamza change 3");
            MessageBox.Show("Changing Saves");
            cmd.Connection = con;
            txt_empl_no.IsEnabled = true;
            AddHotKeys();
        }
        private void AddHotKeys()
        {
            try
            {
                RoutedCommand firstSettings = new RoutedCommand();
                firstSettings.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(firstSettings, My_first_event_handler));
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void writeTextToLogFile(string qry)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(@"logs\logs.txt", true))
                outputFile.WriteLine(qry);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

        }

        private void My_first_event_handler(object sender, ExecutedRoutedEventArgs e)
        {
            window_emps windEmps = new window_emps();
            windEmps.Owner = this;
            windEmps.Show();
        }

        private void callGrid(string areaName, DataGrid gridName, string query)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = query;
                OleDbDataReader rd = cmd.ExecuteReader();
                //var abc = gridName.ToString();
                int counter = 0;
                callZone();
                //callgroup();
                gridName.Items.Clear();
                foreach (var item in rd)
                {
                    gridName.Items.Add(item);
                    counter++;
                }
                rd.Close();
                if (areaName == "areas")
                {
                    lbl_records.Content = (counter > 0) ? "Showing " + counter + " record(s)" : lbl_records.Content = "No record found.";
                }
                if (areaName == "loans")
                {
                    lbl_record.Content = (counter > 0) ? "Showing " + counter + " record(s)" : lbl_record.Content = "No record found.";
                }
                if (areaName == "banks")
                {
                    lbl_bank.Content = (counter > 0) ? "Showing " + counter + " record(s)" : lbl_bank.Content = "No record found.";
                }
                rd.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region Add and Update Area
        string count;
        Int32 new_count;
        private void add_Area_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            txtblock.Text = "";
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");

            if (con.State != ConnectionState.Open)
                con.Open();
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(txt_area_Name.Text) || string.IsNullOrWhiteSpace(comb_zone.Text))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Zone or Area-Name is invalid.\n\nSelect a Zone first, then enter a valid Area-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "Select * from [areas] where area_name='" + txt_area_Name.Text + "' and zone_code=" + comb_zone.SelectedValue + "";
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int chk_rows = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        if (chk_rows == 0)
                        {
                            if (string.IsNullOrWhiteSpace(txt_area_Name.Text) || string.IsNullOrWhiteSpace(comb_zone.Text))
                            {
                                MessageBox.Show("Operation Aborted!\n\nReason: Zone or Area-Name is invalid.\n\nSelect a Zone first, then enter a valid Area-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBoxResult closing_called = MessageBox.Show("Following AREA will be added in the system: -\n\nArea Name = " + txt_area_Name.Text + ",\nZone ID = " + comb_zone.SelectedValue + ",\n\n Are you sure ? ", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (closing_called == MessageBoxResult.Yes)
                                {
                                    int zone_code = int.Parse(comb_zone.SelectedValue.ToString());
                                    string qry_count = "select max(area_code) from [areas] where zone_code<=6";
                                    cmd.CommandText = qry_count;
                                    count = cmd.ExecuteScalar().ToString();
                                    new_count = int.Parse(count) + 1;
                                    string qry_insert = "insert into [areas] (area_code, zone_code, area_name, rec_date, status) values(" + new_count + ", " + comb_zone.SelectedValue + ", '" + txt_area_Name.Text + "', #" + theDate + "#, 1)";
                                    cmd.CommandText = qry_insert;
                                    cmd.ExecuteNonQuery();
                                    tbl_name = "areas";
                                    callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                                    txtblock.Inlines.Add(new Run("A new area named ") { Foreground = Brushes.Black, FontSize = 14 });
                                    txtblock.Inlines.Add(new Run(txt_area_Name.Text) { Foreground = Brushes.Green, FontSize = 14 });
                                    txtblock.Inlines.Add(new Run(" is added successfully") { Foreground = Brushes.Black, FontSize = 14 });
                                    sb.Begin(txtblock);
                                    sb.Begin(border);
                                    clearAllValues();
                                }
                                else if (closing_called == MessageBoxResult.No)
                                {
                                    //e = null;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Operation Aborted!\n\nReason: AreaName already exists! \n\nIf it is not a mistake then please change spellings little bit.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                            search_zone.Clear();
                            search_area.Clear();
                            radio_open.IsChecked = false;
                            radio_closed.IsChecked = false;
                            comb_zone.Focus();
                        }
                    }
                }
                if (update == true)
                {
                    string combofirstValue = null, combolastvalue = null;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;
                    if (string.IsNullOrWhiteSpace(comb_zone.SelectedValue.ToString()) || string.IsNullOrWhiteSpace(txt_area_Name.Text))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Zone or Area-Name is invalid.\n\nSelect a Zone first, then enter a valid Area-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        cmd.CommandText = "Select * from [areas] where area_name='" + txt_area_Name.Text + "' and zone_code=" + comb_zone.SelectedValue + "";
                        //combofirstValue = comb_zone.DisplayMemberPath.ToString();
                        //MessageBox.Show(combofirstValue);
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int chk_rows = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        string select_Zone = "select * from zones where zone_code=" + comb_zone.SelectedValue + "";
                        cmd.CommandText = select_Zone;
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            combolastvalue = reader["zone_name"].ToString();
                        }
                        reader.Close();
                        if (chk_rows == 0)
                        {
                            if (string.IsNullOrWhiteSpace(comb_zone.SelectedValue.ToString()) || string.IsNullOrWhiteSpace(txt_area_Name.Text))
                            {
                                MessageBox.Show("Operation Aborted!\n\nReason: Zone or Area-Name is invalid.\n\nSelect a Zone first, then enter a valid Area-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                string update_qry = "UPDATE [areas] set zone_code=" + comb_zone.SelectedValue.ToString() + " , area_name='" + txt_area_Name.Text + "' where area_code = " + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                tbl_name = "areas";
                                callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                                if (zoneName == combolastvalue)
                                {
                                    txtblock.Inlines.Add(new Run("Area '" + txt_area_Name.Text + "' is updated.") { Foreground = Brushes.Green, FontSize = 14 });
                                    sb.Begin(txtblock);
                                    sb.Begin(border);
                                }
                                else
                                {
                                    txtblock.Inlines.Add(new Run("Area '" + txt_area_Name.Text + "' has been moved from '" + zoneName + "' to '" + combolastvalue + "' Successfully.") { Foreground = Brushes.Green, FontSize = 14 });
                                    sb.Begin(txtblock);
                                    sb.Begin(border);
                                }
                                txt_area_Name.Clear();
                                comb_zone.SelectedIndex = -1;
                                area_Add.Content = "Add Record";

                            }
                        }
                        else
                        {
                            txtblock.Inlines.Add(new Run("AreaName '" + txt_area_Name.Text + "' already exist!") { Foreground = Brushes.Red, FontSize = 14 });
                            sb.Begin(txtblock);
                            sb.Begin(border);
                            MessageBox.Show("Operation Aborted!\n\nReason: AreaName already exists! \n\nIf it is not a mistake then please change spellings little bit.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                            tbl_name = "areas";
                            callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                            clearAllValues();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                txtblock.Inlines.Add(new Run("Something went wrong.Connect the Admin") { Foreground = Brushes.Red, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
                MessageBox.Show("Operation Aborted!\n\nReason: Unknow! \n\nSomething went wrong.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                //throw;
            }
        }
        #endregion
        //private void callgroup()
        //{
        //    try
        //    {
        //        if (con.State != ConnectionState.Open)
        //            con.Open();
        //        OleDbCommand cmd = new OleDbCommand();
        //        cmd.Connection = con;
        //        cmd.CommandText = "SELECT * FROM [groups] WHERE 1=1";
        //        OleDbDataReader rd = cmd.ExecuteReader();
        //        combo_zonetype.Items.Clear();
        //        combo_zonetype.ItemsSource = rd;
        //    }
        //    catch (Exception)
        //    {

        //        //throw;
        //    }
        //}
        private void callZone()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM [zones] WHERE 1=1";
                OleDbDataReader rd = cmd.ExecuteReader();
                comb_zone.Items.Clear();
                comb_zone.ItemsSource = rd;
            }
            catch (Exception)
            {
                //MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease contact with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void comboselection_change(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(comb_zone.SelectedValue.ToString());
        }
        private void management_action(object sender, System.EventArgs e)
        {

            int x;
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(search_area.Text, UnallowedCharacters))
                {

                    int CursorIndex = search_area.SelectionStart - 1;
                    search_area.Text = search_area.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    search_area.SelectionStart = CursorIndex;
                    search_area.SelectionLength = 0;
                    string qry;
                    qry = "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1 ";
                    //callGrid(tbl_name, grid_areas, qry);
                    if (search_zone.Text != "") { qry = qry + " AND zone_code like '" + search_zone.Text + "'"; }
                    if (search_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_area.Text + "')"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status=1"; }
                    if (radio_closed.IsChecked == true) { qry = qry + " AND status=0"; }
                    //grid_areas.Items.Clear();
                    callGrid("areas", grid_areas, qry);
                }
                if (search_zone.Text != " ")
                {
                    x = int.Parse(search_zone.Text);
                    string qry;
                    qry = "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1 ";
                    //callGrid(tbl_name, grid_areas, qry);
                    if (search_zone.Text != "") { qry = qry + " AND zone_code like '" + search_zone.Text + "'"; }
                    if (search_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_area.Text + "')"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status=1"; }
                    if (radio_closed.IsChecked == true) { qry = qry + " AND status=0"; }
                    //grid_areas.Items.Clear();
                    callGrid("areas", grid_areas, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = search_zone.SelectionStart - 1;
                    search_zone.Text = search_zone.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    search_zone.SelectionStart = cursorIndex;
                    search_zone.SelectionLength = 0;
                }
                catch (Exception)
                {
                    string qry;
                    qry = "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1 ";
                    //callGrid(tbl_name, grid_areas, qry);
                    if (search_zone.Text != "") { qry = qry + " AND zone_code like '" + search_zone.Text + "'"; }
                    if (search_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_area.Text + "')"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status=1"; }
                    if (radio_closed.IsChecked == true) { qry = qry + " AND status=0"; }
                    //grid_areas.Items.Clear();
                    callGrid("areas", grid_areas, qry);
                    //MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //throw;
                }
            }

        }

        private bool textContainsUnallowedCharacter(string T, char[] UnallowedCharacters)
        {
            for (int i = 0; i < UnallowedCharacters.Length; i++)
                if (T.Contains(UnallowedCharacters[i]))
                    return true;

            return false;
        }
        #region Buttons
        private void menu_btn_about_Click(object sender, RoutedEventArgs e)
        {
            //mdi_container.Children.Add(new WPF.MDI.MdiChild()
            //{
            //    Content = new aboutPage()

            //});
        }

        private void menu_btn_exit_Click(object sender, RoutedEventArgs e)
        {
            btn_exit2_Click(null, null);
        }
        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            //if (tab_count == 0)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            //else
            //{
            //    MessageBoxResult closing_called = MessageBox.Show("اسلامُ علیکم\nIt seems that some tab(s) are still open.\nUnsaved work will be lost.\n\nDo you want to cloase anyway?", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Error);
            //    if (closing_called == MessageBoxResult.Yes) { Environment.Exit(Environment.ExitCode); }
            //    else { e.Cancel = true; }
            //}
        }
        private void btn_exit2_Click(object sender, RoutedEventArgs e)
        {
            //if (tab_count == 0)
            //{
            //    Environment.Exit(Environment.ExitCode);
            //}
            //else
            //{
            //    MessageBoxResult closing_called = MessageBox.Show("اسلامُ علیکم\nIt seems that some tab(s) are still open.\nUnsaved work will be lost.\n\nDo you want to cloase anyway?", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Error);
            //    if (closing_called == MessageBoxResult.Yes) { Environment.Exit(Environment.ExitCode); }
            //    else { e = null; }
            //}
        }
        #endregion

        void CallTab(TabControl tab, TabItem item, bool a, TabItem tabItem)
        {
            if (item.Visibility == Visibility.Visible)
            {
                tab.SelectedItem = item;
                tab_inMaintain.SelectedItem = tabItem;
            }
            else if (a == true)
            {
                tab.Items.Add(item);
                a = false;
                item.Visibility = Visibility.Visible;
                tab.Visibility = Visibility.Visible;
                tab.SelectedItem = item;
                tab_inMaintain.SelectedItem = tabItem;
                tab_count++;
            }
            else
            {
                tab.Visibility = Visibility.Visible;
                item.Visibility = Visibility.Visible;
                tab.SelectedItem = item;
                tab_inMaintain.SelectedItem = tabItem;
                tab_count++;
            }
        }

        #region Area Show and Hide
        private void Management_Click(object sender, RoutedEventArgs e)
        {
            clearAllValues();
            txtblock.Text = "";
            txtblock.Inlines.Add(new Run("MESSAGE BOX") { Foreground = Brushes.Brown, FontSize = 14 });
            bool a, g, z, d, des, b, pt, ltp, eobi, r;
            a = sender.ToString().Contains("Area");
            g = sender.ToString().Contains("Group");
            z = sender.ToString().Contains("Zone");
            d = sender.ToString().Contains("Department");
            des = sender.ToString().Contains("Designation");
            b = sender.ToString().Contains("Bank");
            pt = sender.ToString().Contains("Payment Types");
            ltp = sender.ToString().Contains("Leaves Type");
            eobi = sender.ToString().Contains("EOBI, Pf & Ss");
            r = sender.ToString().Contains("Religion");
            if (r == true)
            {
                callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                //callGrid("religions", grid_Religion, "select * from religions where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainReligions);
            }
            if (a == true)
            {
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainArea);
                tbl_name = "areas";
                callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                //callGrid(tbl_name, grid_areas, "SELECT * FROM [" + tbl_name + "] WHERE 1=1");
                //callZone();
            }
            if (g == true)
            {
                callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainGroup);
                clearAllValues();
            }
            if (z == true)
            {
                //callGrid("zones", grid_Zones, "select * from zones where 1=1");
                callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                //if (con.State != ConnectionState.Open)
                //    con.Open();
                ////string tableName = "zones";
                //OleDbCommand cmd = new OleDbCommand();
                //cmd.Connection = con;
                //cmd.CommandText = "SELECT * FROM [groups] WHERE 1=1";
                //OleDbDataReader rd = cmd.ExecuteReader();
                //combo_zonetype.Items.Clear();
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainZone);
            }
            if (d == true)
            {
                //callGrid("departs", grid_department, "select * from departs");
                callGrid("departs", grid_department, "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainDepartment);
            }
            if (des == true)
            {
                //callGrid("desigs", grid_designation, "select * from desigs");
                callGrid("desigs", grid_designation, "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainDesignation);
            }
            if (b == true)
            {
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainBank);
                tbl_name = "banks";
                //callGrid("banks", grid_Bank, "SELECT * FROM banks WHERE 1=1");
                callGrid("banks", grid_Bank, "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1");
            }
            if (pt == true)
            {
                //callGrid("payment_types", grid_payType, "select * from payment_types where 1=1");
                callGrid("payment_types", grid_payType, "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_maintainPaymentType);
            }
            if (ltp == true)
            {
                //callGrid("leave_types", grid_leaveType, "select * from leave_types where 1=1");
                callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainLeaveType);
            }
            if (eobi == true)
            {
                CallTab(tabControl, tab_Maintain, var_Maintain, tab_MaintainEOBI);
                try
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    cmd.CommandText = "SELECT * FROM [EOBI_PF_SS]";
                    //OleDbCommand command = new OleDbCommand("SELECT * FROM [EOBI_PF_SS]");
                    //command.Connection = con;
                    OleDbDataReader dr = null;
                    dr = cmd.ExecuteReader();
                    txt_EOBI.Clear();
                    txt_EPF.Clear();
                    txt_ESS.Clear();
                    while (dr.Read())
                    {
                        txt_EOBI.Text = (dr["EOBI"].ToString());
                        txt_EPF.Text = (dr["EPF"].ToString());
                        txt_ESS.Text = (dr["ESS"].ToString());
                    }
                    con.Close();
                    dr.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btn_MaintainArea_Click(object sender, RoutedEventArgs e)
        {
            tab_count--;
            if (tab_count == 0)
            {
                tabControl.Items.Remove(tab_MaintainArea);
                var_MaintainArea = true;
                tab_MaintainArea.Visibility = Visibility.Hidden;
                tabControl.Visibility = Visibility.Hidden;
            }
            else
            {
                tabControl.Items.Remove(tab_MaintainArea);
                tab_MaintainArea.Visibility = Visibility.Hidden;
                var_MaintainArea = true;
            }
        }

        private void grid_loan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //var selectedLoan = grid_loan.SelectedItem as LoansAndAdvances;
                //if (con.State != ConnectionState.Open)
                //    con.Open();
                //OleDbCommand cmd = new OleDbCommand();
                //cmd.Connection = con;
                //string id = selectedLoan.card_no.ToString();
                //card_no = int.Parse(id);
                //loan_id = int.Parse(selectedLoan.loan_id);
                //string selectqry = "select * from [loans] where card_no='" + id + "'";
                //cmd.CommandText = selectqry;
                //OleDbDataReader rd = cmd.ExecuteReader();
                //while (rd.Read())
                //{
                //    txt_empl_no.Text = rd["card_no"].ToString();
                //    txt_loan_amnt.Text = rd["loan_amount"].ToString();
                //    txt_installments.Text = rd["no_of_instal"].ToString();
                //    txt_inst_amnt.Text = rd["instal_amount"].ToString();
                //}
                //btn_loan_add.Content = "UpDate";
                //txt_empl_no.IsEnabled = false;
                object item = grid_areas.SelectedItem;
                string ID = (grid_areas.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                string loanID = (grid_areas.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);
                card_no = id;
                loan_id = int.Parse(loanID);
                MessageBox.Show("Catched");
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                throw;
            }
        }
        private void grid_areas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            //MessageBox.Show(ID);
            try
            {
                if (sender != null)
                {
                    object item = grid_areas.SelectedItem;
                    string ID = (grid_areas.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    string zonecode = (grid_areas.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                    int zcode = int.Parse(zonecode);
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    string area_name = null;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = con;
                    int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                    i = id;
                    //MessageBox.Show(i.ToString());
                    //qry = "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1 ";
                    string selectqry = "select * from areas where area_code=" + id + "";
                    cmd.CommandText = selectqry;
                    OleDbDataReader rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        comb_zone.SelectedValue = rd["zone_code"].ToString();
                        txt_area_Name.Text = rd["area_name"].ToString();
                        area_name = rd["area_name"].ToString();
                    }
                    rd.Close();
                    string select_Zone = "select * from zones where zone_code=" + zcode + "";
                    cmd.CommandText = select_Zone;
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        zoneName = reader["zone_name"].ToString();
                    }
                    reader.Close();
                    //MessageBox.Show(zoneName);
                    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                    txtblock.Text = "";
                    txtblock.Inlines.Add(new Run("Area " + area_name + " is selected for editing") { Foreground = Brushes.Green, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                    area_Add.Content = "UpDate";
                    //txtblock.Text = selectedarea.area_name;
                }
                else
                {
                    MessageBox.Show("Not Selected");
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                //throw;
            }
        }
        private void btn_Delete_Row_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            txtblock.Text = "";
            try
            {
                bool zero = false, one = false;
                object item = grid_areas.SelectedItem;
                string ID = (grid_areas.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                string area_name = (grid_areas.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //var selectedarea = grid_areas.SelectedItem as AreaManagement;
                //int id = int.Parse(selectedarea.area_code.ToString());
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand oleDb = new OleDbCommand();
                oleDb.Connection = con;
                string select_qry = "select * from areas where area_code=" + id + "";
                oleDb.CommandText = select_qry;
                OleDbDataReader rd = oleDb.ExecuteReader();
                //MessageBox.Show("abc");
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    string qru_Delete = "UPDATE [areas] set status=1 where area_code=" + id;
                    oleDb.CommandText = qru_Delete;
                    oleDb.ExecuteNonQuery();
                    tbl_name = "areas";
                    //callGrid(tbl_name, grid_areas, "SELECT * FROM [" + tbl_name + "] WHERE status=1");
                    callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                    callZone();
                    txtblock.Inlines.Add(new Run("Area " + area_name + " status is updated seccessfully.") { Foreground = Brushes.Green, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                }
                if (one == true)
                {
                    string qru_Delete = "UPDATE [areas] set status=0 where area_code=" + id;
                    oleDb.CommandText = qru_Delete;
                    oleDb.ExecuteNonQuery();
                    tbl_name = "areas";
                    //callGrid(tbl_name, grid_areas, "SELECT * FROM [" + tbl_name + "] WHERE status=1");
                    callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where 1=1");
                    callZone();
                    txtblock.Inlines.Add(new Run("Area " + area_name + " status is updated seccessfully.") { Foreground = Brushes.Green, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                }
                else
                {
                    tbl_name = "areas";
                    //callGrid(tbl_name, grid_areas, "SELECT * FROM [" + tbl_name + "] WHERE status=1");
                    callGrid("areas", grid_areas, "select * ,iif([status]=0,1,0) as ali_column from areas where status=1");
                    callZone();
                }
                clearAllValues();

            }
            catch (Exception)
            {
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
                //throw;
            }
        }
        #endregion
        #region Group Show and Hide
        private void group_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(gname.Text, UnallowedCharacters))
                {

                    int CursorIndex = gname.SelectionStart - 1;
                    gname.Text = gname.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    gname.SelectionStart = CursorIndex;
                    gname.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (gid.Text != "") ? qry + " and group_code like'" + gid.Text + "'" : qry;
                    qry = (gname.Text != "") ? qry + "and Instr(group_name,'" + gname.Text + "')" : qry;
                    qry = (g_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (g_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("groups", grid_Group, qry);
                }
                if (gid.Text != " ")
                {
                    x = int.Parse(gid.Text);
                    qry = (gid.Text != "") ? qry + " and group_code like'" + gid.Text + "'" : qry;
                    qry = (gname.Text != "") ? qry + "and Instr(group_name,'" + gname.Text + "')" : qry;
                    qry = (g_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (g_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("groups", grid_Group, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = gid.SelectionStart - 1;
                    gid.Text = gid.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    gid.SelectionStart = cursorIndex;
                    gid.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (gid.Text != "") ? qry + " and group_code like'" + gid.Text + "'" : qry;
                    qry = (gname.Text != "") ? qry + "and Instr(group_name,'" + gname.Text + "')" : qry;
                    qry = (g_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (g_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("groups", grid_Group, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_group_status_changed(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_Group.SelectedItem;
                string ID = (grid_Group.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from groups where group_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update groups set status=1 where group_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
                    group_Action(null, null);
                }

                if (one == true)
                {
                    cmd.CommandText = "update groups set status=0 where group_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
                    group_Action(null, null);
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_group_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_Group.SelectedItem;
                string ID = (grid_Group.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from groups where group_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    gabri.Text = rd["group_abr"].ToString();
                    txt_group_name.Text = rd["group_name"].ToString();

                }
                rd.Close();
                group_add.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        private void add_group_click(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            //try
            //{
            if (add == true)
            {
                if (string.IsNullOrWhiteSpace(gabri.Text) || string.IsNullOrWhiteSpace(txt_group_name.Text))
                {
                    MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!Regex.IsMatch(gabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_group_name.Text, @"[a-zA-Z]+$"))
                    {
                        MessageBox.Show("Only characters are Supported");
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        //string find_qry = "select * from groups where group_abr='"+gabri.Text+"' and instr(group=" + txt_group_name.Text + ")";
                        string find_qry = "select * from groups WHERE (((groups.[group_abr])='" + gabri.Text + "') or ((groups.group_name)='" + txt_group_name.Text + "'))";
                        File.AppendAllText(@"file.txt", find_qry + Environment.NewLine);
                        cmd.CommandText = find_qry;
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int check = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        if (check == 0)
                        {
                            string max_qry = "select max(group_code) from groups";
                            cmd.CommandText = max_qry;
                            int count = int.Parse(cmd.ExecuteScalar().ToString());
                            int new_count = count + 1;
                            string insert_qry = "insert into [groups] (group_code, group_abr, group_name, rec_date, status) values (" + new_count + ",'" + gabri.Text + "','" + txt_group_name.Text + "', #" + theDate + "#, 1)";
                            writeTextToLogFile(insert_qry);
                            cmd.CommandText = insert_qry;
                            cmd.ExecuteNonQuery();
                            callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
                        }
                        else
                        {
                            MessageBox.Show("Group-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                clearAllValues();
                callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
            }
            if (update == true)
            {
                if (string.IsNullOrWhiteSpace(gabri.Text) || string.IsNullOrWhiteSpace(txt_group_name.Text))
                {
                    MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!Regex.IsMatch(gabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_group_name.Text, @"[a-zA-Z]+$"))
                    {
                        MessageBox.Show("Only characters are Supported");
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        string find_qry = "select * from groups WHERE (((groups.[group_abr])='" + gabri.Text + "') or ((groups.group_name)='" + txt_group_name.Text + "'))";
                        //string find_qry = "select * from groups where group_abr='" + gabri.Text + "' and group='" + txt_group_name.Text + "'";
                        cmd.CommandText = find_qry;
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int check = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        if (check == 0)
                        {
                            string update_qry = "update groups set group_abr='" + gabri.Text + "',group_name='" + txt_group_name.Text + "' where group_code=" + i + "";
                            cmd.CommandText = update_qry;
                            cmd.ExecuteNonQuery();
                            callGrid("groups", grid_Group, "select * ,iif([status]=0,1,0) as ali_column from groups where 1=1");
                        }
                        else
                        {
                            MessageBox.Show("Group-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                clearAllValues();
            }
            //}
            //catch (Exception)
            //{
            //    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            //    txtblock.Text = "";
            //    txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
            //    sb.Begin(txtblock);
            //    sb.Begin(border);
            //}
            //end
        }

        #endregion
        #region Zone Show and Hide
        private void zone_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(zone_Name.Text, UnallowedCharacters))
                {

                    int CursorIndex = zone_Name.SelectionStart - 1;
                    zone_Name.Text = zone_Name.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    zone_Name.SelectionStart = CursorIndex;
                    zone_Name.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (zoneid.Text != "") ? qry + " and zone_code like'" + zoneid.Text + "'" : qry;
                    qry = (zone_Name.Text != "") ? qry + "and Instr(zone_name,'" + zone_Name.Text + "')" : qry;
                    qry = (zone_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (zone_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("zones", grid_Zones, qry);
                }
                if (relID.Text != " ")
                {
                    x = int.Parse(relID.Text);
                    qry = (zoneid.Text != "") ? qry + " and zone_code like'" + zoneid.Text + "'" : qry;
                    qry = (zone_Name.Text != "") ? qry + "and Instr(zone_name,'" + zone_Name.Text + "')" : qry;
                    qry = (zone_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (zone_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("zones", grid_Zones, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = relID.SelectionStart - 1;
                    relID.Text = relID.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    relID.SelectionStart = cursorIndex;
                    relID.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (zoneid.Text != "") ? qry + " and zone_code like'" + zoneid.Text + "'" : qry;
                    qry = (zone_Name.Text != "") ? qry + "and Instr(zone_name,'" + zone_Name.Text + "')" : qry;
                    qry = (zone_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (zone_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("zones", grid_Zones, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_zone_status_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_Zones.SelectedItem;
                string ID = (grid_Zones.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from zones where zone_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update zones set status=1 where zone_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update zones set status=0 where zone_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                }
            }
            catch (Exception)
            {

                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void add_zone_Click(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(combo_zonetype.Text) || string.IsNullOrWhiteSpace(txt_zone_name.Text))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Zone-Type or Zone-Name is invalid.\n\nSelect a Zone-Type first, then enter a valid Zone-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        if (combo_zonetype.SelectedIndex == 1)
                        {
                            string find_qry = "select * from zones where zone_name='" + txt_zone_name.Text + "' and type='F'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int chk_row = Convert.ToInt32(rd.HasRows);
                            MessageBox.Show(chk_row.ToString());
                            rd.Close();
                            if (chk_row == 0)
                            {

                                string max_qry = "select max(zone_code) from zones where type='F'";
                                cmd.CommandText = max_qry;
                                int count = Convert.ToInt32(cmd.ExecuteScalar());
                                int new_count = count + 1;
                                if (!Regex.IsMatch(txt_zone_name.Text, @"[a-zA-Z-]+$"))
                                {
                                    MessageBox.Show("Zone-Name is invalid", "Warning");
                                }
                                else
                                {
                                    string insert_qry = "insert into zones (zone_code,zone_name,type,rec_date,status)values(" + new_count + ",'" + txt_zone_name.Text + "','F',#" + theDate + "#,1)";
                                    cmd.CommandText = insert_qry;
                                    cmd.ExecuteNonQuery();
                                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Zone-Name is already exist");
                            }
                        }
                        else
                        {
                            string find_qry = "select * from zones where zone_name='" + txt_zone_name.Text + "' and type='C'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int chk_row = Convert.ToInt32(rd.HasRows);
                            MessageBox.Show(chk_row.ToString());
                            rd.Close();
                            if (chk_row == 0)
                            {
                                string max_qry = "select max(zone_code) from zones where type='C'";
                                cmd.CommandText = max_qry;
                                int count = Convert.ToInt32(cmd.ExecuteScalar());
                                int new_count = count + 1;
                                if (!Regex.IsMatch(txt_zone_name.Text, @"[a-zA-Z-]+$"))
                                {
                                    MessageBox.Show("Zone-Name is invalid", "Warning");
                                }
                                else
                                {
                                    string insert_qry = "insert into zones (zone_code,zone_name,type,rec_date,status)values(" + new_count + ",'" + txt_zone_name.Text + "','C',#" + theDate + "#,1)";
                                    cmd.CommandText = insert_qry;
                                    cmd.ExecuteNonQuery();
                                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                                }
                            }
                        }
                    }
                    clearAllValues();
                }
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(combo_zonetype.Text) || string.IsNullOrWhiteSpace(txt_zone_name.Text))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Zone-Type or Zone-Name is invalid.\n\nSelect a Zone-Type first, then enter a valid Zone-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        if (combo_zonetype.SelectedIndex == 1)
                        {
                            string find_qry = "select * from zones where zone_name='" + txt_zone_name.Text + "' and type='F'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int chk_row = Convert.ToInt32(rd.HasRows);
                            MessageBox.Show(chk_row.ToString());
                            rd.Close();
                            if (chk_row == 0)
                            {
                                if (!Regex.IsMatch(txt_zone_name.Text, @"[a-zA-Z-]+$"))
                                {
                                    MessageBox.Show("Zone-Name is invalid", "Warning");
                                }
                                else
                                {
                                    string update_qry = "update zones set zone_name='" + txt_zone_name.Text + "',type='F' where zone_code=" + i + "";
                                    //string insert_qry = "insert into zones (zone_code,zone_name,type,rec_date,status)values(" + new_count + ",'" + txt_zone_name.Text + "','F',#" + theDate + "#,1)";
                                    cmd.CommandText = update_qry;
                                    cmd.ExecuteNonQuery();
                                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Zone-Name is already exist");
                            }
                        }
                        else
                        {
                            string find_qry = "select * from zones where zone_name='" + txt_zone_name.Text + "' and type='C'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int chk_row = Convert.ToInt32(rd.HasRows);
                            MessageBox.Show(chk_row.ToString());
                            rd.Close();
                            if (chk_row == 0)
                            {
                                if (!Regex.IsMatch(txt_zone_name.Text, @"[a-zA-Z-]+$"))
                                {
                                    MessageBox.Show("Zone-Name is invalid", "Warning");
                                }
                                else
                                {
                                    string update_qry = "update zones set zone_name='" + txt_zone_name.Text + "',type='C' where zone_code=" + i + "";
                                    cmd.CommandText = update_qry;
                                    cmd.ExecuteNonQuery();
                                    callGrid("zones", grid_Zones, "select * ,iif([status]=0,1,0) as ali_column from zones where 1=1");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Zone-Name is already exist");
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {

                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
            //end
        }
        private void grid_Zones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_Zones.SelectedItem;
                string ID = (grid_Zones.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                string zonetype = (grid_Zones.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from zones where zone_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (zonetype == "F")
                    {
                        combo_zonetype.SelectedIndex = 1;
                        txt_zone_name.Text = rd["zone_name"].ToString();
                    }
                    else
                    {
                        combo_zonetype.SelectedIndex = 0;
                        txt_zone_name.Text = rd["zone_name"].ToString();
                    }
                }
                rd.Close();
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                //txtblock.Inlines.Add(new Run("Area " + area_name + " is selected for editing") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
                add_zone.Content = "UpDate";
            }
            catch (Exception)
            {

                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        #endregion
        #region Department Show and Hide
        private void department_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(departmentname.Text, UnallowedCharacters))
                {

                    int CursorIndex = departmentname.SelectionStart - 1;
                    departmentname.Text = departmentname.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    departmentname.SelectionStart = CursorIndex;
                    departmentname.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (deptid.Text != "") ? qry + " and dept_code like'" + deptid.Text + "'" : qry;
                    qry = (departmentname.Text != "") ? qry + "and Instr(department,'" + departmentname.Text + "')" : qry;
                    qry = (dept_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (dept_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("departs", grid_department, qry);
                }
                if (deptid.Text != " ")
                {
                    x = int.Parse(deptid.Text);
                    qry = (deptid.Text != "") ? qry + " and dept_code like'" + deptid.Text + "'" : qry;
                    qry = (departmentname.Text != "") ? qry + "and Instr(department,'" + departmentname.Text + "')" : qry;
                    qry = (dept_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (dept_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("departs", grid_department, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = deptid.SelectionStart - 1;
                    deptid.Text = deptid.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    deptid.SelectionStart = cursorIndex;
                    deptid.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (deptid.Text != "") ? qry + " and dept_code like'" + deptid.Text + "'" : qry;
                    qry = (departmentname.Text != "") ? qry + "and Instr(department,'" + departmentname.Text + "')" : qry;
                    qry = (dept_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (dept_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("departs", grid_department, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_Department_status_change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_department.SelectedItem;
                string ID = (grid_department.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from departs where dept_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update departs set status=1 where dept_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("departs", grid_department, "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update departs set status=0 where dept_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("departs", grid_department, "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1");
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_department_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_department.SelectedItem;
                string ID = (grid_department.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from departs where dept_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    deptabri.Text = rd["dept_abr"].ToString();
                    txt_dept_name.Text = rd["department"].ToString();
                    depttype.Text = rd["type"].ToString();
                }
                rd.Close();
                dept_add.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void add_department_click(object sender, RoutedEventArgs e)
        {
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            if (con.State != ConnectionState.Open)
                con.Open();
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(deptabri.Text) || string.IsNullOrWhiteSpace(txt_dept_name.Text) || string.IsNullOrWhiteSpace(depttype.Text))
                    {
                        MessageBox.Show("Somethigs are missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (!Regex.IsMatch(deptabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_dept_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from departs where dept_abr='" + deptabri.Text + "' and department='" + txt_dept_name.Text + "' and type='" + depttype.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string max_qry = "select max(dept_code) from departs";
                                cmd.CommandText = max_qry;
                                int count = int.Parse(cmd.ExecuteScalar().ToString());
                                int new_count = count + 1;
                                string insert_qry = "insert into departs (dept_code,dept_abr,department,rec_date,type,status) values(" + new_count + ",'" + deptabri.Text + "','" + txt_dept_name.Text + "',#" + theDate + "#,'" + depttype.Text + "',1)";
                                cmd.CommandText = insert_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("departs", grid_department, "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1");
                                clearAllValues();
                            }
                            else
                            {
                                MessageBox.Show("Department-Name or Abbreviation already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(deptabri.Text) || string.IsNullOrWhiteSpace(txt_dept_name.Text) || string.IsNullOrWhiteSpace(depttype.Text))
                    {
                        MessageBox.Show("Somethigs are missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (!Regex.IsMatch(deptabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_dept_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from departs where dept_abr='" + deptabri.Text + "' and department='" + txt_dept_name.Text + "' and type='" + depttype.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string update_qry = "update departs set dept_abr='" + deptabri.Text + "',department='" + txt_dept_name.Text + "',type='" + depttype.Text + "' where dept_code=" + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("departs", grid_department, "select * ,iif([status]=0,1,0) as ali_column from departs where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Department-Name or Abbreviation already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }

            //end
        }
        #endregion
        #region Designation Show and Hide
        private void designation_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(dname.Text, UnallowedCharacters))
                {

                    int CursorIndex = dname.SelectionStart - 1;
                    dname.Text = dname.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    dname.SelectionStart = CursorIndex;
                    dname.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (did.Text != "") ? qry + " and des_code like'" + did.Text + "'" : qry;
                    qry = (dname.Text != "") ? qry + "and Instr(designation,'" + dname.Text + "')" : qry;
                    qry = (d_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (d_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("desigs", grid_designation, qry);
                }
                if (did.Text != " ")
                {
                    x = int.Parse(did.Text);
                    qry = (did.Text != "") ? qry + " and des_code like'" + did.Text + "'" : qry;
                    qry = (dname.Text != "") ? qry + "and Instr(designation,'" + dname.Text + "')" : qry;
                    qry = (d_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (d_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("desigs", grid_designation, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = did.SelectionStart - 1;
                    did.Text = did.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    did.SelectionStart = cursorIndex;
                    did.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (did.Text != "") ? qry + " and des_code like'" + did.Text + "'" : qry;
                    qry = (dname.Text != "") ? qry + "and Instr(designation,'" + dname.Text + "')" : qry;
                    qry = (d_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (d_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("desigs", grid_designation, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_designation_status_change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_designation.SelectedItem;
                string ID = (grid_designation.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from desigs where des_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update desigs set status=1 where des_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("desigs", grid_designation, "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update desigs set status=0 where des_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("desigs", grid_designation, "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1");
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_designation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_designation.SelectedItem;
                string ID = (grid_designation.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from desigs where des_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    dabri.Text = rd["des_abr"].ToString();
                    txt_des_name.Text = rd["designation"].ToString();
                    dtype.Text = rd["type"].ToString();
                }
                rd.Close();
                desig_add.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        private void add_designation_click(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(dabri.Text) || string.IsNullOrWhiteSpace(txt_des_name.Text) || string.IsNullOrWhiteSpace(dtype.Text))
                    {
                        MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(dabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_des_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from desigs where des_abr='" + dabri.Text + "'and designation='" + txt_des_name.Text + "' and type='" + dtype.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string max_qry = "select max(des_code) from desigs";
                                cmd.CommandText = max_qry;
                                int count = int.Parse(cmd.ExecuteScalar().ToString());
                                int new_count = count + 1;
                                string insert_qry = "insert into desigs (des_code,des_abr,designation,rec_date,type,status) values(" + new_count + ",'" + dabri.Text + "','" + txt_des_name.Text + "',#" + theDate + "#,'" + dtype.Text + "',1)";
                                cmd.CommandText = insert_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("desigs", grid_designation, "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Designation-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(dabri.Text) || string.IsNullOrWhiteSpace(txt_des_name.Text) || string.IsNullOrWhiteSpace(dtype.Text))
                    {
                        MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(dabri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_des_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from desigs where des_abr='" + dabri.Text + "'and designation='" + txt_des_name.Text + "' and type='" + dtype.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string update_qry = "update desigs set des_abr='" + dabri.Text + "',designation='" + txt_des_name.Text + "',type='" + dtype.Text + "' where des_code=" + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("desigs", grid_designation, "select * ,iif([status]=0,1,0) as ali_column from desigs where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Designation-Name or Abbreviation already exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }

        }
        #endregion
        #region Bank Show and Hide
        private void btn_Bank_status_change_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            txtblock.Text = "";
            try
            {
                bool zero = false, one = false;
                object item = grid_Bank.SelectedItem;
                string id = (grid_Bank.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int ID = int.Parse(id);
                string bank_name = (grid_Bank.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand oleDb = new OleDbCommand();
                oleDb.Connection = con;
                string select_qry = "Select * from [banks] where bank_code=" + id + "";
                oleDb.CommandText = select_qry;
                OleDbDataReader rd = oleDb.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    string qry_Delete = "Update [banks] set status = 1 where bank_code=" + id + "";
                    oleDb.CommandText = qry_Delete;
                    oleDb.ExecuteNonQuery();
                    //"select * ,iif([status]=0,1,0) as ali_column from banks where 1=1"
                    callGrid("banks", grid_Bank, "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1");
                    txtblock.Inlines.Add(new Run("Bank ") { Foreground = Brushes.Black, FontSize = 14 });
                    txtblock.Inlines.Add(new Run(bank_name) { Foreground = Brushes.Green, FontSize = 14 });
                    txtblock.Inlines.Add(new Run(" status is updated successfully") { Foreground = Brushes.Black, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                }
                if (one == true)
                {
                    string qry_Delete = "Update [banks] set status = 0 where bank_code=" + id + "";
                    oleDb.CommandText = qry_Delete;
                    oleDb.ExecuteNonQuery();
                    callGrid("banks", grid_Bank, "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1");
                    txtblock.Inlines.Add(new Run("Bank ") { Foreground = Brushes.Black, FontSize = 14 });
                    txtblock.Inlines.Add(new Run(bank_name) { Foreground = Brushes.Green, FontSize = 14 });
                    txtblock.Inlines.Add(new Run(" status is updated successfully") { Foreground = Brushes.Black, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                }
            }
            catch (Exception)
            {
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Red, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                //throw;
            }
        }

        private void bank_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(bank_Name.Text, UnallowedCharacters))
                {

                    int CursorIndex = bank_Name.SelectionStart - 1;
                    bank_Name.Text = bank_Name.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    bank_Name.SelectionStart = CursorIndex;
                    bank_Name.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (bank_ID.Text != "") ? qry + " and bank_code like'" + bank_ID.Text + "'" : qry;
                    qry = (bank_Name.Text != "") ? qry + "and Instr(bankname,'" + bank_Name.Text + "')" : qry;
                    qry = (bank_Open_Status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (bank_Close_Status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("banks", grid_Bank, qry);
                }
                if (bank_ID.Text != " ")
                {
                    x = int.Parse(bank_ID.Text);
                    qry = (bank_ID.Text != "") ? qry + " and bank_code like'" + bank_ID.Text + "'" : qry;
                    qry = (bank_Name.Text != "") ? qry + "and Instr(bankname,'" + bank_Name.Text + "')" : qry;
                    qry = (bank_Open_Status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (bank_Close_Status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("banks", grid_Bank, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = bank_ID.SelectionStart - 1;
                    bank_ID.Text = bank_ID.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    bank_ID.SelectionStart = cursorIndex;
                    bank_ID.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (bank_ID.Text != "") ? qry + " and bank_code like'" + bank_ID.Text + "'" : qry;
                    qry = (bank_Name.Text != "") ? qry + "and Instr(bankname,'" + bank_Name.Text + "')" : qry;
                    qry = (bank_Open_Status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (bank_Close_Status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("banks", grid_Bank, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void add_bank_click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            txtblock.Text = "";
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            if (add == true)
            {
                if (string.IsNullOrWhiteSpace(bank_abri.Text) || string.IsNullOrWhiteSpace(txt_bank_name.Text))
                {
                    MessageBox.Show("Operation Aborted!\n\nReason: Bank Abbreviation or Bank-Name is invalid.\n\nWrite a Bank Abbreviation first, then enter a valid Bank-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if ((!Regex.IsMatch(bank_abri.Text, @"[a-zA-z]+$")) || (!Regex.IsMatch(txt_bank_name.Text, @"[a-zA-Z]+$")))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Bank Abbreviation or Bank-Name is invalid.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "select * from banks where bank_abr='" + bank_abri.Text + "' or bankname='" + txt_bank_name.Text + "'";
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int chk_count = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        if (chk_count == 0)
                        {
                            string qry_count = "select max(bank_code) from [banks]";
                            cmd.CommandText = qry_count;
                            int count = int.Parse(cmd.ExecuteScalar().ToString());
                            //MessageBox.Show(count.ToString());
                            int new_count = count + 1;
                            string qry_insert = "insert into [banks] (bank_code, bank_abr, bankname, rec_date, status) values(" + new_count + ", " + bank_abri.Text.ToUpper() + ", '" + txt_bank_name.Text.ToUpper() + "', #" + theDate + "#, 1)";
                            cmd.CommandText = qry_insert;
                            cmd.ExecuteNonQuery();
                            txtblock.Inlines.Add(new Run("Bank ") { Foreground = Brushes.Black, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(txt_bank_name.Text.ToUpper()) { Foreground = Brushes.Green, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(" is added successfully") { Foreground = Brushes.Black, FontSize = 14 });
                            sb.Begin(txtblock);
                            sb.Begin(border);
                            callGrid("banks", grid_Bank, "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1");
                        }
                        else
                        {
                            txtblock.Inlines.Add(new Run("Bank or Abbreviation") { Foreground = Brushes.Black, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(txt_bank_name.Text.ToUpper()) { Foreground = Brushes.Red, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(bank_abri.Text.ToUpper()) { Foreground = Brushes.Red, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(" already exist") { Foreground = Brushes.Black, FontSize = 14 });
                            sb.Begin(txtblock);
                            sb.Begin(border);

                            MessageBox.Show("Operation Aborted!\n\nReason: Bank-Name or Abbreviation already exists! \n\nIf it is not a mistake then please change spellings little bit.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                clearAllValues();
            }
            if (update == true)
            {
                if (string.IsNullOrWhiteSpace(bank_abri.Text) || string.IsNullOrWhiteSpace(txt_bank_name.Text))
                {
                    MessageBox.Show("Operation Aborted!\n\nReason: Bank Abbreviation or Bank-Name is invalid.\n\nWrite a Bank Abbreviation first, then enter a valid Bank-Name please.\n\n", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if ((!Regex.IsMatch(bank_abri.Text, @"[a-zA-z]+$")) || (!Regex.IsMatch(txt_bank_name.Text, @"[a-zA-Z]+$")))
                    {
                        MessageBox.Show("Operation Aborted!\n\nReason: Bank Abbreviation or Bank-Name is invalid.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = con;
                        cmd.CommandText = "select * from banks where bank_abr='" + bank_abri.Text + "' or bankname='" + txt_bank_name.Text + "'";
                        OleDbDataReader rd = cmd.ExecuteReader();
                        int chk_count = Convert.ToInt32(rd.HasRows);
                        rd.Close();
                        if (chk_count == 0)
                        {
                            string update_qry = "update banks set bank_abr='" + bank_abri.Text.ToUpper() + "',bankname='" + txt_bank_name.Text.ToUpper() + "'";
                            cmd.CommandText = update_qry;
                            cmd.ExecuteNonQuery();
                            txtblock.Inlines.Add(new Run("Bank ") { Foreground = Brushes.Black, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(txt_bank_name.Text.ToUpper()) { Foreground = Brushes.Green, FontSize = 14 });
                            //txtblock.Inlines.Add(new Run(bank_abri.Text.ToUpper()) { Foreground = Brushes.Green, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(" is updated Successfully") { Foreground = Brushes.Black, FontSize = 14 });
                            sb.Begin(txtblock);
                            sb.Begin(border);
                            callGrid("banks", grid_Bank, "select * ,iif([status]=0,1,0) as ali_column from banks where 1=1");
                        }
                        else
                        {
                            txtblock.Inlines.Add(new Run("Bank or Abbreviation") { Foreground = Brushes.Black, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(bank_abri.Text.ToUpper()) { Foreground = Brushes.Red, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(" OR ") { Foreground = Brushes.Black, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(txt_bank_name.Text.ToUpper()) { Foreground = Brushes.Red, FontSize = 14 });
                            txtblock.Inlines.Add(new Run(" already exist") { Foreground = Brushes.Black, FontSize = 14 });
                            sb.Begin(txtblock);
                            sb.Begin(border);
                            MessageBox.Show("Operation Aborted!\n\nReason: Bank-Name or Abbreviation already exists! \n\nIf it is not a mistake then please change spellings little bit.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                clearAllValues();
            }

            //end
        }
        private void grid_bank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            txtblock.Text = "";
            try
            {
                object item = grid_Bank.SelectedItem;
                string ID = (grid_Bank.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                id_bank = int.Parse(ID);
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from banks where bank_code=" + id_bank + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    bank_abri.Text = rd["bank_abr"].ToString();
                    txt_bank_name.Text = rd["bankname"].ToString();
                }
                rd.Close();
                bank_add.Content = "UpDate";
                txtblock.Inlines.Add(new Run("Bank or Abbreviation") { Foreground = Brushes.Black, FontSize = 14 });
                txtblock.Inlines.Add(new Run(bank_abri.Text.ToUpper()) { Foreground = Brushes.Green, FontSize = 14 });
                txtblock.Inlines.Add(new Run(" OR ") { Foreground = Brushes.Black, FontSize = 14 });
                txtblock.Inlines.Add(new Run(txt_bank_name.Text.ToUpper()) { Foreground = Brushes.Green, FontSize = 14 });
                txtblock.Inlines.Add(new Run(" is selected for update") { Foreground = Brushes.Black, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                //throw;
            }
        }
        #endregion
        #region Payment Show and Hide
        private void paymenttype_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(payname.Text, UnallowedCharacters))
                {

                    int CursorIndex = payname.SelectionStart - 1;
                    payname.Text = payname.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    payname.SelectionStart = CursorIndex;
                    payname.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (payid.Text != "") ? qry + " and ptype_code like'" + payid.Text + "'" : qry;
                    qry = (payname.Text != "") ? qry + "and Instr(paytype,'" + payname.Text + "')" : qry;
                    qry = (pay_open_staus.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (pay_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("payment_types", grid_payType, qry);
                }
                if (payid.Text != " ")
                {
                    x = int.Parse(payid.Text);
                    qry = (payid.Text != "") ? qry + " and ptype_code like'" + payid.Text + "'" : qry;
                    qry = (payname.Text != "") ? qry + "and Instr(paytype,'" + payname.Text + "')" : qry;
                    qry = (pay_open_staus.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (pay_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("payment_types", grid_payType, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = payid.SelectionStart - 1;
                    payid.Text = payid.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    payid.SelectionStart = cursorIndex;
                    payid.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (payid.Text != "") ? qry + " and ptype_code like'" + payid.Text + "'" : qry;
                    qry = (payname.Text != "") ? qry + "and Instr(paytype,'" + payname.Text + "')" : qry;
                    qry = (pay_open_staus.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (pay_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("payment_types", grid_payType, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_paytype_Status_Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_payType.SelectedItem;
                string ID = (grid_payType.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from payment_types where ptype_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update payment_types set status=1 where ptype_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("payment_types", grid_payType, "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update payment_types set status=0 where ptype_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("payment_types", grid_payType, "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1");
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void add_paytype_click(object sender, RoutedEventArgs e)
        {
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(paytype_abri.Text) || string.IsNullOrWhiteSpace(txt_pay_type.Text))
                    {
                        MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(paytype_abri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_pay_type.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            //string find_qry = "select * from groups where group_abr='"+gabri.Text+"' and instr(group=" + txt_group_name.Text + ")";
                            string find_qry = "select * from payment_types WHERE ptype_abr='" + paytype_abri.Text + "' AND paytype='" + txt_pay_type.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string max_qry = "select max(ptype_code) from payment_types";
                                cmd.CommandText = max_qry;
                                int count = int.Parse(cmd.ExecuteScalar().ToString());
                                int new_count = count + 1;
                                string insert_qry = "insert into payment_types (ptype_code,ptype_abr,paytype,status) values(" + new_count + ",'" + paytype_abri.Text + "','" + txt_pay_type.Text + "',1)";
                                cmd.CommandText = insert_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("payment_types", grid_payType, "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Payment-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(paytype_abri.Text) || string.IsNullOrWhiteSpace(txt_pay_type.Text))
                    {
                        MessageBox.Show("Somethings are Missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(paytype_abri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(txt_pay_type.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from payment_types WHERE ptype_abr='" + paytype_abri.Text + "' AND paytype='" + txt_pay_type.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string update_qry = "update payment_types set ptype_abr='" + paytype_abri.Text + "',paytype='" + txt_pay_type.Text + "' where ptype_code=" + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("payment_types", grid_payType, "select * ,iif([status]=0,1,0) as ali_column from payment_types where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Payment-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_paytype_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_payType.SelectedItem;
                string ID = (grid_payType.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from payment_types where ptype_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    paytype_abri.Text = rd["ptype_abr"].ToString();
                    txt_pay_type.Text = rd["paytype"].ToString();

                }
                rd.Close();
                add_paytype.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        #endregion
        #region Salary Show and Hide

        #endregion
        #region Leave Type Show and Hide
        private void leave_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(leavetype.Text, UnallowedCharacters))
                {

                    int CursorIndex = leavetype.SelectionStart - 1;
                    leavetype.Text = leavetype.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    leavetype.SelectionStart = CursorIndex;
                    leavetype.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (leaveid.Text != "") ? qry + " and lt_code like'" + leaveid.Text + "'" : qry;
                    qry = (leavetype.Text != "") ? qry + "and Instr(leavetype,'" + leavetype.Text + "')" : qry;
                    qry = (leave_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (leave_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("leave_types", grid_leaveType, qry);
                }
                if (leaveid.Text != " ")
                {
                    x = int.Parse(leaveid.Text);
                    qry = (leaveid.Text != "") ? qry + " and lt_code like'" + leaveid.Text + "'" : qry;
                    qry = (leavetype.Text != "") ? qry + "and Instr(leavetype,'" + leavetype.Text + "')" : qry;
                    qry = (leave_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (leave_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("leave_types", grid_leaveType, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = leaveid.SelectionStart - 1;
                    leaveid.Text = leaveid.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    leaveid.SelectionStart = cursorIndex;
                    leaveid.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (leaveid.Text != "") ? qry + " and lt_code like'" + leaveid.Text + "'" : qry;
                    qry = (leavetype.Text != "") ? qry + "and Instr(leavetype,'" + leavetype.Text + "')" : qry;
                    qry = (leave_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (leave_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("leave_types", grid_leaveType, qry);
                    //throw;
                }
                //throw;
            }
        }

        private void btn_Leave_Status_change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_leaveType.SelectedItem;
                string ID = (grid_leaveType.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from leave_types where lt_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update leave_types set status=1 where lt_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update leave_types set status=0 where lt_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_leavetype_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_leaveType.SelectedItem;
                string ID = (grid_leaveType.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                string selectqry = "select * from leave_types where lt_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    lt_abri.Text = rd["lt_abr"].ToString();
                    lt_name.Text = rd["leavetype"].ToString();

                }
                rd.Close();
                lt_add.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        private void add_leavetype_click(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(lt_abri.Text) || string.IsNullOrWhiteSpace(lt_name.Text))
                    {
                        MessageBox.Show("Somethings are missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(lt_abri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(lt_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from leave_types where lt_abr='" + lt_abri.Text + "' and leaveType='" + lt_name.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string max_qry = "select max(lt_code) from leave_types";
                                cmd.CommandText = max_qry;
                                int count = int.Parse(cmd.ExecuteScalar().ToString());
                                int new_count = count + 1;
                                string insert_qry = "insert into leave_types (lt_code,lt_abr,leaveType,status) values(" + new_count + ",'" + lt_abri.Text + "','" + lt_name.Text + "',1)";
                                cmd.CommandText = insert_qry;
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                MessageBox.Show("Leave-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                    clearAllValues();
                }
                callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(lt_abri.Text) || string.IsNullOrWhiteSpace(lt_name.Text))
                    {
                        MessageBox.Show("Somethings are missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(lt_abri.Text, @"[a-zA-Z]+$") || !Regex.IsMatch(lt_name.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from leave_types where lt_abr='" + lt_abri.Text + "' and leaveType='" + lt_name.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string update_qry = "update leave_types set lt_abr='" + lt_abri.Text + "',leaveType='" + lt_name.Text + "' where lt_code=" + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("leave_types", grid_leaveType, "select * ,iif([status]=0,1,0) as ali_column from leave_types where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Leave-Name or Abbreviation is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }
        #endregion
        #region EOBI Show and Hide


        #endregion
        #region Provident Fund Show and Hide

        #endregion
        #region Social Security Show and Hide


        #endregion
        #region EmpAddEdit Show and Hide
        private void menu_EmpAddEdit_Click(object sender, RoutedEventArgs e)
        {
            if (tab_EmpAddEdit.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_EmpAddEdit;
            }
            else if (var_EmpAddEdit == true)
            {
                tabControl.Items.Add(tab_EmpAddEdit);
                var_EmpAddEdit = false;
                tab_EmpAddEdit.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_EmpAddEdit;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_EmpAddEdit.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_EmpAddEdit;
                tab_count++;

            }
        }

        private void btn_update_EOBI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = "";
                if (string.IsNullOrWhiteSpace(txt_EOBI.Text)) { msg = msg + "\n- Invalid value in EOBI field."; }
                if (string.IsNullOrWhiteSpace(txt_EPF.Text)) { msg = msg + "\n- Invalid value in EPS field."; }
                if (string.IsNullOrWhiteSpace(txt_ESS.Text)) { msg = msg + "\n- Invalid value in ESS field."; }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    MessageBox.Show("اسلامُ علیکم\n\nInvalid data found. Find details below: -\n\n" + msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();

                    //OleDbCommand cmd = new OleDbCommand();
                    //cmd.Connection = con;
                    string qry_update = "UPDATE [EOBI_PF_SS] SET EOBI=" + txt_EOBI.Text + ", EPF=" + txt_EPF.Text + ", ESS=" + txt_ESS.Text + " ";
                    cmd.CommandText = (qry_update);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("اسلامُ علیکم\n\nJob done.\n\nAll fields updated successfully.\n", "INFORMATION", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_EmpAddEdit_Click(object sender, RoutedEventArgs e)
        {
            tab_count--;
            if (tab_count == 0)
            {
                tabControl.Items.Remove(tab_EmpAddEdit);
                var_EmpAddEdit = true;
                tab_EmpAddEdit.Visibility = Visibility.Hidden;
                tabControl.Visibility = Visibility.Hidden;
            }
            else
            {
                tabControl.Items.Remove(tab_EmpAddEdit);
                var_EmpAddEdit = true;
                tab_EmpAddEdit.Visibility = Visibility.Hidden;
            }
        }


        #endregion
        private void menu_Advanceloan_Click(object sender, RoutedEventArgs e)
        {
            tbl_name = "loans";
            fields = "card_no, start_date, status";
            callGrid(tbl_name, grid_loan, "SELECT * FROM [" + tbl_name + "] WHERE 1=1");
            if (tab_Deductions.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsAdvanceLoan;
            }
            else if (var_Deductions == true)
            {
                tabControl.Items.Add(tab_Deductions);
                var_Deductions = false;
                tab_Deductions.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsAdvanceLoan;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_Deductions.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsAdvanceLoan;
                tab_count++;
            }
        }
        #region Menus Click
        private void menu_Disincentive_click(object sender, RoutedEventArgs e)
        {
            if (tab_Deductions.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsDisincentive;
            }
            else if (var_Deductions == true)
            {
                tabControl.Items.Add(tab_Deductions);
                var_Deductions = false;
                tab_Deductions.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsDisincentive;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_Deductions.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Deductions;
                tab_inDeductions.SelectedItem = tab_DeductionsDisincentive;
                tab_count++;
            }
        }
        private void btn_Deductions_click(object sender, RoutedEventArgs e)
        {
            tab_count--;
            if (tab_count == 0)
            {
                tabControl.Items.Remove(tab_Deductions);
                var_Deductions = true;
                tab_Deductions.Visibility = Visibility.Hidden;
                tabControl.Visibility = Visibility.Hidden;
            }
            else
            {
                tabControl.Items.Remove(tab_Deductions);
                var_Deductions = true;
                tab_Deductions.Visibility = Visibility.Hidden;
            }
        }
        private void menu_EarningsEarning_Click(object sender, RoutedEventArgs e)
        {
            if (tab_Earnings.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsEarnings;
            }
            else if (var_Earnings == true)
            {
                tabControl.Items.Add(tab_Earnings);
                var_Earnings = false;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsEarnings;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsEarnings;
                tab_count++;
            }
        }

        private void menu_EarningsIncentive_Click(object sender, RoutedEventArgs e)
        {
            if (tab_Earnings.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsIncentive;
            }
            else if (var_Earnings == true)
            {
                tabControl.Items.Add(tab_Earnings);
                var_Earnings = false;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsIncentive;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningsIncentive;
                tab_count++;
            }
        }

        private void menu_EarningstadaExpenses_Click(object sender, RoutedEventArgs e)
        {
            if (tab_Earnings.Visibility == Visibility.Visible)
            {
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningstadaExpense;
            }
            else if (var_Earnings == true)
            {
                tabControl.Items.Add(tab_Earnings);
                var_Earnings = false;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningstadaExpense;
                tab_count++;
            }
            else
            {
                tabControl.Visibility = Visibility.Visible;
                tab_Earnings.Visibility = Visibility.Visible;
                tabControl.SelectedItem = tab_Earnings;
                tab_inEarnings.SelectedItem = tab_EarningstadaExpense;
                tab_count++;
            }
        }

        private void btn_Earnings_Click(object sender, RoutedEventArgs e)
        {
            tab_count--;
            if (tab_count == 0)
            {
                tabControl.Items.Remove(tab_Earnings);
                var_Earnings = true;
                tab_Earnings.Visibility = Visibility.Hidden;
                tabControl.Visibility = Visibility.Hidden;
            }
            else
            {
                tabControl.Items.Remove(tab_Earnings);
                var_Earnings = true;
                tab_Earnings.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Loans And Advances
        private void call_grid_loan(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                //grid_loan.Items.Clear();
                string query = "SELECT * FROM [loans] WHERE card_no='" + txt_empl_no.Text + "'";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = query;

                OleDbDataReader rd = cmd.ExecuteReader();
                //Console.WriteLine(grid_loan);
                var abc = grid_loan.ToString();
                grid_loan.Items.Clear();
                foreach (var item in rd) { grid_loan.Items.Add(item); }
                //callLoans(grid_loan, query);
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoanAction(object sender, System.EventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                string qry;
                qry = "SELECT * FROM [loans] WHERE 1=1";
                qry = (txt_EmpID_Search.Text != "") ? qry + " and card_no like'" + txt_EmpID_Search.Text + "'" : qry;
                qry = (txt_Ammount_Search.Text != "") ? qry + " and loan_amount like'" + txt_Ammount_Search.Text + "'" : qry;
                qry = (Radio_Loan_Close.IsChecked == true) ? qry + " and status=0" : qry;
                qry = (Radio_Loan_Open.IsChecked == true) ? qry + " and status =1" : qry;
                callLoans(grid_loan, qry);
            }
            catch (Exception)
            {

            }
        }
        private void callLoans(DataGrid grid, string qry)
        {
            //if (con.State != ConnectionState.Open)
            //    con.Open();
            //OleDbCommand cmd = new OleDbCommand();
            //cmd.Connection = con;
            //cmd.CommandText = qry;
            //OleDbDataReader rd = cmd.ExecuteReader();
            //var abc = grid_loan.ToString();
            //int count = 0;
            //List<LoansAndAdvances> list = new List<LoansAndAdvances>();
            //while (rd.Read())
            //{
            //    list.Add(new LoansAndAdvances()
            //    {
            //        card_no = rd["card_no"].ToString(),
            //        loan_id = rd["loan_id"].ToString(),
            //        loan_amount = rd["loan_amount"].ToString(),
            //        instal_amount = rd["instal_amount"].ToString(),
            //        no_of_instal = rd["no_of_instal"].ToString(),
            //        last_installment_deducted_on = rd["last_installment_deducted_on"].ToString(),
            //        status = rd["status"].ToString(),
            //        start_date = rd["start_date"].ToString(),
            //        end_date = rd["end_date"].ToString()
            //    });
            //    count++;
            //}
            //grid.ItemsSource = list;
            //rd.Close();
            //lbl_record.Content = (count > 0) ? "Showing " + count + " record(s)." : lbl_record.Content = "No record found.";
            //txt_empl_no.Clear();
            //txt_loan_amnt.Clear();
            //txt_installments.Clear();
            //inst_amnt = 0;
            //txt_empl_no.Focus();
        }
        double inst_amnt;
        private void loan_add(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            //try
            //{
            if (add == true)
            {
                MessageBoxResult closing_called = MessageBox.Show("اسلامُ علیکم\nFollowing loan is about to be added in the system: -\n\nEmpl. # " + txt_empl_no.Text + ",\nLoam amount = " + txt_loan_amnt.Text + ",\nNo. of installments = " + txt_installments.Text + "\nMonthly installments = " + txt_inst_amnt.Text + "\n\nDo yo want to continue ?\n", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (closing_called == MessageBoxResult.Yes)
                {

                    string qry_count = "SELECT count(card_no) FROM [loans] WHERE card_no='" + txt_empl_no.Text + "'";
                    cmd.CommandText = (qry_count);
                    Int32 count = (Int32)cmd.ExecuteScalar() + 1;
                    var dt = DateTime.Now.Date.ToShortDateString();
                    cmd.Connection = con;
                    string qry_insert = "";
                    qry_insert = "insert into [loans] (card_no, loan_id, loan_amount, no_of_instal, instal_amount, balance, status, start_date) values('" + txt_empl_no.Text + "', " + count + ", " + txt_loan_amnt.Text + ", " + txt_installments.Text + ", " + inst_amnt.ToString("0.##") + ", " + 0 + ", " + 1 + ", #" + theDate + "# )";
                    cmd.CommandText = (qry_insert);
                    cmd.ExecuteNonQuery();
                    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                    txtblock.Text = "";
                    txtblock.Inlines.Add(new Run("A new loan for Empl. # " + txt_empl_no.Text + " has been added successfully.") { Foreground = Brushes.Green, FontSize = 14 });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                    call_grid_loan(sender, null);
                    txt_empl_no.Clear();
                    count = 0;
                    txt_loan_amnt.Clear();
                    txt_installments.Clear();
                    inst_amnt = 0;
                    txt_empl_no.Focus();
                }
                else
                {
                    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                    txtblock.Text = "";
                    txtblock.Inlines.Add(new Run("Record not entered, process cancelled") { Foreground = Brushes.Red });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                }
            }
            if (update == true)
            {
                string update_qry = "update loans set loan_amount='" + txt_loan_amnt.Text + "',no_of_instal='" + txt_installments.Text + "',instal_amount='" + txt_inst_amnt.Text + "' where card_no='" + txt_empl_no.Text + "' and loan_id=" + loan_id + "";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = update_qry;
                cmd.ExecuteNonQuery();
                txt_empl_no.IsEnabled = true;
                string qry = "select * from loans";
                btn_loan_add.Content = "Add Record";
                callLoans(grid_areas, qry);

            }
            //}
            //catch (Exception)
            //{
            //    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
            //    txtblock.Text = "";
            //    txtblock.Inlines.Add(new Run("Some thing went wrong, connect the Admin") { Foreground = Brushes.Red });
            //    sb.Begin(txtblock);
            //    sb.Begin(border);
            //    MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void zone_combo_selectionchanged(object sender, SelectionChangedEventArgs e)
        {
            //if (combo_zonetype.SelectedIndex == 0)
            //{
            //    MessageBox.Show("City Office");
            //}

        }
        
        #region Religion Actions
        private void btn_relicion_statusChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                bool one = false, zero = false;
                object item = grid_Religion.SelectedItem;
                string ID = (grid_Religion.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                int id = int.Parse(ID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                cmd.CommandText = "select * from religions where rel_code=" + id + "";
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    cmd.CommandText = "update religions set status=1 where rel_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                }

                if (one == true)
                {
                    cmd.CommandText = "update religions set status=0 where rel_code=" + id + "";
                    cmd.ExecuteNonQuery();
                    callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
                //throw;
            }
        }
        private void religion_Action(object sender, EventArgs e)
        {
            int x;
            string qry = "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1";
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(relName.Text, UnallowedCharacters))
                {

                    int CursorIndex = relName.SelectionStart - 1;
                    relName.Text = relName.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    relName.SelectionStart = CursorIndex;
                    relName.SelectionLength = 0;

                    //callGrid(tbl_name, grid_areas, qry);
                    qry = (relID.Text != "") ? qry + " and rel_code like'" + relID.Text + "'" : qry;
                    qry = (relName.Text != "") ? qry + "and Instr(religion,'" + relName.Text + "')" : qry;
                    qry = (rel_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (rel_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("religions", grid_Religion, qry);
                }
                if (relID.Text != " ")
                {
                    x = int.Parse(relID.Text);
                    qry = (relID.Text != "") ? qry + " and rel_code like'" + relID.Text + "'" : qry;
                    qry = (relName.Text != "") ? qry + "and Instr(religion,'" + relName.Text + "')" : qry;
                    qry = (rel_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (rel_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("religions", grid_Religion, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = relID.SelectionStart - 1;
                    relID.Text = relID.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    relID.SelectionStart = cursorIndex;
                    relID.SelectionLength = 0;
                }
                catch (Exception)
                {
                    //x = int.Parse(bank_ID.Text);
                    qry = (relID.Text != "") ? qry + " and rel_code like'" + relID.Text + "'" : qry;
                    qry = (relName.Text != "") ? qry + "and Instr(religion,'" + relName.Text + "')" : qry;
                    qry = (rel_open_status.IsChecked == true) ? qry + "and status=1" : qry;
                    qry = (rel_close_status.IsChecked == true) ? qry + "and status=0" : qry;
                    callGrid("religions", grid_Religion, qry);
                    //throw;
                }
                //throw;
            }
        }
        private void add_religion_click(object sender, RoutedEventArgs e)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            bool add, update;
            add = sender.ToString().Contains("Add Record");
            update = sender.ToString().Contains("UpDate");
            try
            {
                if (add == true)
                {
                    if (string.IsNullOrWhiteSpace(txt_religion.Text))
                    {
                        MessageBox.Show("Somethings are missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(txt_religion.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from religions where religion='" + txt_religion.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string max_qry = "select max(rel_code) from religions";
                                cmd.CommandText = max_qry;
                                int count = int.Parse(cmd.ExecuteScalar().ToString());
                                int new_count = count + 1;
                                string insert_qry = "insert into religions (rel_code,religion,rec_date,status) values(" + new_count + ",'" + txt_religion.Text + "',#" + theDate + "#,1)";
                                cmd.CommandText = insert_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Religion is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
                if (update == true)
                {
                    if (string.IsNullOrWhiteSpace(txt_religion.Text))
                    {
                        MessageBox.Show("Somethings are missing", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (!Regex.IsMatch(txt_religion.Text, @"[a-zA-Z]+$"))
                        {
                            MessageBox.Show("Only characters are Supported");
                        }
                        else
                        {
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = con;
                            string find_qry = "select * from religions where religion='" + txt_religion.Text + "'";
                            cmd.CommandText = find_qry;
                            OleDbDataReader rd = cmd.ExecuteReader();
                            int check = Convert.ToInt32(rd.HasRows);
                            rd.Close();
                            if (check == 0)
                            {
                                string update_qry = "update religions set religion='" + txt_religion.Text + "' where rel_code=" + i + "";
                                cmd.CommandText = update_qry;
                                cmd.ExecuteNonQuery();
                                callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                            }
                            else
                            {
                                MessageBox.Show("Religion is already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    clearAllValues();
                }
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        private void grid_religion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                object item = grid_Religion.SelectedItem;
                string ID = (grid_Religion.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                //string zonetype = (grid_payType.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                //int zcode = int.Parse(zonecode);
                if (con.State != ConnectionState.Open)
                    con.Open();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = con;
                int id = int.Parse(ID);/*int.Parse(selectedarea.area_code.ToString());*/
                i = id;
                //MessageBox.Show(i.ToString());
                //callGrid("religions", grid_Religion, "select * ,iif([status]=0,1,0) as ali_column from religions where 1=1");
                string selectqry = "select * ,iif([status]=0,1,0) as ali_column from religions where rel_code=" + id + "";
                cmd.CommandText = selectqry;
                OleDbDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    //paytype_abri.Text = rd["ptype_abr"].ToString();
                    txt_religion.Text = rd["religion"].ToString();

                }
                rd.Close();
                add_religion.Content = "UpDate";
            }
            catch (Exception)
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                txtblock.Inlines.Add(new Run("Something went wrong.Connect with Admin") { Foreground = Brushes.Green, FontSize = 14 });
                sb.Begin(txtblock);
                sb.Begin(border);
            }
        }

        #endregion

        private void menu_emp_details_Click(object sender, RoutedEventArgs e)
        {
            window_emps windEmps = new window_emps();
            windEmps.Owner = this;
            windEmps.Show();
        }

       

       

       

        
       

        private void txt_empl_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddEditEmployee(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hi, Add Edit Employee - Wizard");
        }

        private void calculate_installmets(object sender, RoutedEventArgs e)
        {
            try
            {
                double loan_amnt, instmnt;
                if (string.IsNullOrWhiteSpace(txt_loan_amnt.Text) || string.IsNullOrWhiteSpace(txt_installments.Text))
                {
                    txt_loan_amnt.Clear();
                    txt_loan_amnt.Focus();
                    if (txt_installments.Text.Length >= 2)
                        btn_loan_add.Focus();
                }
                else
                {
                    loan_amnt = Convert.ToDouble(txt_loan_amnt.Text);
                    instmnt = Convert.ToDouble(txt_installments.Text);
                    inst_amnt = loan_amnt / instmnt;
                    txt_inst_amnt.Text = inst_amnt.ToString("n2");
                    if (txt_installments.Text.Length >= 2)
                        btn_loan_add.Focus();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void announce_employee(object sender, RoutedEventArgs e)
        {

            try
            {

                //string des_code = null, dpt_code = null;
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand command = new OleDbCommand();
                command.Connection = con;
                string query = "SELECT emps.sstop,emps.status, emps.name, desigs.designation, departs.department, departs.dept_abr, emps.card_no FROM departs INNER JOIN(desigs INNER JOIN emps ON desigs.des_code = emps.des_code) ON departs.dept_code = emps.dpt_code WHERE(((emps.card_no) = '" + txt_empl_no.Text + "'))";
                cmd.CommandText = query;
                OleDbDataReader dr = cmd.ExecuteReader();
                string announcement = "";
                int chk_rows = Convert.ToInt32(dr.HasRows);

                if (dr.Read())
                {
                    if ((dr["status"].ToString() == "A") && (dr["sstop"].ToString() == "N"))
                    {
                        Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                        announcement = announcement + (dr["name"].ToString().ToUpper()) + ", " + (dr["dept_abr"].ToString().ToUpper());
                        txtblock.Text = "";
                        txtblock.Inlines.Add(new Run("(Active:)  ") { Foreground = Brushes.Green });
                        txtblock.Inlines.Add(new Run(announcement) { Foreground = Brushes.Black });
                        sb.Begin(txtblock);
                        sb.Begin(border);
                    }

                    else
                    {
                        Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                        txtblock.Text = "";
                        txtblock.Inlines.Add(new Run("(inActive:)  ") { Foreground = Brushes.Red });
                        txtblock.Inlines.Add(new Run("Employee not found") { Foreground = Brushes.Black });
                        sb.Begin(txtblock);
                        sb.Begin(border);
                    }
                    dr.Close();
                }
                else
                {
                    Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                    txtblock.Text = "";
                    txtblock.Inlines.Add(new Run("Employee not found") { Foreground = Brushes.Red });
                    sb.Begin(txtblock);
                    sb.Begin(border);
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("اسلامُ علیکم\n(Probably Emps table problem..\nIt seems that something went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_Delete_Row_loan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Storyboard sb = Resources["sbHideAnimation"] as Storyboard;
                txtblock.Text = "";
                bool zero = false, one = false;
                string empID = null, loanID = null;
                int lid = 0;
                object item = grid_loan.SelectedItem;
                string ID = (grid_loan.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                string loan = (grid_loan.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                empID = ID;
                loanID = loan;
                lid = int.Parse(loanID);
                if (con.State != ConnectionState.Open)
                    con.Open();
                OleDbCommand oleDb = new OleDbCommand();
                oleDb.Connection = con;
                string select_qry = "Select * from [loans] where card_no='" + empID + "' and loan_id=" + lid + "";
                oleDb.CommandText = select_qry;
                OleDbDataReader rd = oleDb.ExecuteReader();
                while (rd.Read())
                {
                    if (rd["status"].ToString() == "0") { zero = true; }
                    if (rd["status"].ToString() == "1") { one = true; }
                }
                rd.Close();
                if (zero == true)
                {
                    string qry_Delete = "Update [loans] set status = 1 where card_no='" + empID + "' and loan_id=" + lid + "";
                    oleDb.CommandText = qry_Delete;
                    oleDb.ExecuteNonQuery();

                }
                if (one == true)
                {
                    string qry_Delete = "Update [loans] set status = 0 where card_no='" + empID + "' and loan_id=" + lid + "";
                    oleDb.CommandText = qry_Delete;
                    oleDb.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        private void btn_MaintainEOBI_Click(object sender, RoutedEventArgs e)
        {

        }
        private void clearAllValues()
        {
            try
            {
                i = 0;
                txt_religion.Clear();
                group_add.Content = "Add Record";
                g_open_status.IsChecked = false;
                g_close_status.IsChecked = false;
                gid.Clear();
                gname.Clear();
                gabri.Clear();
                txt_group_name.Clear();
                gabri.Focus();
                add_religion.Content = "Add Record";
                rel_open_status.IsChecked = false;
                rel_close_status.IsChecked = false;
                zoneid.Clear();
                zone_Name.Clear();
                zone_close_status.IsChecked = false;
                zone_close_status.IsChecked = false;
                relName.Clear();
                relID.Clear();
                lt_abri.Clear();
                paytype_abri.Clear();
                txt_pay_type.Clear();
                paytype_abri.Focus();
                add_paytype.Content = "Add Record";
                pay_open_staus.IsChecked = false;
                pay_close_status.IsChecked = false;
                payid.Clear();
                payname.Clear();
                lt_abri.Focus();
                lt_add.Content = "Add Record";
                lt_name.Clear();
                leaveid.Clear();
                leavetype.Clear();
                leave_open_status.IsChecked = false;
                leave_close_status.IsChecked = false;
                dabri.Clear();
                dtype.SelectedIndex = -1;
                bank_ID.Clear();
                bank_Name.Clear();
                bank_Open_Status.IsChecked = false;
                bank_Close_Status.IsChecked = false;
                txt_bank_name.Clear();
                bank_abri.Clear();
                bank_abri.Focus();
                bank_add.Content = "Add Record";
                txt_area_Name.Clear();
                comb_zone.SelectedIndex = -1;
                search_zone.Clear();
                search_area.Clear();
                radio_open.IsChecked = false;
                radio_closed.IsChecked = false;
                txt_area_Name.Clear();
                comb_zone.SelectedIndex = -1;
                area_Add.Content = "Add Record";
                combo_zonetype.SelectedIndex = -1;
                txt_zone_name.Clear();
                txt_zone_name.Focus();
                add_zone.Content = "Add Record";
                dept_add.Content = "Add Record";
                desig_add.Content = "Add Record";
                deptid.Clear();
                departmentname.Clear();
                deptabri.Clear();
                txt_dept_name.Clear();
                txt_des_name.Clear();
                depttype.SelectedIndex = -1;
                did.Clear();
                dname.Clear();
                dept_close_status.IsChecked = false;
                dept_open_status.IsChecked = false;
                d_open_status.IsChecked = false;
                d_close_status.IsChecked = false;
            }
            catch (Exception)
            {

                //throw;
            }
        }
    }
}

