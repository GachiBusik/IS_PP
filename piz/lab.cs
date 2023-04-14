using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using IronBarCode;
using Aspose.Pdf.Operators;
using Image = iTextSharp.text.Image;

namespace piz
{
    enum RowState
    {
        Elisted,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class lab : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;
        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetString(3),  record.GetString(4), record.GetString(5), record.GetString(6));
        }
        private void CreateColums()
        {
            dataGridView1.Columns.Add("id_client", "id_client");
            dataGridView1.Columns.Add("FIO", "FIO");
            dataGridView1.Columns.Add("PhoneNumber", "PhoneNumber");
            dataGridView1.Columns.Add("Mail", "Mail");
            dataGridView1.Columns.Add("Seria", "Seria");
            dataGridView1.Columns.Add("Nomer", "Nomer");
            dataGridView1.Columns.Add("Prob", "Prob");
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from dbo.Client";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Search()
        {
            dataGridView1.Show();

            dataGridView1.Rows.Clear();

            string searchString = $"select * from dbo.Client where concat (FIO, PhoneNumber, Mail, Seria, Nomer, Prob) like '%" + textBox1.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());
            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dataGridView1, read);
            }
            read.Close();

            //textBox1.Text = $"SELECT = prob.nom FROM dbo.prob WHERE JOIN dbo.p ON prob.id_prob = p.id";
        }


        public lab()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CreateColums();
            RefreshDataGrid(dataGridView1);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = Convert.ToString(textBox1.Text);

            if (str.Length != 15)
            {
                label11.Visible = true;
                label11.Text = "Неверный код";
            }

            label11.Visible = false;

            Search();
            IronBarCode.License.LicenseKey = "IRONBARCODE.DANILATYULKIN.31314-2EB1868BB7-HNXEDO-XY3OIEHH2HKQ-ZWH4OJPEI3DT-IRDOVO4JGNBN-PYDX6ZL4O4ZJ-QGTGJY2PJLZU-R7Q6AV-TGG77DI2HF6JUA-DEPLOYMENT.TRIAL-BOXF47.TRIAL.EXPIRES.12.MAY.2023";

            GeneratedBarcode myBarcode = IronBarCode.BarcodeWriter.CreateBarcode(str, BarcodeWriterEncoding.Code128);

            myBarcode.AddAnnotationTextAboveBarcode("Номер пробирки:");
            myBarcode.AddBarcodeValueTextBelowBarcode(str);

            myBarcode.SaveAsPng("barcode.png");
            myBarcode.SaveAsPdf("barcode.pdf");

            System.Drawing.Image image = System.Drawing.Image.FromFile("barcode.png");
            pictureBox1.Image = image;
        }

        private void ExportToPdf(DataGridView dataGridView)
        {
            // Создаем диалог сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Save PDF File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                Document document = new Document(PageSize.A4);

                try
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    document.Open();

                    // Создание таблицы
                    PdfPTable pdfTable = new PdfPTable(dataGridView.ColumnCount);

                    // Заполнение таблицы данными из DataGridView
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                        pdfTable.AddCell(cell);
                    }

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            pdfTable.AddCell(cell.Value.ToString());
                        }
                    }

                    // Добавление таблицы на страницу документа
                    document.Add(pdfTable);

                    // Загрузка изображения
                    Image barcode = Image.GetInstance("barcode.png");

                    // Размещение изображения на странице документа
                    barcode.SetAbsolutePosition(PageSize.A4.Width - 100, PageSize.A4.Height - 100);
                    document.Add(barcode);

                    document.Close();

                    MessageBox.Show("Table exported to PDF.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting table to PDF: " + ex.Message);
                }
            }
        }
        private void ExportPNGToPDF()
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExportToPdf(dataGridView1);
            
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox2.Enabled = false;


                textBox2.Text = row.Cells[0].Value.ToString();
                textBox5.Text = row.Cells[1].Value.ToString();
                textBox6.Text = row.Cells[2].Value.ToString();
                textBox7.Text = row.Cells[3].Value.ToString();
                textBox8.Text = row.Cells[4].Value.ToString();
                textBox9.Text = row.Cells[5].Value.ToString();
                textBox10.Text = row.Cells[6].Value.ToString();
            }

        }
        private void Save()
        {
            dataBase.getConnection();

            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                    var id_p = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var F = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var I = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var O = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var Ser = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var Nom = dataGridView1.Rows[index].Cells[5].Value.ToString();
                    var Phone = dataGridView1.Rows[index].Cells[6].Value.ToString();

                    var changeQuery = $"update dbo.Client set FIO = '{F}', PhoneNumber = '{I}', Mail = '{O}', Seria = '{Ser}', Nomer = '{Nom}', Prob = '{Phone}' where id_client = '{id_p}'";

                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
            }
            dataBase.closeConnection();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;

            var id_p = textBox2.Text;
            var F = textBox5.Text;
            var I = textBox6.Text;
            var O = textBox7.Text;
            var Ser = textBox8.Text;
            var Nom = textBox9.Text;
            var Nomst = textBox10.Text;

            dataGridView1.Rows[selectedRowIndex].SetValues(id_p, F, I, O, Ser, Nom, Nomst);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
