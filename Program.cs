using System;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    class Program
    {
        static readonly string inputFile = @"F:\Docs\Kuliah\Stima\Tubes2Test\Test\InputFile.txt";
        static readonly string populationFile = @"F:\Docs\Kuliah\Stima\Tubes2Test\Test\PopulationFile.txt";

        static void Main(string[] args)
        {
            string[] inputLines = { };
            string[] populationLines = { };
            int cityLength = 0;
            string startingCity = "";
            //int amtOfCityConnected;
            List<string> result = new List<string>();
            Dictionary<string, int> cityPopulationList = new Dictionary<string, int>(); 
            Dictionary<string, Dictionary<string, float>> connectedCityList = new Dictionary<string, Dictionary<string, float>>();
            Dictionary<string, bool> visited = new Dictionary<string, bool>();

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
            BFS("A", visited, connectedCityList, result);
            Console.WriteLine("~~~~~~~~~~~~BFS~~~~~~~~~~~~");
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

        public static void BFS(string startingCity, Dictionary<string, bool> visited, Dictionary<string, Dictionary<string, float>> connectedCityList, List<string> result)
        {
            Queue<string> bfsQueue = new Queue<string>();
            bfsQueue.Enqueue(startingCity);
            result.Add(startingCity);
            while (bfsQueue.Count > 0)
            {
                string plaguedCity = bfsQueue.Peek();
                visited[plaguedCity] = true;
                bfsQueue.Dequeue();
                foreach (KeyValuePair<string, float> adjacentCity in connectedCityList[plaguedCity])
                {
                    if (!visited[adjacentCity.Key])
                    {
                        bfsQueue.Enqueue(adjacentCity.Key);
                        visited[adjacentCity.Key] = true;
                        result.Add(adjacentCity.Key);
                    }
                }
            }
        }
    }
}
