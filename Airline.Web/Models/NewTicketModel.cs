using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Models
{
    public class NewTicketModel
    {

        public string UserEmail { get; set; }

        public string FullName { get; set; }

        // ========== Flight OneWay =====================
        public int FlightId { get; set; }
        public string ClassName { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string Seat { get; set; }

        // ========== Flight OneWay =====================
        public int FlightIdReturn { get; set; }

        public string ClassNameReturn { get; set; }

        public string FromReturn { get; set; }

        public string ToReturn { get; set; }

        public string DateReturn { get; set; }

        public string TimeReturn { get; set; }

        public string SeatReturn { get; set; }
    }
}
