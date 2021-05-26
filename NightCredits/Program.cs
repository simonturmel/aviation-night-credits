﻿using Newtonsoft.Json;
using NightCredits.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace NightCredits
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                PrintSeparator();
                Run();
            }
            while (Continue());
        }

        private static void PrintSeparator()
        {
            Console.WriteLine(new string('=', 30));
        }

        private static bool Continue()
        {
            Console.Write("Continue (Y/N)?");
            var continueInput = Console.ReadLine();
            Console.WriteLine();

            return continueInput.ToUpper().Equals("Y");
        }

        private static void Run()
        {
            var airportCode = ReadAirportCode();
            var dateTimeInput = ReadTime(ReadDate());

            var airport = GetAirport(airportCode);

            var previousDay = GetDateSunriseSunsetAtAirport(dateTimeInput.AddDays(-1), airport);
            var specifiedDay = GetDateSunriseSunsetAtAirport(dateTimeInput, airport);
            var nextDay = GetDateSunriseSunsetAtAirport(dateTimeInput.AddDays(1), airport);

            Console.WriteLine();
            Console.WriteLine(GetCredit(dateTimeInput, previousDay, specifiedDay, nextDay));
#if DEBUG
            LogDatesStack(dateTimeInput, airport, previousDay, specifiedDay, nextDay);
#endif
            Console.WriteLine();
        }

        private static string GetCredit(DateTime dateToEvaluate, SunriseSunsetModel previousDay, SunriseSunsetModel specifiedDay, SunriseSunsetModel nextDay)
        {
            var credit = "This event should be credited for DAY";

            if (dateToEvaluate >= previousDay.TwilightSunset && dateToEvaluate <= specifiedDay.TwilightSunrise ||
                dateToEvaluate >= specifiedDay.TwilightSunset && dateToEvaluate <= nextDay.TwilightSunrise)
            {
                credit = "This event should be credited for NIGHT";
            }

            return credit;
        }

        private static void LogDatesStack(DateTime dateToEvaluate, AirportModel airport, SunriseSunsetModel previousDay, SunriseSunsetModel specifiedDay, SunriseSunsetModel nextDay)
        {
            var events = new List<EventModel>()
            {

                new EventModel { DateTime = previousDay.TwilightSunrise, Label = "Day-1 Twilight Sunrise", Priority = 1 },
                new EventModel { DateTime = previousDay.TwilightSunset, Label = "Day-1 Twilight Sunset", Priority = -1 },
                new EventModel { DateTime = specifiedDay.TwilightSunrise, Label = "Day-0 Twilight Sunrise", Priority = 1 },
                new EventModel { DateTime = specifiedDay.TwilightSunset, Label = "Day-0 Twilight Sunset", Priority = -1 },
                new EventModel { DateTime = nextDay.TwilightSunrise, Label = "Day+1 Twilight Sunrise", Priority = 1 },
                new EventModel { DateTime = nextDay.TwilightSunset, Label = "Day+1 Twilight Sunset", Priority = -1 },
                new EventModel { DateTime = dateToEvaluate, Label = $"Event At {airport.Code}", Priority = 0 }
            };

            var sortedEvents = events.OrderBy(x => x.DateTime).ThenBy(x => x.Priority).ToList();

            Console.WriteLine();
            foreach (var sortedEvent in sortedEvents)
            {
                Console.WriteLine($"{sortedEvent.Label.PadRight(23, ' ') + ":"} {sortedEvent.DateTime}");
            }
        }

        private static SunriseSunsetModel GetDateSunriseSunsetAtAirport(DateTime dateTime, AirportModel airport)
        {
            var client = new RestClient("https://api.sunrise-sunset.org");
            
            var request = new RestRequest("json", DataFormat.Json);
            request.AddParameter("lat", airport.Latitude);
            request.AddParameter("lng", airport.Longitude);
            request.AddParameter("date", dateTime.ToString("yyyy-MM-dd"));
            request.AddParameter("formatted", 0);

            var response = client.Get(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SunriseSunsetResponseModel>(response.Content).Results;
            }

            return null;
        }

        private static AirportModel GetAirport(string airportCode)
        {
            var airports = GetAirports();
            return airports.FirstOrDefault(x => x.Code == airportCode);
        }

        private static List<AirportModel> GetAirports()
        {
            return JsonConvert.DeserializeObject<List<AirportModel>>(File.ReadAllText(@"airports.json"));
        }

        private static DateTime ReadTime(DateTime date)
        {
            Console.Write("Enter time (HHmm): ");
            var timeInput = Console.ReadLine();

            if (timeInput.Length != 4 || !int.TryParse(timeInput.Substring(0, 2), out var hour) || !int.TryParse(timeInput.Substring(2, 2), out var minute))
            {
                throw new Exception("Invalid time entered. Format must be HHmm.");
            }

            return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0, DateTimeKind.Utc);
        }

        private static DateTime ReadDate()
        {
            Console.Write("Enter date (yyyyMMdd): ");
            var dateInput = Console.ReadLine();

            if (dateInput.Length != 8 || !int.TryParse(dateInput.Substring(0, 4), out var year) || !int.TryParse(dateInput.Substring(4, 2), out var month) || !int.TryParse(dateInput.Substring(6,2), out var day))
            {
                throw new Exception("Invalid date entered. Format must be YYYMMDD.");
            }

            return new DateTime(year, month, day);
        }

        private static string ReadAirportCode()
        {
            Console.Write("Enter airport ICAO code: ");
            var airport = Console.ReadLine().ToUpper();
            if (string.IsNullOrEmpty(airport) || airport.Length != 4)
            {
                throw new Exception("Invalid airport ICAO code entered. Aiport ICAO code must have 4 characters.");
            }

            return airport;
        }
    }
}