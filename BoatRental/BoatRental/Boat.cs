using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoatRental
{
    class Boat
    {
        private double weight, horsepower, length, pricePerHour;
        private int number;
        private TimeSpan totalRentTime = TimeSpan.Zero;
        public Boat(string weight, string horsepower, string length, string number, string pricePerHour)
        {
            this.weight = Double.Parse(weight);
            this.horsepower = Double.Parse(horsepower);
            this.length = Double.Parse(length);
            this.number = int.Parse(number);
            this.pricePerHour = Double.Parse(pricePerHour);
        }

        public Boat(string weight, string horsepower, string length, string number, string pricePerHour, TimeSpan totalRentTime)
        {
            this.weight = Double.Parse(weight);
            this.horsepower = Double.Parse(horsepower);
            this.length = Double.Parse(length);
            this.number = int.Parse(number);
            this.pricePerHour = Double.Parse(pricePerHour);
            this.totalRentTime = totalRentTime;
        }
        public double Weight 
        { 
            get { return weight; }
            set { weight = value; } 
        }
        public double Horsepower 
        {
            get { return horsepower; }
            set { horsepower = value; }
        }
        public double Length 
        {
            get { return length; } 
            set { length = value; }
        }
        public int Number 
        { 
            get { return number; } 
            set { number = value; }
        }
        public double PricePerHour 
        { 
            get { return pricePerHour; }
            set { pricePerHour = value; }
        }

        public TimeSpan TotalRentTime
        {
            get { return totalRentTime; }
            set { totalRentTime = value; }
        }
    }
}
