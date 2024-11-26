using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace PuzzleSolverLib.Puzzles.Y2023
{

    public record MapData
    {
        public MapData(long source, long destination, long length)
        {
            DestinationRangeStart = destination;
            SourceRangeStart = source;
            RangeLength = length;
        }
        public long DestinationRangeStart { get; init; }
        public long DestinationRangeEnd => DestinationRangeStart + RangeLength - 1; // start number is included in the range
        public long SourceRangeStart { get; init; }
        public long SourceRangeEnd => SourceRangeStart + RangeLength - 1; // start number is included in the range

        // Assume source range has already been range checked and is within the destination range
        public Range GetDestinationRange(Range source)
        {
            var diff = source.Start - SourceRangeStart;
            return new Range() { Start = DestinationRangeStart + diff , Length = source.Length };
        } 

        public long RangeLength { get; init; }


    }

    public struct Range
    {
        public long Start { get; set; }

        public long End => Start + Length - 1; // start number is included in the range

        public long Length { get; set; }

    }

    public partial class Day05b : PuzzleBaseClass
    {
        public override string Description => @"Too long, didn't read... see http://adventToCode.com/2023/day/5";

        [GeneratedRegex(@"((?<num>\d+)\s+(?<range>\d+))+")]
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


                var seeds = new HashSet<Range>();

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
                                seeds.Add(new Range() { Start = long.Parse(match.Groups["num"].Value), Length = long.Parse(match.Groups["range"].Value) });
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

                var locations = new HashSet<long>() ;
                foreach (var seed in seeds)
                {
                    Log.WriteInfo($"Mapping seeds in range [{seed.Start}-{seed.End}]");

                    Log.WriteSuccess($"Mapping seeds to soil");
                    var soil = GetMapValue(ref seed_soil, seed);
                    Log.WriteSuccess("Mapping soil to fertilizer");
                    var fertilizer = GetMapValues(ref soil_fertilizer, soil);
                    Log.WriteSuccess("Mapping fertilizer to water");
                    var water = GetMapValues(ref fertilizer_water, fertilizer);
                    Log.WriteSuccess("Mapping water to light");
                    var light = GetMapValues(ref water_light, water);
                    Log.WriteSuccess("Mapping light to temperature");
                    var temperature = GetMapValues(ref light_temperature, light);
                    Log.WriteSuccess("Mapping temperature to humidity");
                    var humidity = GetMapValues(ref temperature_humidity, temperature);
                    Log.WriteSuccess("Mapping humidity to location");
                    var location = GetMapValues(ref humidity_location, humidity);

                    Log.EmptyLine();

                    foreach(var loc in location)
                    {
                        locations.Add(loc.Start);
                    }
      
                }


                return locations.Min().ToString();
            }
            catch (Exception ex)
            {
                LastError = ex;
                return null;
            }
        }

        public Range[] GetMapValues(ref List<MapData> mapData, Range[] sourceValues)
        {
            var ranges = new ConcurrentBag<Range>();

            var localMapData = mapData;

            Parallel.ForEach(sourceValues, ()=>new List<Range>(), (sourceValue, state, list) =>
            {
                 list.AddRange(GetMapValue(ref localMapData, sourceValue));
                 return list;
            }, result =>
            {
                foreach (var range in result)
                {
                    ranges.Add(range);
                }
            }
            );

            return ranges.ToArray();
        }

        public Range[] GetMapValue(ref List<MapData> mapData, Range sourceRangeToMap)
        {
            var ranges = new List<Range>();

            // get rid of edge cases
            var minSourceRangeStart = mapData.Min(x => x.SourceRangeStart);
            var maxSourceRangeEnd = mapData.Max(x => x.SourceRangeEnd);

            if (sourceRangeToMap.End < minSourceRangeStart || sourceRangeToMap.Start > maxSourceRangeEnd)
            {
                // source - target mapping are equal
                ranges.Add(sourceRangeToMap);
                return ranges.ToArray();
            }

            var rangesToMap = new Queue<Range>();
            rangesToMap.Enqueue(sourceRangeToMap);

            while (rangesToMap.TryDequeue(out var rangeToMap))
            {
                if (rangeToMap.End < minSourceRangeStart || rangeToMap.Start > maxSourceRangeEnd)
                {
                    // source - target mapping are equal
                    ranges.Add(rangeToMap);
                    continue;
                }

                // Get all mapdata ranges which contains the start of the source range
                var mapDataAllWithValidStart = mapData
                    .Where(x => x.SourceRangeStart <= rangeToMap.Start && rangeToMap.Start <= x.SourceRangeEnd)
                    .OrderBy(x => x.SourceRangeStart)
                    .ToArray();

                foreach (var mapRange in mapDataAllWithValidStart)
                {
                    // if the end of the source range is greater than the end of the map range, we need to split the source range
                    if (rangeToMap.End > mapRange.SourceRangeEnd)
                    {
                        ranges.Add(mapRange.GetDestinationRange(new Range() { Start = rangeToMap.Start, Length = mapRange.SourceRangeEnd - rangeToMap.Start }));
                        rangesToMap.Enqueue(new Range() { Start = mapRange.SourceRangeEnd + 1, Length = rangeToMap.End - mapRange.SourceRangeEnd});
                        continue;
                    }

                    // if the end of the source range is less or equal to end of the map range, we can map the whole source range
                    ranges.Add(mapRange.GetDestinationRange(new Range() { Start = rangeToMap.Start, Length = rangeToMap.Length }));
                }

                var mapDataWithValidEnd = mapData
                    .Where(x => rangeToMap.Start < x.SourceRangeStart && rangeToMap.End >= x.SourceRangeStart && rangeToMap.End <= x.SourceRangeEnd)
                    .ToArray();

                foreach (var mapRange in mapDataWithValidEnd)
                {
                    ranges.Add(mapRange.GetDestinationRange(new Range() { Start = mapRange.SourceRangeStart, Length = rangeToMap.End - mapRange.SourceRangeStart }));
                    if (rangeToMap.Start < mapRange.SourceRangeStart && rangeToMap.End >= mapRange.SourceRangeStart)
                        rangesToMap.Enqueue(new Range() { Start = rangeToMap.Start, Length = (mapRange.SourceRangeStart - 1) - rangeToMap.Start });
                }
            }

            return ranges.ToArray();
        }

    }
}
