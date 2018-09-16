using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Mir;
using Core.Mir.BaseTypes;
using Core.Mir.Enumerations;
using Core.Mir.Interfaces;
using System.Configuration;
using Npgsql;

namespace MURRDesktop
{
    public partial class ScalarAndTimeSeries : Form
    {
        private string usedDataSource = string.Empty;
        private DateTime usedDateFrom = DateTime.Today;
        private DateTime usedDateTo = DateTime.Today;
        private HashSet<string> usedColumns = new HashSet<string>();
        private Dictionary<string, Dictionary<DateTime, decimal>> values = null;
        private string _connection =
            ConfigurationManager.AppSettings["mirconnection"];
        string _ident = string.Empty;
        List<string> dataSources = new List<string>();
        Dictionary<string, string> _finField = new Dictionary<string, string>();

        public ScalarAndTimeSeries(string ident)
        {
            InitializeComponent();
            _ident = ident;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            cbDataSource3.Items.Clear();

            dtpFrom.Value = DateTime.Today.Date.AddYears(-1);
            dtpTo.Value = DateTime.Today.Date;

            #region загрузить доступные источники данных
            using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    string query = "select ds.ident as ident from data_source ds";
                    command.CommandText = query;
                    using (NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string ds = dataReader["ident"].ToString();
                            comboBox1.Items.Add(ds);
                            comboBox2.Items.Add(ds);
                            cbDataSource3.Items.Add(ds);
                        }
                    }
                }
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
                comboBox2.SelectedItem = comboBox2.Items[0];
                cbDataSource3.SelectedItem = cbDataSource3.Items[0];
            }
            #endregion

            #region загрузить все доступные финансовые атрибуты
            using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    string query = "select ff.fif_id as id, ff.ident as ident from fin_field ff";
                    command.CommandText = query;
                    using (NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string id = dataReader["id"].ToString();
                            string ds = dataReader["ident"].ToString();
                            _finField.Add(id, ds);
                        }
                    }
                }
            }
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
            }
            #endregion
        }

        private void ScalarAndTimeSeries_Load(object sender, EventArgs e)
        {

        }

        private void ScalarAndTimeSeries_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void dateReport_ValueChanged(object sender, EventArgs e)
        {


        }

        private void UpdateScalarTable()
        {
            DataTable dataTable = new DataTable();
            string identColumn = "Идентификатор";
            string titleColumn = "Наименование";
            string valueColumn = "Значение";
            DataColumn columnIdent = new DataColumn(identColumn, typeof(string));
            DataColumn columnTitle = new DataColumn(titleColumn, typeof(string));
            DataColumn columnValue = new DataColumn(valueColumn, typeof(string));

            dataTable.Columns.AddRange(new DataColumn[] { columnIdent, columnTitle, columnValue });

            string dataSource = comboBox1.SelectedItem.ToString();
            DateTime reportDate = dateReport.Value;
            foreach (var x in _finField)
            {
                #region num
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                    {
                        connection.Open();
                        string query =
                        string.Format(
                            @"	SELECT ff.ident as ident, ff.title as title, fd.dat_from as dat_from, fd.val as val
                            from fin_instrument fi join
	                            ffd t on t.fi_id = fi.fi_id join
                                fin_field ff on t.fif_id = ff.fif_id join
	                            data_source ds on t.ds_id = ds.ds_id join
	                            fisd_num fd on fd.fisd_id = t.fisd_id 
	                            where fi.ident = '{0}'
		                            and t.fif_id = {1}
		                            and ds.ident = '{2}'
                                    and fd.dat_from <= to_date('{3}', 'dd.mm.yyyy')",
                            _ident,
                            x.Key,
                            dataSource,
                            reportDate.ToString("dd.MM.yyyy"));
                        NpgsqlCommand command = new NpgsqlCommand(query, connection);
                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                DataRow newRow = dataTable.NewRow();
                                newRow[identColumn] = dataReader["ident"];
                                newRow[titleColumn] = dataReader["title"];
                                newRow[valueColumn] = dataReader["val"];
                                dataTable.Rows.Add(newRow);
                            }
                        }
                    }
                }
                #endregion

                #region string
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                    {
                        connection.Open();
                        string query =
                        string.Format(
                            @"	SELECT ff.ident as ident, ff.title as title, fd.dat_from as dat_from, fd.val as val
                            from fin_instrument fi join
	                            ffd t on t.fi_id = fi.fi_id join
                                fin_field ff on t.fif_id = ff.fif_id join
	                            data_source ds on t.ds_id = ds.ds_id join
	                            fisd_str fd on fd.fisd_id = t.fisd_id 
	                            where fi.ident = '{0}'
		                            and t.fif_id = {1}
		                            and ds.ident = '{2}'
                                    and fd.dat_from <= to_date('{3}', 'dd.mm.yyyy')",
                            _ident,
                            x.Key,
                            dataSource,
                            reportDate.ToString("dd.MM.yyyy"));
                        NpgsqlCommand command = new NpgsqlCommand(query, connection);
                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                DataRow newRow = dataTable.NewRow();
                                newRow[identColumn] = dataReader["ident"];
                                newRow[titleColumn] = dataReader["title"];
                                newRow[valueColumn] = dataReader["val"];
                                dataTable.Rows.Add(newRow);
                            }
                        }
                    }
                }
                #endregion

                #region dateTime
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                    {
                        connection.Open();
                        string query =
                        string.Format(
                            @"	SELECT ff.ident as ident, ff.title as title, fd.dat_from as dat_from, fd.val as val
                            from fin_instrument fi join
	                            ffd t on t.fi_id = fi.fi_id join
                                fin_field ff on t.fif_id = ff.fif_id join
	                            data_source ds on t.ds_id = ds.ds_id join
	                            fisd_date fd on fd.fisd_id = t.fisd_id 
	                            where fi.ident = '{0}'
		                            and t.fif_id = {1}
		                            and ds.ident = '{2}'
                                    and fd.dat_from <= to_date('{3}', 'dd.mm.yyyy')",
                            _ident,
                            x.Key,
                            dataSource,
                            reportDate.ToString("dd.MM.yyyy"));
                        NpgsqlCommand command = new NpgsqlCommand(query, connection);
                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                DataRow newRow = dataTable.NewRow();
                                newRow[identColumn] = dataReader["ident"];
                                newRow[titleColumn] = dataReader["title"];
                                newRow[valueColumn] = dataReader["val"];
                                dataTable.Rows.Add(newRow);
                            }
                        }
                    }
                }
                #endregion

                #region enum
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(_connection))
                    {
                        connection.Open();
                        string query =
                        string.Format(
                            @"	SELECT ff.ident as ident, ff.title as title, fd.dat_from as dat_from, di.val as val
                            from fin_instrument fi join
	                            ffd t on t.fi_id = fi.fi_id join
                                fin_field ff on t.fif_id = ff.fif_id join
	                            data_source ds on t.ds_id = ds.ds_id join
	                            fisd_item fd on fd.fisd_id = t.fisd_id join
	                            dict_item di on (ff.fif_id = di.fif_id and fd.val = di.key_v)
	                            where fi.ident = '{0}'
		                            and t.fif_id = {1}
		                            and ds.ident = '{2}'
                                    and fd.dat_from <= to_date('{3}', 'dd.mm.yyyy')",
                            _ident,
                            x.Key,
                            dataSource,
                            reportDate.ToString("dd.MM.yyyy"));
                        NpgsqlCommand command = new NpgsqlCommand(query, connection);
                        using (NpgsqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                DataRow newRow = dataTable.NewRow();
                                newRow[identColumn] = dataReader["ident"];
                                newRow[titleColumn] = dataReader["title"];
                                newRow[valueColumn] = dataReader["val"];
                                dataTable.Rows.Add(newRow);
                            }
                        }
                    }
                }
                #endregion
            }

            DataScalars.DataSource = dataTable;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateScalarTable();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            UpdateTimeSeries();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            UpdateTimeSeries();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimeSeries();
        }

        private void UpdateTimeSeries()
        {
            usedColumns.Clear();
            values = new Dictionary<string, Dictionary<DateTime, decimal>>();

            string mirConnection = _connection;
            if (comboBox2 == null ||
                comboBox2.SelectedItem == null ||
                comboBox2.Items.Count == 0 ||
                string.IsNullOrEmpty(comboBox2.SelectedItem.ToString()))
                return;

            string dataSource = comboBox2.SelectedItem.ToString();

            if (dataSource == usedDataSource &&
                usedDateFrom == dtpFrom.Value.Date &&
                usedDateTo == dtpTo.Value.Date)
                return;
            //обновляем временную серию 

            string query =
                string.Format(@"SELECT fd.dat as DAT, fd.val as VAL, ff.ident as IDENT 
                                from fin_instrument fi join
	                                ffd t on t.fi_id = fi.fi_id join
	                                fisd_dq fd on fd.fisd_id = t.fisd_id join
	                                data_source ds on ds.ds_id = t.ds_id join
	                                fin_field ff on ff.fif_id = t.fif_id
	                                and fi.ident = '{0}'
	                                and ds.ident = '{1}'
                                    and fd.dat >= to_date('{2}','dd.mm.yyyy') 
                                    and fd.dat <= to_date('{3}','dd.mm.yyyy')",
                                _ident,
                                dataSource,
                                dtpFrom.Value.ToString("dd.MM.yyyy"),
                                dtpTo.Value.ToString("dd.MM.yyyy"));

            using (NpgsqlConnection connection = new NpgsqlConnection(mirConnection))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    using (NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            DateTime date = (DateTime)dataReader["DAT"];
                            decimal val = (decimal)dataReader["VAL"];
                            string ident = (string)dataReader["IDENT"];

                            if (values.ContainsKey(ident))
                            {
                                values[ident].Add(date, val);
                            }
                            else
                            {
                                values.Add(ident, new Dictionary<DateTime, decimal>()
                                    {
                                        {date, val}
                                    });
                            }
                        }
                    }
                }
            }

            //values в нормальную форму
            if (values.Count == 0)
            {
                DataTable dataTable_ = new DataTable();
                dataGridView1.DataSource = dataTable_;
                return;
            }

            HashSet<string> columns = new HashSet<string>();
            foreach (var x in values.Keys)
            {
                columns.Add(x);
            }
            usedColumns = columns;

            DataColumn dateTimeColumn = new DataColumn("Дата", typeof(DateTime));
            List<DataColumn> dataColumns = new List<DataColumn>() { dateTimeColumn };
            foreach (var x in columns)
            {
                dataColumns.Add(new DataColumn(x, typeof(decimal)));
            }
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(dataColumns.ToArray());

            for (DateTime begin = dtpFrom.Value; begin <= dtpTo.Value; begin = begin.AddDays(1))
            {
                DataRow newRow = dataTable.NewRow();
                newRow["Дата"] = begin;
                foreach (var x in values.Keys)
                {
                    if (values[x].ContainsKey(begin))
                    {
                        newRow[x] = values[x][begin];
                    }
                }
                dataTable.Rows.Add(newRow);
            }

            dataGridView1.DataSource = dataTable;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if(columns.Contains(column.Name))
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string title = dataGridView1.Columns[e.ColumnIndex].Name;
            UpdateTimeSeries();
            if(usedColumns.Contains(title))
            {
                var tt = values[title];
                var t = new PaintWindow(tt)
                {
                    Text = _ident
                };
                t.ShowDialog();
            }
        }

        private void dtpActualDate3_ValueChanged(object sender, EventArgs e)
        {
            UpdateOfferTable();
        }

        private void cbDataSource3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOfferTable();
        }

        private void UpdateOfferTable()
        {
            Dictionary<string, Dictionary<DateTime, decimal?>> values =
                new Dictionary<string, Dictionary<DateTime, decimal?>>();

            if (cbDataSource3 == null ||
                cbDataSource3.SelectedItem == null ||
                cbDataSource3.Items.Count == 0 ||
                string.IsNullOrEmpty(cbDataSource3.SelectedItem.ToString()))
                return;

            string dataSource = cbDataSource3.SelectedItem.ToString();
            DateTime dateTime = dtpActualDate3.Value;

            string query = string.Format(
                @"select cf.val as val, cf.dat as dat, ct.ident as ident from fcs fcs join 
	            fin_instrument fi on fcs.fi_id = fi.fi_id join
	            cashflow_types ct on ct.ct_id = fcs.ct_id join
	            data_source ds on ds.ds_id = fcs.ds_id join
	            cashflow cf on cf.cf_id = fcs.cf_id 
	            where fi.ident = '{0}'
	            and cf.valid_dat <= to_date('{1}', 'dd.mm.yyyy')
	            and ds.ident = '{2}'
	            and cf.valid_dat in 
	            (
		            select max(cf_2.valid_dat) from fcs fcs_2 join 
		            fin_instrument fi_2 on fcs_2.fi_id = fi_2.fi_id join
		            cashflow_types ct_2 on ct_2.ct_id = fcs_2.ct_id join
		            data_source ds_2 on ds_2.ds_id = fcs_2.ds_id join
		            cashflow cf_2 on cf_2.cf_id = fcs_2.cf_id 
		            where fi_2.ident = fi.ident
		            and cf_2.dat = cf.dat
		            and cf_2.cf_id = cf.cf_id
		            and ds_2.ds_id = ds.ds_id
		            group by cf_2.cf_id, cf_2.dat, fcs_2.ct_id 
	            )", _ident,
                  dateTime.ToString("dd.MM.yyyy"),
                  dataSource);

            using(NpgsqlConnection connection = new NpgsqlConnection(_connection))
            {
                connection.Open();
                using(NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.CommandText = query;
                    command.Connection = connection;

                    using(NpgsqlDataReader dataReader = command.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            string ident = dataReader["ident"].ToString();
                            decimal? val = null;
                            if (!string.IsNullOrEmpty(dataReader["val"].ToString()))
                                val = Convert.ToDecimal(dataReader["val"]);
                            DateTime date = (DateTime)dataReader["dat"];

                            if(values.ContainsKey(ident))
                            {
                                values[ident].Add(date, val);
                            }
                            else
                            {
                                values.Add(ident, new Dictionary<DateTime, decimal?>() { {date, val} });
                            }
                        }
                    }
                }
            }

            if(values.Count() == 0)
            {
                dataGridView2.ClearSelection();
                return;
            }

            DataTable dataTable = new DataTable();
            SortedList<DateTime, DateTime> dates = new SortedList<DateTime, DateTime>();
            foreach(var x in values)
            {
                foreach(var y in x.Value)
                {
                    if(!dates.ContainsKey(y.Key))
                    {
                        dates.Add(y.Key, y.Key);
                    }
                }
            }

            List<string> columns = new List<string>();
            foreach(var x in values)
            {
                columns.Add(x.Key);
            }

            DataColumn time = new DataColumn("Дата", typeof(string));
            List<DataColumn> dataColumns = new List<DataColumn>() { time };
            foreach(var x in columns)
            {
                DataColumn dataColumn = new DataColumn(x, typeof(string));
                dataColumns.Add(dataColumn);
            }

            dataTable.Columns.AddRange(dataColumns.ToArray());

            foreach(var innerDate in dates)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["Дата"] = innerDate.Key.ToShortDateString();
                foreach(var column in columns)
                {
                    if(values[column].ContainsKey(innerDate.Key))
                    {
                        decimal? val = values[column][innerDate.Key];
                        if (val == null)
                            dataRow[column] = "пусто";
                        else
                            dataRow[column] = val.Value.ToString();
                    }
                }
                dataTable.Rows.Add(dataRow);
            }

            dataGridView2.DataSource = dataTable;
        }
    }
}
