using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            // Your code goes here .....
            // Re-write this method to be more efficient instead of a assigning all customers to the same teller
            Queue<Teller> TellerQueue = new Queue<Teller>();
            Queue<Customer> CustomerQueue = new Queue<Customer>();
            tellers.Teller = tellers.Teller.OrderBy(t => t.multiplier).ThenBy(t => t.specialtyType).ToList();

            customers.Customer = customers.Customer.OrderBy(c => Int32.Parse(c.duration)).ThenBy(c => c.type).ToList();

            foreach (Customer customer in customers.Customer)
            {
                CustomerQueue.Enqueue(customer);
            }

            foreach (Teller teller in tellers.Teller)
            {
                TellerQueue.Enqueue(teller);
            }

            foreach (Customer customer in customers.Customer)
            {

                foreach (Teller teller in tellers.Teller)
                {
                    while (TellerQueue.Count > 0 && CustomerQueue.Count > 0)
                    {
                        if (customer.type == teller.specialtyType)
                        {
    
                            var appointment = new Appointment(customer, teller);
                            appointmentList.Add(appointment);
                        }
                        else
                        {
                            var appointment = new Appointment(customer, teller);
                            appointmentList.Add(appointment);
                        }
                        TellerQueue.Dequeue();
                        CustomerQueue.Dequeue();
                    }
                    TellerQueue.Enqueue(teller);

                }

            }

        }
        static void OutputTotalLengthToConsole()
        {
            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalDuration = tellerGroup.Sum(x => x.duration),
                };
            var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
            Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
        }

    }
}
