using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoatRental
{
    public partial class Form1 : Form
    {
        const string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = E:\programe\Vstudio repo\Period-4\BoatRental\BoatRental\Boat_rental.mdf; Integrated Security = True";

        Overview overview = new Overview();
        public Form1()
        {
            Thread thread = new Thread(new ThreadStart(StartForm));
            thread.Start();
            Thread.Sleep(4500);
            InitializeComponent();
            thread.Abort();

        }

        public void StartForm()
        {
            Application.Run(new SplashScreen());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length != 0 && textBox2.Text.Length != 0 && textBox3.Text.Length != 0 &&
                textBox4.Text.Length != 0 && textBox5.Text.Length != 0)
            {
                try
                {
                    Boat addedBoat = new Boat(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
                    using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            sqlConnection.Open();

                            string sql = "Insert into Boats (number, weight, horsepower, length, pricePerHour)" +
                                "values(@number, @weight, @horsepower, @length, @pricePerHour)";

                            using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                            {
                                try
                                {
                                    sqlCommand.Parameters.Add("@number", SqlDbType.Int).Value = addedBoat.Number;
                                    sqlCommand.Parameters.Add("@weight", SqlDbType.Float).Value = addedBoat.Weight;
                                    sqlCommand.Parameters.Add("@horsepower", SqlDbType.Float).Value = addedBoat.Horsepower;
                                    sqlCommand.Parameters.Add("@length", SqlDbType.Float).Value = addedBoat.Length;
                                    sqlCommand.Parameters.Add("@pricePerHour", SqlDbType.Float).Value = addedBoat.PricePerHour;

                                    int rowsAdded = sqlCommand.ExecuteNonQuery();
                                    if (rowsAdded > 0)
                                    {
                                        MessageBox.Show("Boat added!");
                                    }
                                }
                                catch (SqlException)
                                {
                                    string message = "Cannot add a boat with the same number!";
                                    string title = "Error";
                                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                finally
                                {
                                    sqlCommand.Dispose();
                                    sqlConnection.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                catch (FormatException)
                {
                    string message = "Please make sure all data is entered correctly!";
                    string title = "Error";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                string message = "Please fill in all the fields!";
                string title = "Error";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Boolean hasRows;

            if (textBox10.Text.Length != 0 && textBox7.Text.Length != 0 && textBox8.Text.Length != 0
                && textBox6.Text.Length != 0)
            {
                try
                {
                    if (DateTime.Compare(DateTime.Parse(textBox7.Text), DateTime.Parse(textBox8.Text)) <= 0)
                    {
                        if (overview.BoatAvailable(int.Parse(textBox10.Text), DateTime.Parse(textBox7.Text)))
                        {
                            Rental rental = new Rental(int.Parse(textBox10.Text), DateTime.Parse(textBox7.Text),
                            DateTime.Parse(textBox8.Text), double.Parse(textBox6.Text), checkBox1.Checked);

                            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                            {
                                try
                                {
                                    sqlConnection.Open();

                                    string sqlRead = "Select number, weight, horsepower, length, pricePerHour From dbo.Boats Where number = @boatNumber";

                                    using (SqlCommand sqlCommandRead = new SqlCommand(sqlRead, sqlConnection))
                                    {
                                        sqlCommandRead.Parameters.Add("@boatNumber", SqlDbType.Int).Value = rental.BoatNumber;

                                        using (SqlDataReader reader = sqlCommandRead.ExecuteReader())
                                        {
                                            hasRows = reader.HasRows;
                                            reader.Close();
                                        }
                                        sqlCommandRead.Dispose();
                                    }

                                    if (hasRows)
                                    {
                                        string sqlWrite = "Insert into Rentals (rentStart, rentEnd, fuelUsed, boatNumber, damaged)" +
                                            " values(@rentStart, @rentEnd, @fuelUsed, @boatNumber, @damaged)";

                                        using (SqlCommand sqlCommandWrite = new SqlCommand(sqlWrite, sqlConnection))
                                        {
                                            sqlCommandWrite.Parameters.Add("@rentStart", SqlDbType.VarChar).Value = rental.RentStart.ToString();
                                            sqlCommandWrite.Parameters.Add("@rentEnd", SqlDbType.VarChar).Value = rental.RentEnd.ToString();
                                            sqlCommandWrite.Parameters.Add("@fuelUsed", SqlDbType.Float).Value = rental.FuelUsed;
                                            sqlCommandWrite.Parameters.Add("@boatNumber", SqlDbType.Int).Value = rental.BoatNumber;
                                            sqlCommandWrite.Parameters.Add("@damaged", SqlDbType.VarChar).Value = rental.Damaged.ToString();

                                            int rowsAdded = sqlCommandWrite.ExecuteNonQuery();
                                            if (rowsAdded > 0)
                                            {
                                                MessageBox.Show("Rental added!");
                                            }

                                            sqlCommandWrite.Dispose();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Boat doesn't exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    sqlConnection.Close();
                                }
                            }
                        }
                        else
                        {
                            string message = "Boat is not available for rental on this date";
                            string title = "Error";
                            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        string message = "Can't enter a start date later than the end date!";
                        string title = "Error";
                        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Make sure data entered is in a correct format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
            else if (FormWindowState.Normal == this.WindowState)
            { notifyIcon1.Visible = false; }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.Show();
        }

        private void boatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            tabControl1.SelectedIndex = 0;
        }

        private void rentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            tabControl1.SelectedIndex = 1;
        }

        private void overviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            tabControl1.SelectedIndex = 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double turnover = overview.GetTurnover();

            string title = "Turnover";
            MessageBox.Show(turnover.ToString() + "€", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string rentalTime = overview.GetRentalTime();

            string title = "Rental time";
            MessageBox.Show(rentalTime, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string message = overview.GetBoatWithHighestConsumption();
            string title = "Most consuming boat";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string message = overview.GetPercentageOfDamagedBoats();
            string title = "Damaged boats";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string message = overview.GetShortestBoatRented();
            string title = "Least rented boat";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
