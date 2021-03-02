using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Whetstone
{
    public class Colors
    {
        public Colors(string path)
        {
            swatch = new Swatch(path);

            screen = swatch.primaryDarkest;       

            messageDefault = swatch.complementLighter;

            player = swatch.primaryDarkest;

            floor = swatch.secondaryLighter_2;
            bgFloor = swatch.secondary_2;
            oosFloor = swatch.secondaryDarker_2;
            oosBgFloor = swatch.secondaryDarkest_2;

            wall = swatch.secondaryLightest_1;
            bgWall = swatch.secondary_1;
            oosWall = swatch.secondaryDarker_1;
            oosBgWall = swatch.secondaryDarkest_1;
        }

        public Color screen;

        public Color messageDefault;

        public Color player;

        public Color floor;
        public Color bgFloor;
        public Color oosFloor;
        public Color oosBgFloor;

        public Color wall;
        public Color bgWall;
        public Color oosWall;
        public Color oosBgWall;

        public Swatch swatch { get; private set; }

        public class Swatch
        {
            public Color primary { get; private set; }
            public Color primaryLightest { get; private set; }
            public Color primaryLighter { get; private set; }
            public Color primaryDarker { get; private set; }
            public Color primaryDarkest { get; private set; }

            public Color secondary_1 { get; private set; }
            public Color secondaryLightest_1 { get; private set; }
            public Color secondaryLighter_1 { get; private set; }
            public Color secondaryDarker_1 { get; private set; }
            public Color secondaryDarkest_1 { get; private set; }

            public Color secondary_2 { get; private set; }
            public Color secondaryLightest_2 { get; private set; }
            public Color secondaryLighter_2 { get; private set; }
            public Color secondaryDarker_2 { get; private set; }
            public Color secondaryDarkest_2 { get; private set; }

            public Color complement { get; private set; }
            public Color complementLightest { get; private set; }
            public Color complementLighter { get; private set; }
            public Color complementDarker { get; private set; }
            public Color complementDarkest { get; private set; }

            public Swatch(string path)
            {
                XDocument xdoc = XDocument.Load(path);

                foreach (XElement xColorset in xdoc.Root.Elements("colorset"))
                {
                    Color[] colors = GetColorsFromColorset(xColorset.Elements("color"));
                    switch (xColorset.Attribute("id").Value)
                    {
                        case "primary":                   
                            primary = colors[0];
                            primaryLightest = colors[1];
                            primaryLighter = colors[2];
                            primaryDarker = colors[3];
                            primaryDarkest = colors[4];
                            break;

                        case "secondary-1":
                            secondary_1 = colors[0];
                            secondaryLightest_1 = colors[1];
                            secondaryLighter_1 = colors[2];
                            secondaryDarker_1 = colors[3];
                            secondaryDarkest_1 = colors[4];
                            break;

                        case "secondary-2":
                            secondary_2 = colors[0];
                            secondaryLightest_2 = colors[1];
                            secondaryLighter_2 = colors[2];
                            secondaryDarker_2 = colors[3];
                            secondaryDarkest_2 = colors[4];
                            break;

                        case "complement":
                            complement = colors[0];
                            complementLightest = colors[1];
                            complementLighter = colors[2];
                            complementDarker = colors[3];
                            complementDarkest = colors[4];
                            break;
                    }
                }
            }

            private Color[] GetColorsFromColorset(IEnumerable<XElement> elements)
            {
                int index = 0;
                Color[] colors = new Color[5];
                foreach (XElement xColor in elements)
                {
                    int r = Int32.Parse(xColor.Attribute("r").Value);
                    int g = Int32.Parse(xColor.Attribute("g").Value);
                    int b = Int32.Parse(xColor.Attribute("b").Value);
                    colors[index] = new Color(r, g, b);
                    index++;
                }
                return colors;
            }
        }
    }
}
