using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Payroll
{
    /// <summary>
    /// Interaction logic for window_emps.xaml
    /// </summary>
    public partial class window_emps : Window
    {
        OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString());
        public window_emps()
        {
            con.Open();
            InitializeComponent();
            string qry;
            qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
            callGrid("emps_details", grid_emps, qry);
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
                gridName.Items.Clear();
                foreach (var item in rd)
                {
                    gridName.Items.Add(item);
                    counter++;
                }
                rd.Close();
                lbl_records.Content = (counter > 0) ? "Showing " + counter + " record(s)" : lbl_records.Content = "No record found.";
            }
            catch (Exception)
            {
                MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void management_action(object sender, System.EventArgs e)
        {
            callGrid("emps_details", grid_emps, "SELECT * FROM [emps_details] WHERE 1=1 ");
            int x;
            try
            {
                char[] UnallowedCharacters = { '0', '1',
                                           '2', '3',
                                           '4', '5',
                                           '6', '7','\'',
                                           '8', '9','!','@','#','$','%','^','&','*','('
                ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
                if (textContainsUnallowedCharacter(search_emps_name.Text, UnallowedCharacters))
                {
                    int CursorIndex = search_emps_name.SelectionStart - 1;
                    search_emps_name.Text = search_emps_name.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    search_emps_name.SelectionStart = CursorIndex;
                    search_emps_name.SelectionLength = 0;
                    string qry;
                    qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
                    if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
                    if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
                    if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
                    if (radio_closed.IsChecked == true)
                    {
                        qry = qry + " AND status='A' AND sstop='S' " +
"OR status = 'R' and sstop = 'N'" +
"OR status = 'R' and sstop = 'S'" +
"OR status = 'T' and sstop = 'N'" +
"OR status = 'T' and sstop = 'S'";
                    }
                    //MessageBox.Show("" + qry);
                    callGrid("emps_details", grid_emps, qry);
                }
                if (textContainsUnallowedCharacter(search_emps_area.Text, UnallowedCharacters))
                {

                    int CursorIndex = search_emps_area.SelectionStart - 1;
                    search_emps_area.Text = search_emps_area.Text.Remove(CursorIndex, 1);

                    //Align Cursor to same index
                    search_emps_area.SelectionStart = CursorIndex;
                    search_emps_area.SelectionLength = 0;
                    string qry;
                    qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
                    if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
                    if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
                    if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
                    if (radio_closed.IsChecked == true)
                    {
                        qry = qry + " AND status='A' AND sstop='S' " +
                            "OR status = 'R' and sstop = 'N'" +
                            "OR status = 'R' and sstop = 'S'" +
                            "OR status = 'T' and sstop = 'N'" +
                            "OR status = 'T' and sstop = 'S'";
                    }
                    //if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
                    //MessageBox.Show("" + qry);
                    callGrid("emps_details", grid_emps, qry);
                }
                if (search_emps_no.Text != " ")
                {
                    x = int.Parse(search_emps_no.Text);
                    string qry;
                    qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
                    if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
                    if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
                    if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
                    if (radio_closed.IsChecked == true)
                    {
                        qry = qry + " AND status='A' AND sstop='S' " +
                            "OR status = 'R' and sstop = 'N'" +
                            "OR status = 'R' and sstop = 'S'" +
                            "OR status = 'T' and sstop = 'N'" +
                            "OR status = 'T' and sstop = 'S'";
                    }
                    //if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
                    //MessageBox.Show("" + qry);
                    callGrid("emps_details", grid_emps, qry);
                }
            }
            catch (Exception)
            {
                try
                {
                    int cursorIndex = search_emps_no.SelectionStart - 1;
                    search_emps_no.Text = search_emps_no.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    search_emps_no.SelectionStart = cursorIndex;
                    search_emps_no.SelectionLength = 0;
                }
                catch (Exception)
                {
                    string qry;
                    qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
                    if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
                    if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
                    if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
                    if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
                    if (radio_closed.IsChecked == true)
                    {
                        qry = qry + " AND status='A' AND sstop='S' " +
                            "OR status = 'R' and sstop = 'N'" +
                            "OR status = 'R' and sstop = 'S'" +
                            "OR status = 'T' and sstop = 'N'" +
                            "OR status = 'T' and sstop = 'S'";
                    }
                    //if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
                    //MessageBox.Show("" + qry);
                    callGrid("emps_details", grid_emps, qry);
                    //MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //throw;
                }
            }
            //int x;
            //try
            //{
            //    char[] UnallowedCharacters = { '0', '1',
            //                               '2', '3',
            //                               '4', '5',
            //                               '6', '7','\'',
            //                               '8', '9','!','@','#','$','%','^','&','*','('
            //    ,')','-','_','+','=','[',']','{','}','\\','|','"',';',':','/','?','.','>',',','<','~'};
            //    if (textContainsUnallowedCharacter(search_emps_area.Text, UnallowedCharacters))
            //    {

            //        int CursorIndex = search_emps_area.SelectionStart - 1;
            //        search_emps_area.Text = search_emps_area.Text.Remove(CursorIndex, 1);

            //        //Align Cursor to same index
            //        search_emps_area.SelectionStart = CursorIndex;
            //        search_emps_area.SelectionLength = 0;
            //        string qry;
            //        qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
            //        if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
            //        if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
            //        if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
            //        if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
            //        if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
            //        //MessageBox.Show("" + qry);
            //        callGrid("emps_details", grid_emps, qry);
            //    }
            //    if (search_emps_name.Text != " ")
            //    {
            //        int CursorIndex = search_emps_name.SelectionStart - 1;
            //        search_emps_name.Text = search_emps_name.Text.Remove(CursorIndex, 1);
            //        //x = int.Parse(search_emps_name.Text);
            //        search_emps_name.SelectionStart = CursorIndex;
            //        search_emps_name.SelectionLength = 0;
            //        string qry;
            //        qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
            //        if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
            //        if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
            //        if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
            //        if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
            //        if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
            //        //MessageBox.Show("" + qry);
            //        callGrid("emps_details", grid_emps, qry);
            //    }
            //}
            //catch (Exception)
            //{
            //    try
            //    {
            //        int cursorIndex = search_emps_name.SelectionStart - 1;
            //        search_emps_name.Text = search_emps_name.Text.Remove(cursorIndex, 1);

            //        //Align Cursor to same index
            //        search_emps_name.SelectionStart = cursorIndex;
            //        search_emps_name.SelectionLength = 0;
            //    }
            //    catch (Exception)
            //    {
            //        string qry;
            //        qry = "SELECT * FROM [emps_details] WHERE 1=1 ";
            //        if (search_emps_name.Text != "") { qry = qry + " AND InStr(name, '" + search_emps_name.Text + "')"; }
            //        if (search_emps_area.Text != "") { qry = qry + " AND InStr(area_name, '" + search_emps_area.Text + "')"; }
            //        if (search_emps_no.Text != "") { qry = qry + " AND empl_no like '" + search_emps_no.Text + "'"; }
            //        if (radio_open.IsChecked == true) { qry = qry + " AND status='A' AND sstop='N'"; }
            //        if (radio_closed.IsChecked == true) { qry = qry + " AND status<>'A' AND sstop<>'N'"; }
            //        //MessageBox.Show("" + qry);
            //        callGrid("emps_details", grid_emps, qry);
            //        //MessageBox.Show("اسلامُ علیکم\nIt seems that some thing went wrong.\nPlease connect with Admin.", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        //throw;
            //    }
            //}

        }

        private bool textContainsUnallowedCharacter(string T, char[] UnallowedCharacters)
        {
            for (int i = 0; i < UnallowedCharacters.Length; i++)
                if (T.Contains(UnallowedCharacters[i]))
                    return true;

            return false;
        }




    }


}
