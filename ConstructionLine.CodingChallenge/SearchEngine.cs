using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;
        private readonly Dictionary<string, SizeContainer> _sizes;
        private readonly Dictionary<string, ColorContainer> _colors;


        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;
            _sizes = new Dictionary<string, SizeContainer>();
            _colors = new Dictionary<string, ColorContainer>();

            InitSizes();
            InitColors();

            foreach (var shirt in _shirts)
            {
                _sizes[shirt.Size.Name].Shirts.Add(shirt);
                _colors[shirt.Color.Name].Shirts.Add(shirt);
            }
        }
    
        public SearchResults Search(SearchOptions options)
        {
            var result = new SearchResults
            {
                Shirts = new List<Shirt>(),
                SizeCounts = new List<SizeCount>(),
                ColorCounts = new List<ColorCount>()
            };

            var shirts = new Dictionary<string, Shirt>();

            var filterningBySize = options.Sizes.Any();
            var filterningByColor = options.Colors.Any();

            foreach (var size in _sizes)
            {
                if (filterningBySize && !options.Sizes.Any(s => s.Name == size.Key))
                {
                    continue;
                }

                var additionalShirts = size.Value.Shirts
                    .Where(s => !filterningByColor || options.Colors.Any(c => c.Name == s.Color.Name));

                result.Shirts.AddRange(additionalShirts);
            }

            CountSizesAndColors(result);

            return result;
        }

        private void InitSizes()
        {
            foreach (var size in Size.All)
            {
                if (_sizes.ContainsKey(size.Name)) continue;

                _sizes.Add(size.Name, new SizeContainer()
                {
                    Shirts = new List<Shirt>(),
                    Count = new SizeCount()
                    {
                        Size = size,
                        Count = 0
                    }
                });
            }
        }

        private void InitColors()
        {
            foreach (var color in Color.All)
            {
                if (_colors.ContainsKey(color.Name)) continue;

                _colors.Add(color.Name, new ColorContainer()
                {
                    Shirts = new List<Shirt>(),
                    Count = new ColorCount()
                    {
                        Color = color,
                        Count = 0
                    }
                });
            }
        }

        private void CountSizesAndColors(SearchResults result)
        {
            foreach (var shirt in result.Shirts)
            {
                _sizes[shirt.Size.Name].Count.Count++;
                _colors[shirt.Color.Name].Count.Count++;
            }
            result.SizeCounts = _sizes.Select(s => s.Value.Count).ToList();
            result.ColorCounts = _colors.Select(c => c.Value.Count).ToList();
        }
    }
}