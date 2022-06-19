using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoatRental
{
    class Rental
    {
        private DateTime rentStart, rentEnd;
        private double fuelUsed;
        private int boatNumber;
        private Boolean damaged;

        public Rental(int boatNumber, DateTime rentStart, DateTime rentEnd, double fuelUsed, Boolean damaged)
        {
            this.boatNumber = boatNumber;
            this.rentStart = rentStart;
            this.rentEnd = rentEnd;
            this.fuelUsed = fuelUsed;
            this.damaged = damaged;
        }

        public DateTime RentStart 
        { 
            get
            { 
                return rentStart; 
            }
            set
            { 
                rentStart = value;
            }
        }

        public DateTime RentEnd 
        { 
            get
            {
                return rentEnd; 
            }
            set
            {
                rentEnd = value; 
            }
        }

        public double FuelUsed
        {
            get 
            {
                return fuelUsed; 
            }
            set
            {
                fuelUsed = value;
            }
        }
        public int BoatNumber
        { 
            get
            {
                return boatNumber;
            }
            set
            {
                boatNumber = value;
            }
        }
        public Boolean Damaged 
        { 
            get
            {
                return damaged;
            }
            set
            {
                damaged = value;
            }
        }
    }
}
