using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    class Program
    {
        static readonly string inputFile = Path.Combine(Environment.CurrentDirectory, "InputFile.txt");
        static readonly string populationFile = Path.Combine(Environment.CurrentDirectory, "PopulationFile.txt");

        static void Main(string[] args)
        {
            string[] inputLines = { };
            string[] populationLines = { };
            int cityLength = 0;
            int visitedCities = 0;
            string startingCity = "";
            //int amtOfCityConnected;
            List<string> result = new List<string>();
            Dictionary<string, int> cityPopulationList = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, float>> connectedCityList = new Dictionary<string, Dictionary<string, float>>();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            Dictionary<string, int> dayCityGotInfected = new Dictionary<string, int>();

            if (File.Exists(inputFile))
            {
                inputLines = File.ReadAllLines(inputFile);
                Console.WriteLine("INPUT FILE TEXT");
                int.TryParse(inputLines[0], out cityLength);
            }

            // INPUT LIST OF CITY
            Console.WriteLine("~~~~~~~~~~~~~~~~~~INPUT CITY LIST~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("The amount of connected cities: {0}", cityLength);
            Console.WriteLine();

            // INPUT CONNECTED CITY
            Console.WriteLine("~~~~~~~~~~~~INPUT CONNECTING CITY~~~~~~~~~~~~~~~");
            foreach (string inputLine in inputLines)
            {
                string[] inputLineSplit = { };
                float weight;
                if (inputLine != inputLines[0])
                {
                    inputLineSplit = inputLine.Split(" ", 3, StringSplitOptions.RemoveEmptyEntries);
                    float.TryParse(inputLineSplit[2], out weight);
                    AddConnectedCity(inputLineSplit[0], inputLineSplit[1], weight, connectedCityList);
                    if (!visited.ContainsKey(inputLineSplit[0]))
                    {
                        visited.Add(inputLineSplit[0], false);
                    }
                    if (!visited.ContainsKey(inputLineSplit[1]))
                    {
                        visited.Add(inputLineSplit[1], false);
                    }
                }
            }

            // INPUT CITY POPULATION
            Console.WriteLine("~~~~~~~~~~~INPUT CITY POPULATION~~~~~~~~~~~");
            if (File.Exists(populationFile))
            {
                populationLines = File.ReadAllLines(populationFile);
                startingCity = populationLines[0].Split(" ", 2, StringSplitOptions.RemoveEmptyEntries)[1];
                foreach (string populationLine in populationLines)
                {
                    Console.WriteLine(populationLine);
                    if (populationLine != populationLines[0])
                    {
                        string[] populationLineSplit = populationLine.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
                        if (visited.ContainsKey(populationLineSplit[0]))
                        {
                            int tempPopulation;
                            int.TryParse(populationLineSplit[1], out tempPopulation);
                            cityPopulationList.Add(populationLineSplit[0], tempPopulation);
                            Console.WriteLine(tempPopulation);
                        }
                    }
                }
            }
            // NGEPRINT DOANG
            Console.WriteLine("~~~~~~~CITY POPULATION LIST~~~~~~~~~");
            foreach (KeyValuePair<string, int> cityPopulation in cityPopulationList)
            {
                Console.WriteLine("City {0} Population: {1}", cityPopulation.Key, cityPopulation.Value);
            }

            Console.WriteLine("~~~~~~~CITY CONNECTION LIST~~~~~~~~");
            foreach (KeyValuePair<string, Dictionary<string, float>> connectedCity in connectedCityList)
            {
                Console.WriteLine("City {0} to :", connectedCity.Key);
                foreach (KeyValuePair<string, float> city in connectedCity.Value)
                {
                    Console.WriteLine("{0} => {1}", city.Key, city.Value);
                }
            }

            // TEST BFS
            Console.WriteLine("~~~~~~~~~~~~BFS~~~~~~~~~~~~");
            int inputDays;
            Console.Write("Input jumlah hari total: ");
            inputDays = Convert.ToInt32(Console.ReadLine());
            BFS("A", visited, connectedCityList, cityPopulationList, result, inputDays);
            Console.WriteLine("~~~~~~~~~~~~HASIL BFS~~~~~~~~~~~~");
            Console.Write("[");
            foreach (string cityResult in result)
            {
                Console.Write(" {0},", cityResult);
            }
            Console.WriteLine("]");

        }

        // AddConnectedCity()
        // Adds the connection from {sourceCity} to {targetCity} with weight = {weight} to the dictionary {connectedCityList}
        public static void AddConnectedCity(string sourceCity, string targetCity, float weight, Dictionary<string, Dictionary<string, float>> connectedCityList)
        {
            if (connectedCityList.ContainsKey(sourceCity))
            {
                if (!connectedCityList[sourceCity].ContainsKey(targetCity))
                {
                    connectedCityList[sourceCity].Add(targetCity, weight);
                }
                else
                {
                    Console.WriteLine("Connection already established.");
                }
            } else
            {
                Dictionary<string, float> tempDict = new Dictionary<string, float>();
                tempDict.Add(targetCity, weight);
                connectedCityList.Add(sourceCity, tempDict);
            }
        }

        // Function for I(A, t(A))
        public static float infectedPopulationInCity(string plaguedCity, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, int totalDays) {
            float T_city = dayCityGotInfected[plaguedCity]; // T(plaguedCity)
            Console.WriteLine("T({0}) = {1}", plaguedCity, T_city);
            float t_city = totalDays - T_city; // t(plaguedCity)
            Console.WriteLine("t({0}) = {1}", plaguedCity, t_city);
            float cityPopulation = cityPopulationList[plaguedCity]; // P(plaguedCity)
            Console.WriteLine("P({0}) = {1}", plaguedCity, cityPopulation);
            // Console.WriteLine("{0}", (float)Math.Exp(0.25 * t_city));

            return (cityPopulation / (1 + ((cityPopulation - 1) / (float)Math.Exp(0.25 * t_city))));
        }

        // Function for finding how many days adjacentCity got infected after plaguedCity got infected
        // (t)
        public static int whenCityGotInfected(string plaguedCity, string adjacentCity, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, Dictionary<string, Dictionary<string, float>> connectedCityList, int totalDays) {
            float cityPopulation = cityPopulationList[plaguedCity]; // P(A)
            float travelChance = connectedCityList[plaguedCity][adjacentCity]; // Tr(A,B)
            double tempDays = (Math.Log((cityPopulation - 1) / (travelChance * cityPopulation - 1)) * 4);
            // Karena di spek > 1,
            if (tempDays % 1 == 0) {
                tempDays += 1;
            } else {
                tempDays = Math.Ceiling(tempDays);
            }

            return (int)tempDays;
        }

        // Function to check if an adjacentCity will get infected by a plagueCity
        public static bool isSpread(string plaguedCity, string adjacentCity, Dictionary<string, Dictionary<string, float>> connectedCityList, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, int totalDays) {
            float travelChance = connectedCityList[plaguedCity][adjacentCity]; // Tr(PlaguedCity, adjacentCity)
            Console.WriteLine("From City {0}", plaguedCity);
            Console.WriteLine("To City {0}", adjacentCity);
            float infectedPopulation = infectedPopulationInCity(plaguedCity, cityPopulationList, dayCityGotInfected, totalDays); // I(plaguedCity, t(plaguedCity))
            float spreadChance = infectedPopulation * travelChance; // S(plaguedCity, adjacentCity)
            Console.WriteLine("Tr({0}, {1}) = {2}", plaguedCity, adjacentCity, travelChance);
            Console.WriteLine("I({0}) = {1}", plaguedCity, infectedPopulation);
            Console.WriteLine("S({0}, {1}) = {2}", plaguedCity, adjacentCity, spreadChance);

            return (spreadChance > 1);
        }

        public static void BFS(string startingCity, Dictionary<string, bool> visited, Dictionary<string, Dictionary<string, float>> connectedCityList, Dictionary<string, int> cityPopulationList, List<string> result, int totalDays)
        {
            Queue<string> bfsQueue = new Queue<string>();
            Dictionary<string, int> dayCityGotInfected = new Dictionary<string, int>(); // To keep track of the start day when a city got infected
            bfsQueue.Enqueue(startingCity);
            dayCityGotInfected[startingCity] = 0;
            result.Add(startingCity);
            while (bfsQueue.Count > 0)
            {
                string plaguedCity = bfsQueue.Peek();
                visited[plaguedCity] = true;
                bfsQueue.Dequeue();
                foreach (KeyValuePair<string, float> adjacentCity in connectedCityList[plaguedCity])
                {
                    if ((!visited[adjacentCity.Key]) && (isSpread(plaguedCity, adjacentCity.Key, connectedCityList, cityPopulationList, dayCityGotInfected, totalDays)))
                    {
                        bfsQueue.Enqueue(adjacentCity.Key);
                        visited[adjacentCity.Key] = true;
                        result.Add(adjacentCity.Key);
                        int dayAdjacentCityInfected = whenCityGotInfected(plaguedCity, adjacentCity.Key, cityPopulationList, dayCityGotInfected, connectedCityList, totalDays);
                        dayCityGotInfected.Add(adjacentCity.Key, dayAdjacentCityInfected + dayCityGotInfected[plaguedCity]); // Add an infected city to the dict

                        Console.WriteLine("City {0} got infected from city {1}!", adjacentCity.Key, plaguedCity);
                    }
                }
            }

            foreach (KeyValuePair<string, int> city in dayCityGotInfected)
            {
                Console.WriteLine("City {0} Infected Since Day: {1}", city.Key, city.Value);
            }
        }

        // public static void BFS(string startingCity, Dictionary<string, bool> visited, Dictionary<string, Dictionary<string, float>> connectedCityList, List<string> result)
        // {
        //     Queue<string> bfsQueue = new Queue<string>();
        //     bfsQueue.Enqueue(startingCity);
        //     result.Add(startingCity);
        //     while (bfsQueue.Count > 0)
        //     {
        //         string plaguedCity = bfsQueue.Peek();
        //         visited[plaguedCity] = true;
        //         bfsQueue.Dequeue();
        //         foreach (KeyValuePair<string, float> adjacentCity in connectedCityList[plaguedCity])
        //         {
        //             if (!visited[adjacentCity.Key])
        //             {
        //                 bfsQueue.Enqueue(adjacentCity.Key);
        //                 visited[adjacentCity.Key] = true;
        //                 result.Add(adjacentCity.Key);
        //             }
        //         }
        //     }
        // }
    }
}
