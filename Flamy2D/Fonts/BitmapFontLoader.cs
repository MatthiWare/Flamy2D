using Flamy2D.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamy2D.Fonts
{
    public static class BitmapFontLoader
    {
        public static BitmapFont Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName)) throw new FileNotFoundException($"Unable to find bitmap font file at {fileName}", fileName);

            var font = new BitmapFont();

            var resourcePath = Path.GetDirectoryName(fileName);
            var pageData = new SortedDictionary<int, Page>();

            font.Kernings = new Dictionary<Kerning, int>();
            font.Characters = new Dictionary<char, Character>();
            font.Textures = new Dictionary<int, Texture2D>();

            using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var tokens = Split(line, ' ');

                    if (tokens.Length < 1)
                        continue;

                    switch (tokens[0])
                    {
                        case "info":
                            LoadInfo(font, tokens);
                            break;

                        case "common":
                            LoadCommon(font, tokens);
                            break;

                        case "page":
                            var page = LoadPage(resourcePath, tokens);
                            pageData.Add(page.Id, page);
                            break;

                        case "kerning":
                            var kern = LoadKerning(tokens);
                            if (!font.Kernings.ContainsKey(kern))
                                font.Kernings.Add(kern, kern.Amount);

                            break;

                        case "char":
                            var data = LoadCharacter(tokens);
                            font.Characters.Add(data.Char, data);
                            break;
                    }
                }
            }

            font.Pages = pageData.Select(kvp => kvp.Value).ToArray();

            foreach (Page page in font.Pages)
                font.Textures.Add(page.Id, Texture2D.LoadFromFileAsync(page.FileName, TextureConfiguration.Linear));

            return font;
        }

        private static Character LoadCharacter(string[] tokens)
        {
            return new Character
            {
                Char = GetNamedChar("id", tokens),
                Bounds = new Rectangle(
                    GetNamedInt("x", tokens),
                    GetNamedInt("y", tokens),
                    GetNamedInt("width", tokens),
                    GetNamedInt("height", tokens)),
                Offset = new Point(
                    GetNamedInt("xoffset", tokens),
                    GetNamedInt("yoffset", tokens)),
                XAdvance = GetNamedInt("xadvance", tokens),
                TexturePage = GetNamedInt("page", tokens),
                Channel = GetNamedInt("chnl", tokens)
            };
        }

        private static Kerning LoadKerning(string[] tokens) => new Kerning(GetNamedChar("first", tokens), GetNamedChar("second", tokens), GetNamedChar("amount", tokens));

        private static Page LoadPage(string resource, string[] tokens) => new Page(GetNamedInt("id", tokens), Path.Combine(resource, GetNamedString("file", tokens)));


        private static void LoadCommon(BitmapFont font, string[] tokens)
        {
            font.LineHeight = GetNamedInt("lineHeight", tokens);
            font.BaseHeight = GetNamedInt("base", tokens);
            font.TextureSize = new Size
            {
                Width = GetNamedInt("scaleW", tokens),
                Height = GetNamedInt("scaleH", tokens)
            };

            font.Packed = GetNamedBool("packed", tokens);
            font.AlphaChannel = GetNamedInt("alphaChnl", tokens);
            font.RedChannel = GetNamedInt("redChnl", tokens);
            font.GreenChannel = GetNamedInt("greenChnl", tokens);
            font.BlueChannel = GetNamedInt("blueChnl", tokens);
        }

        private static void LoadInfo(BitmapFont font, string[] tokens)
        {
            font.FamilyName = GetNamedString("face", tokens);
            font.FontSize = GetNamedInt("size", tokens);
            font.Bold = GetNamedBool("bold", tokens);
            font.Italic = GetNamedBool("italic", tokens);
            font.Charset = GetNamedString("charset", tokens);
            font.Unicode = GetNamedBool("unicode", tokens);
            font.StretchedHeight = GetNamedInt("stretchH", tokens);
            font.Smoothed = GetNamedBool("smooth", tokens);
            font.SuperSampling = GetNamedInt("aa", tokens);
            font.Padding = ParsePadding(GetNamedString("padding", tokens));
            font.Spacing = ParsePoint(GetNamedString("spacing", tokens));
            font.OutlineSize = GetNamedInt("outline", tokens);
        }

        private static char GetNamedChar(string name, string[] tokens) => (char)GetNamedInt(name, tokens);

        private static int GetNamedInt(string name, string[] tokens) => int.Parse(GetNamedString(name, tokens));

        private static bool GetNamedBool(string name, string[] tokens) => GetNamedInt(name, tokens) != 0;

        private static string GetNamedString(string name, string[] tokens)
        {
            foreach (string s in tokens)
            {
                var data = s.Split('=');

                if (data.Length == 0)
                    continue;

                if (data[0] != name)
                    continue;

                return (data.Length < 2) ? string.Empty : data[1];
            }

            return string.Empty;
        }

        private static Padding ParsePadding(string s)
        {
            var tokens = s.Split(',');

            return new Padding()
            {
                Top = int.Parse(tokens[0].Trim()),
                Right = int.Parse(tokens[1].Trim()),
                Bottom = int.Parse(tokens[2].Trim()),
                Left = int.Parse(tokens[3].Trim())
            };
        }

        private static Point ParsePoint(string s)
        {
            var tokens = s.Split(',');

            return new Point()
            {
                X = int.Parse(tokens[0].Trim()),
                Y = int.Parse(tokens[1].Trim())
            };
        }

        private static string[] Split(string input, char splitter)
        {
            const char quote = '"';

            List<string> items = new List<string>();
            int begin = 0;
            bool isInQuote = false;

            for (int i = 0; i < input.Length; i++)
            {
                char data = input[i];
                var end = i == input.Length - 1;

                if ((data == splitter && !isInQuote) || end)
                {
                    var length = i - begin;
                    if (end) length++;

                    if (length > 0)
                        items.Add(input.Substring(begin, length).Replace($"{quote}", string.Empty));

                    begin = i + 1;
                }

                if (data == quote)
                    isInQuote = !isInQuote;
            }

            return items.ToArray();
        }

    }
}
