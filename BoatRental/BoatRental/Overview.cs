using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoatRental
{
    internal class Overview
    {
        const string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = E:\programe\Vstudio repo\Period-4\BoatRental\BoatRental\Boat_rental.mdf; Integrated Security = True";

        private List<Boat> boats = new List<Boat>();
        private List<Rental> rentals = new List<Rental>();

        public Overview()
        { }

        public List<Boat> Boats 
        {
            get
            { 
                return boats;
            }
            set 
            {
                boats = value;
            }
        }
        public List<Rental> Rentals 
        {
            get
            {
                return rentals;
            }
            set
            { 
                rentals = value; 
            }
        }

        public Boolean BoatAvailable(int boatNumber, DateTime rentStart)
        {
            Boolean available = true;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals Where boatNumber = @boatNumber";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        sqlCommand.Parameters.Add("@boatNumber", SqlDbType.Int).Value = boatNumber;
                        sqlCommand.Parameters.Add("@rentStart", SqlDbType.VarChar).Value = rentStart.ToString();

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                DateTime dbRentStart = DateTime.Parse(reader["rentStart"].ToString());
                                if(DateTime.Compare(dbRentStart.Date, rentStart.Date) == 0)
                                {
                                    available = false;
                                }
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                    return available;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return available;
        }

        public double GetTurnover()
        {
            double turnover = 0;
            List<Boat> list = new List<Boat>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Boats";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                string weight = reader["weight"].ToString();
                                string horsepower = reader["horsepower"].ToString();
                                string length = reader["length"].ToString();
                                string number = reader["number"].ToString();
                                string pricePerHour = reader["pricePerHour"].ToString();

                                Boat boat = new Boat(weight, horsepower, length, number, pricePerHour);
                                list.Add(boat);
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                DateTime rentStart = DateTime.Parse(reader["rentStart"].ToString());
                                DateTime rentEnd = DateTime.Parse(reader["rentEnd"].ToString());
                                double fuelUsed = double.Parse(reader["fuelUsed"].ToString());
                                int boatNumber = int.Parse(reader["boatNumber"].ToString());

                                double secondsRented = rentEnd.Subtract(rentStart).TotalSeconds;

                                foreach(Boat boat in list)
                                {
                                    if(boat.Number == boatNumber)
                                    {
                                        turnover += (secondsRented / 3600.0 * boat.PricePerHour) + (fuelUsed * 5.0);
                                        break;
                                    }
                                }
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message); 
                }
                finally 
                { 
                    sqlConnection.Close(); 
                }
            }
            return turnover;
        }

        public string GetRentalTime()
        {
            TimeSpan rentalTime = TimeSpan.Zero;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                DateTime rentStart = DateTime.Parse(reader["rentStart"].ToString());
                                DateTime rentEnd = DateTime.Parse(reader["rentEnd"].ToString());

                                TimeSpan duration = rentEnd.Subtract(rentStart);
                                rentalTime += duration;
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally 
                { 
                    sqlConnection.Close(); 
                }
                return rentalTime.ToString();
            }
        }

        public string GetBoatWithHighestConsumption()
        {
            string boatWithHighestConsumption = "Not available";
            double highestConsumption = 0;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                DateTime rentStart = DateTime.Parse(reader["rentStart"].ToString());
                                DateTime rentEnd = DateTime.Parse(reader["rentEnd"].ToString());
                                double fuelUsed = double.Parse(reader["fuelUsed"].ToString());
                                int boatNumber = int.Parse(reader["boatNumber"].ToString());

                                double durationInMinutes = rentEnd.Subtract(rentStart).TotalMinutes;
                                double fuelUsedPerMinute = fuelUsed / durationInMinutes;
                                if(highestConsumption < fuelUsedPerMinute)
                                {
                                    highestConsumption = fuelUsedPerMinute;
                                    boatWithHighestConsumption = "Boat " + boatNumber.ToString() + " has the highest " +
                                        "consumption of:" + Math.Round(highestConsumption, 2).ToString() + " litres per minute";
                                }
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally 
                {
                    sqlConnection.Close();
                }
                return boatWithHighestConsumption;
            }
        }

        public string GetPercentageOfDamagedBoats()
        {
            double numberOfBoats = 0;
            double numberOfBoatsDamaged = 0;
            List<int> boatsChecked = new List<int>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Boats";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                numberOfBoats++;
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally 
                { 
                    sqlConnection.Close(); 
                }
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int boatNumber = int.Parse(reader["boatNumber"].ToString());
                                string damaged = reader["damaged"].ToString();

                                if(damaged == "True" && !boatsChecked.Contains(boatNumber))
                                {
                                    numberOfBoatsDamaged++;
                                    boatsChecked.Add(boatNumber);
                                }
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }

                double percentageOfDamagedBoats = (numberOfBoatsDamaged / numberOfBoats) * 100;
                return "Percentage of damaged boats is: " + percentageOfDamagedBoats.ToString() + "%";
            }
        }

        public string GetShortestBoatRented()
        {
            TimeSpan shortestRent = TimeSpan.MaxValue;
            int shortestBoatRented = 0;
            List<Boat> boatList = new List<Boat>();
            List<Rental> rentalList = new List<Rental>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Boats";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string weight = reader["weight"].ToString();
                                string horsepower = reader["horsepower"].ToString();
                                string length = reader["length"].ToString();
                                string number = reader["number"].ToString();
                                string pricePerHour = reader["pricePerHour"].ToString();

                                Boat boat = new Boat(weight, horsepower, length, number, pricePerHour);
                                boatList.Add(boat);
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string sql = "Select * From Rentals";

                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                DateTime rentStart = DateTime.Parse(reader["rentStart"].ToString());
                                DateTime rentEnd = DateTime.Parse(reader["rentEnd"].ToString());
                                double fuelUsed = double.Parse(reader["fuelUsed"].ToString());
                                int boatNumber = int.Parse(reader["boatNumber"].ToString());
                                Boolean damaged = Boolean.Parse(reader["damaged"].ToString());

                                Rental rental = new Rental(boatNumber, rentStart, rentEnd, fuelUsed, damaged);
                                rentalList.Add(rental);
                            }
                            reader.Close();
                        }
                        sqlCommand.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }

                foreach(Rental rental in rentalList)
                {
                    TimeSpan rentalTime = rental.RentEnd.Subtract(rental.RentStart);

                    foreach(Boat boat in boatList)
                    {
                        if(rental.BoatNumber == boat.Number)
                        {
                            boat.TotalRentTime += rentalTime;
                        }
                    }
                }

                foreach(Boat boat in boatList)
                {
                    if(shortestRent > boat.TotalRentTime)
                    {
                        shortestRent = boat.TotalRentTime;
                        shortestBoatRented = boat.Number;
                    }
                }

                return "The boat rented out for the shortest time is: "  + shortestBoatRented.ToString();
            }
        }
    }
}
