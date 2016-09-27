﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Traceability_Labels
{
    public partial class EditarProduto : Form
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataAdapter adapter;
        DataSet dataSet;
        DataTable table;
        SqlDataReader reader;
        bool flag;

        public EditarProduto()
        {
            InitializeComponent();

            dataSet = new DataSet();
            connection = new SqlConnection(Global.connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar com o servidor: " + ex.Message, "ERRO!!!");
            }
            finally
            {
                connection.Close();
            }
            flag = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void DeletarProduto_Load(object sender, EventArgs e)
        {
            UpdateBox();
            flag = true;
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(this,"Confirme se você quer mesmo deletar este produto.", "ATENÇÃO", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if(confirm == DialogResult.Yes)
            {
                try
                {
                    command = new SqlCommand("delete from produto where id=@id", connection);
                    command.Parameters.AddWithValue("@id", Convert.ToInt32(cbox_Produtos.SelectedValue));
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Produto deletado com sucesso!");
                    Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private void UpdateBox()
        {
            try
            {
                cbox_Produtos.Items.Clear();
                command = new SqlCommand("select id,nome from produto", connection);
                adapter = new SqlDataAdapter(command);
                table = new DataTable();
                adapter.Fill(table);
                cbox_Produtos.DataSource = table;
                cbox_Produtos.DisplayMember = table.Columns[1].ToString();
                cbox_Produtos.ValueMember = table.Columns[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "ERRO!");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void cbox_Produtos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (flag == true)
            {
                try
                {
                    command = new SqlCommand("select nome,gtin,embalagem,caixa from produto where id=@id", connection);
                    command.Parameters.AddWithValue("@id", Convert.ToInt32(cbox_Produtos.SelectedValue));
                    connection.Open();
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        txt_Nome.Text = reader.GetString(0);
                        txt_Gtin.Text = reader.GetString(1);
                        txt_Embalagem.Text = reader.GetDecimal(2).ToString();
                        txt_Caixa.Text = reader.GetDecimal(3).ToString();
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                double embalagem, caixa;
                bool testEmbalagem = double.TryParse(txt_Embalagem.Text, out embalagem);
                if (!testEmbalagem)
                    throw new Exception("A tara da embalagem esta em um formato incorreto!");
                bool testCaixa = double.TryParse(txt_Caixa.Text, out caixa);
                if (!testCaixa)
                    throw new Exception("A tara da caixa esta em um formato incorreto!");

                command = new SqlCommand("update produto set nome=@nome,gtin=@gtin,embalagem=@embalagem,caixa=@caixa where id=@id", connection);
                command.Parameters.AddWithValue("@nome", txt_Nome.Text);
                command.Parameters.AddWithValue("@gtin", txt_Gtin.Text);
                command.Parameters.AddWithValue("@embalagem", txt_Embalagem.Text);
                command.Parameters.AddWithValue("@caixa", txt_Caixa.Text);
                command.Parameters.AddWithValue("@id", Convert.ToInt32(cbox_Produtos.SelectedValue));
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Produto editado com sucesso!");
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message, "ERRO!!!");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
    }
}
