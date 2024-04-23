using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace uzemanyag_elszamolas
{
    class dbClass
    {
        private string dbhost;
        private string dbuser;
        private string dbpass;
        private string dbname;
        public MySqlConnection conn;
        public MySqlDataReader results;
        public int AffecterdRows;
        public string msg;
        public bool debug = false;

        public dbClass(string dbhost, string dbuser, string dbpass, string dbname)
        {
            this.dbhost = dbhost;
            this.dbuser = dbuser;
            this.dbpass = dbpass;
            this.dbname = dbname;

            string connectionString = 
                "server="+this.dbhost+
                ";uid="+this.dbuser+
                ";pwd="+this.dbpass+
                ";database="+this.dbname;
            this.conn = new MySqlConnection(connectionString);
        }

        // connect
        // disconnect

        // insert
        public void insert(string table, string[] fields, string[] values)
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                string fieldsList = string.Join(", ", fields);
                string valuesList = string.Join("', '", values);
                string sql = $"INSERT INTO {table} ({fieldsList}) VALUES('{valuesList}')";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.AffecterdRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        // select
        public void select(string table, string field, string value, string op = "eq")
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                op = this.getOp(op);
                if (op == "like")
                {
                    value = "%" + value + "%";
                }
                string sql = $"SELECT * FROM {table} WHERE {field} {op} '{value}'";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.results = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        // selectAll
        public void selectAll(string table)
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                string sql = $"SELECT * FROM {table}";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.results = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        // deletAll
        public void deletAll(string table)
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                string sql = $"DELETE FROM {table}";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.AffecterdRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        // delete
        public void delete(string table, string field, string value, string op = "eq")
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                op = this.getOp(op);
                if (op == "like")
                {
                    value = "%" + value + "%";
                }
                string sql = $"DELETE FROM {table} WHERE {field} {op} '{value}'";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.results = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        // update
        public void update(string table, string field, string value, string[] fields, string[] values, string op = "eq")
        {
            try
            {
                this.conn.Close();
                this.conn.Open();
                string newdata = "";

                for (int i = 0; i < fields.Length; i++)
                {
                    newdata += fields[i] + "='" + values[i] + "',";
                }

                op = this.getOp(op);
                if (op == "like")
                {
                    value = "%" + value + "%";
                }
                newdata = newdata.TrimEnd(',');
                string sql = $"UPDATE {table} SET {newdata} WHERE {field} {op} '{value}'";
                MySqlCommand cmd = new MySqlCommand(sql, this.conn);
                this.results = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                this.AffecterdRows = -1;
                if (this.debug)
                {
                    this.msg = ex.Message;
                }
            }
        }

        private string getOp(string value)
        {
            //eq, lt, gt, lte, gte, nt, lk
            string op = "";
            switch (value)
            {
                case "eq": { op = "="; break; }
                case "lt": { op = "<"; break; }
                case "gt": { op = ">"; break; }
                case "lte": { op = "<="; break; }
                case "gte": { op = ">="; break; }
                case "nt": { op = "<>"; break; }
                case "lk": { op = "like"; break; }
            }
            return op;
        }
    }
}
