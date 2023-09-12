using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hash_Cracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        string[] wordlist;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Gray;
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            label2.ForeColor = Color.White;
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Gray;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.ForeColor = Color.White;
        }
        Point lastPoint;
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter file = new StreamWriter(saveFileDialog1.FileName))
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[2].Value.ToString() != "")
                        {
                            file.WriteLine(
                                dataGridView1.Rows[i].Cells[1].Value.ToString() + ":" +
                                dataGridView1.Rows[i].Cells[2].Value.ToString()
                            );
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] list = File.ReadAllLines(openFileDialog1.FileName);

                foreach (var hash in list)
                {
                    if (Regex.IsMatch(hash, "^[0-9a-fA-F]{32}$"))
                    {
                        dataGridView1.Rows.Add("MD5", hash, "");
                    }
                    else if (Regex.IsMatch(hash, "^[0-9a-fA-F]{40}$"))
                    {
                        dataGridView1.Rows.Add("SHA1", hash, "");
                    }
                    else if (Regex.IsMatch(hash, "^[0-9a-fA-F]{64}$"))
                    {
                        dataGridView1.Rows.Add("SHA256", hash, "");
                    }
                    else if (Regex.IsMatch(hash, "^[0-9a-fA-F]{96}$"))
                    {
                        dataGridView1.Rows.Add("SHA384", hash, "");
                    }
                    else if (Regex.IsMatch(hash, "^[0-9a-fA-F]{128}$"))
                    {
                        dataGridView1.Rows.Add("SHA512", hash, "");
                    }
                    else
                    {
                        dataGridView1.Rows.Add("Bilinmiyor", hash, "");
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                wordlist = File.ReadAllLines(openFileDialog1.FileName);
                MessageBox.Show("wordlist eklendi!");
            }
        }

        bool rook = false;

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Hash listesi eklemedin!");
                return;
            }
            else if (wordlist == null)
            {
                MessageBox.Show("Wordlist eklemedin!");
                return;
            }
            else if (button3.Text == "Durdur")
            {
                rook = true;
                return;
            }

            button3.ForeColor = Color.DarkOrange;
            button3.Text = "Durdur";

            Task task1 = Task.Run(hCrack);
            Task task2 = task1.ContinueWith(previousTask =>
            {
                button3.Text = "Baþlat";
                button3.ForeColor = Color.White;
                MessageBox.Show($"{crackd} adet hash kýrýldý.");
            });
        }
        int crackd = 0;
        private void hCrack()
        {
            Parallel.ForEach(wordlist, word =>
            {
                if (rook)
                    return;

                string[] hashes = {
                    BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(word))).Replace("-", ""),
                    BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(word))).Replace("-", ""),
                    BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(word))).Replace("-", ""),
                    BitConverter.ToString(SHA384.Create().ComputeHash(Encoding.ASCII.GetBytes(word))).Replace("-", ""),
                    BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.ASCII.GetBytes(word))).Replace("-", "")
                };

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    foreach (var hash in hashes)
                    {
                        if (dataGridView1.Rows[i].Cells[1].Value.ToString() == hash.ToLower() & dataGridView1.Rows[i].Cells[2].Value.ToString() == "")
                        {
                            dataGridView1.Rows[i].Cells[2].Value = word;
                            crackd++;
                            break;
                        }
                    }

                }

            });

        }

    }
}