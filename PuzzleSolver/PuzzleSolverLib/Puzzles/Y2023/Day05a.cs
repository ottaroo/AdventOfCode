using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023
{


    public partial class Day05a : PuzzleBaseClass
    {
        public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/5";

        [GeneratedRegex(@"(?<num>\d+)")]
        public partial Regex Numbers();

        [GeneratedRegex(@"(?<source>\w+)-to-(?<dest>\w+)\s*map:")]
        public partial Regex MapSourceDestination();

        [GeneratedRegex(@"(?<dest>\d+)\s+(?<source>\d+)\s+(?<len>\d+)")]
        public partial Regex MapDataSearch();


        public override string? OnSolve(ReadOnlySpan<char> inputFile)
        {
            try
            {
                using var fs = new FileStream(inputFile.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
                using var sr = new StreamReader(fs, Encoding.UTF8, true, 4096, true);


                var seeds = new HashSet<long>();

                var seed_soil = new List<MapData>();
                var soil_fertilizer = new List<MapData>();
                var fertilizer_water = new List<MapData>();
                var water_light = new List<MapData>();
                var light_temperature = new List<MapData>();
                var temperature_humidity = new List<MapData>();
                var humidity_location = new List<MapData>();

                var getMap = new Func<string, List<MapData>>(map =>
                {
                    return map switch
                    {
                        "seed" => seed_soil,
                        "soil" => soil_fertilizer,
                        "fertilizer" => fertilizer_water,
                        "water" => water_light,
                        "light" => light_temperature,
                        "temperature" => temperature_humidity,
                        "humidity" => humidity_location,
                        _ => new List<MapData>()
                    };
                });


                List<MapData>? currentMap = null;

                while (!sr.EndOfStream)
                {

                    var line = sr.ReadLine();
                    if (line == null)
                        continue;

                    if (line.StartsWith("seeds:", StringComparison.OrdinalIgnoreCase))
                    {
                        Log.WriteInfo("Processing seeds");
                        foreach (Match match in Numbers().Matches(line))
                        {
                            if (match.Success)
                                seeds.Add(long.Parse(match.Groups["num"].Value));
                        }

                        continue;
                    }

                    var mapMatch = MapSourceDestination().Match(line);
                    if (mapMatch.Success)
                    {
                        Log.WriteInfo($"Processing map {mapMatch.Groups["source"].Value} to {mapMatch.Groups["dest"].Value}");
                        currentMap = getMap(mapMatch.Groups["source"].Value);
                        continue;
                    }

                    if (currentMap == null)
                        continue;

                    var mapDataMatch = MapDataSearch().Match(line);
                    if (mapDataMatch.Success)
                    {
                        currentMap.Add(new MapData(
                            long.Parse(mapDataMatch.Groups["source"].Value),
                            long.Parse(mapDataMatch.Groups["dest"].Value),
                            long.Parse(mapDataMatch.Groups["len"].Value)
                        ));
                    }
                }

                var locations = new HashSet<KeyValuePair<long, long>>();
                foreach (var seed in seeds)
                {
                    var soil = GetMapValue(ref seed_soil, seed);
                    var fertilizer = GetMapValue(ref soil_fertilizer, soil);
                    var water = GetMapValue(ref fertilizer_water, fertilizer);
                    var light = GetMapValue(ref water_light, water);
                    var temperature = GetMapValue(ref light_temperature, light);
                    var humidity = GetMapValue(ref temperature_humidity, temperature);
                    var location = GetMapValue(ref humidity_location, humidity);

                    locations.Add(new KeyValuePair<long, long>(seed, location));
                }


                return locations.Min(x => x.Value).ToString();
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }

        public long GetMapValue(ref List<MapData> mapData, long sourceValue)
        {
            var data = mapData.FirstOrDefault(x => sourceValue >= x.SourceRangeStart && sourceValue < (x.SourceRangeStart + x.RangeLength));
            if (data == null)
                return sourceValue;

            var diff = sourceValue - data.SourceRangeStart;

            return data.DestinationRangeStart + diff;
        }
    }
}
