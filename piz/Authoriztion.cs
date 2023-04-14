using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace piz
{
    public partial class Authoriztion : Form
    {

        DataBase dataBase = new DataBase();
        private string text = String.Empty;

        public Authoriztion()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox3.Enabled == true) && (textBox3.Text == this.text))
            {
                MessageBox.Show("Неправильная капча");
                pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
            }
            else
            {
                var loginUser = textBox1.Text;
                var passUser = textBox2.Text;
                lab frm1 = new lab();
                bu frm12 = new bu();

                SqlDataAdapter adapter = new SqlDataAdapter();

                DataTable table = new DataTable();

                string querystring = $"select id_user, login_user, password_user from register where login_user = '{loginUser}' and password_user = '{passUser}'";

                SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

                adapter.SelectCommand = command;
                adapter.Fill(table);

                if (table.Rows.Count == 1)
                {
                    string user = Convert.ToString(table.Rows[0].ItemArray[1]);

                    MessageBox.Show("Успешный вход", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (user == "labor")
                    {
                        this.Hide();
                        frm1.Show();
                    }
                    else if (user == "Bugalt")
                    {
                        this.Hide();
                        frm12.Show();
                    }
                    else if(user == "labor_is")
                    {
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Нет такого аккаунта!", "повтори", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Enabled = true;
                    textBox3.Show();
                    label3.Show();
                    label4.Show();
                    pictureBox1.Image = this.CreateImage(pictureBox1.Width, pictureBox1.Height);
                }
            }

        }

        private Bitmap CreateImage(int Width, int Height)
        {
            Random rnd = new Random();

            Bitmap result = new Bitmap(Width, Height);

            int Xpos = rnd.Next(0, Width - 50);
            int Ypos = rnd.Next(15, Height - 15);

            Brush[] colors = { Brushes.Black,
                     Brushes.Red,
                     Brushes.RoyalBlue,
                     Brushes.Green };

            Graphics g = Graphics.FromImage((Image)result);

            g.Clear(Color.Gray);


            text = String.Empty;
            string ALF = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < 5; ++i)
                text += ALF[rnd.Next(ALF.Length)];


            g.DrawString(text,
                         new Font("Arial", 15),
                         colors[rnd.Next(colors.Length)],
                         new PointF(Xpos, Ypos));
            g.DrawLine(Pens.Black,
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));
            g.DrawLine(Pens.Black,
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, Color.White);

            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox3.Enabled = false;
            label4.Hide();
            textBox3.Hide();
            label3.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.UseSystemPasswordChar == true)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }
        
    }
}
