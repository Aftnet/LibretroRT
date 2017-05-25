function loadConfiguration()
{
Add-Type @'
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string FileName { get; set; }
    }

    public class TileConfiguration
    {
        public string LayerName { get; set; }
        public Size[] Sizes { get; set; }
    }

    public class TileConfigurations
    {
        readonly Dictionary<int, Tuple<string, Dictionary<int, int>>> SquareScaleData;
        readonly Dictionary<string, Tuple<string, Dictionary<int, Tuple<int, int>>>> RectangleScaleData;
        readonly int[] TargetSizes = new int[] { 16, 24, 32, 48, 256 };

        public TileConfigurations()
        {
            SquareScaleData = new Dictionary<int, Tuple<string, Dictionary<int, int>>>()
            {
                { 44, Tuple.Create("Tile_ListView", new Dictionary<int, int>() { { 100, 44 }, { 125, 55 }, { 150, 66 }, { 200, 88 }, { 400, 176 } } )},
                { 50, Tuple.Create("Tile_ListView", new Dictionary<int, int>() { { 100, 50 }, { 125, 63 }, { 150, 75 }, { 200, 100 }, { 400, 200 } } )},
                { 71, Tuple.Create("Tile_Small", new Dictionary<int, int>() { { 100, 71 }, { 125, 89 }, { 150, 107 }, { 200, 142 }, { 400, 284 } } )},
                { 150, Tuple.Create("Tile_MediumLarge", new Dictionary<int, int>() { { 100, 150 }, { 125, 188 }, { 150, 225 }, { 200, 300 }, { 400, 600 } } )},
                { 310, Tuple.Create("Tile_MediumLarge", new Dictionary<int, int>() { { 100, 310 }, { 125, 388 }, { 150, 465 }, { 200, 620 }, { 400, 1240 } } )}
            };

            RectangleScaleData = new Dictionary<string, Tuple<string, Dictionary<int, Tuple<int, int>>>>()
            {
                { "Wide310x150Logo", Tuple.Create("Tile_WideSplash", new Dictionary<int, Tuple<int, int>>() { { 100, Tuple.Create(310, 150) }, { 125, Tuple.Create(388, 188) }, { 150, Tuple.Create(465, 225) }, { 200, Tuple.Create(620, 300) }, { 400, Tuple.Create(1240, 600) } } )},
                { "SplashScreen", Tuple.Create("Tile_WideSplash", new Dictionary<int, Tuple<int, int>>() { { 100, Tuple.Create(620, 300) }, { 125, Tuple.Create(775, 375) }, { 150, Tuple.Create(930, 450) }, { 200, Tuple.Create(1240, 600) }, { 400, Tuple.Create(2480, 1200) } } )}
            };
        }

        public TileConfiguration[] GetConfigurations()
        {
            var squareScaleTiles = SquareScaleData.Select(d => new TileConfiguration
            {
                LayerName = d.Value.Item1,
                Sizes = d.Value.Item2.Select(e => new Size
                {
                    Width = e.Value,
                    Height = e.Value,
                    FileName = string.Format("Square{0}x{0}Logo.scale-{1}.png", d.Key, e.Key)
                }).ToArray()
            });

            var rectangleScaleTiles = RectangleScaleData.Select(d => new TileConfiguration
            {
                LayerName = d.Value.Item1,
                Sizes = d.Value.Item2.Select(e => new Size
                {
                    Width = e.Value.Item1,
                    Height = e.Value.Item2,
                    FileName = string.Format("{0}.scale-{1}.png", d.Key, e.Key)
                }).ToArray()
            });

			var platedScaleTiles = new TileConfiguration
            {
                LayerName = "Tile_ListView",
                Sizes = TargetSizes.Select(d => new Size { Width = d, Height = d, FileName = string.Format("Square44x44Logo.targetsize-{0}.png", d) }).ToArray()
            };

            var unplatedScaleTiles = new TileConfiguration
            {
                LayerName = "Tile_Target",
                Sizes = TargetSizes.Select(d => new Size { Width = d, Height = d, FileName = string.Format("Square44x44Logo.targetsize-{0}_altform-unplated.png", d) }).ToArray()
            };

            var output = squareScaleTiles.Concat(rectangleScaleTiles).ToList();
			output.Add(platedScaleTiles);
            output.Add(unplatedScaleTiles);

            return output.ToArray();
        }
    }
'@

$config = New-Object TileConfigurations;
return $config.GetConfigurations()
}

$inkScapePath = "C:\Users\Alberto\Downloads\InkscapePortable\inkscapeportable.exe"
$iconMasterFile = ".\Icon Master.svg"

$backgroundColor = "#ff6600"
$tempFile = ".\Temp.svg"

echo "Deleting old icons"
Remove-Item -Path *.png

$config = loadConfiguration
foreach($tileConfig in $config)
{
    [xml]$xml = Get-Content $iconMasterFile
    $svgNode = $xml.ChildNodes | Where-Object { $_.Name -eq "svg" } | Select-Object -First 1
    $metadataNode = $svgNode.ChildNodes | Where-Object { $_.Name -eq "metadata" } | Select-Object -First 1  
    $groupNode = $svgNode.ChildNodes | Where-Object { $_.Name -eq "g" } | Where-Object { $_.label -eq $tileConfig.LayerName } | Select-Object -First 1  

    $null = $svgNode.RemoveAll()
    $null = $svgNode.AppendChild($metadataNode)
    $null = $svgNode.AppendChild($groupNode)
    $xml.Save($tempFile)
    
    $text = Get-Content $tempFile
    $text = $text.Replace("fill:$($backgroundColor)", "fill:#000000;fill-opacity:0")
    $text | Out-File $tempFile
    
    foreach($size in $tileConfig.Sizes)
    {
		echo "Generating " $size.FileName
        &$inkScapePath $tempFile --export-area-drawing --export-png=$($size.FileName) --export-width=$($size.Width) --export-height=$($size.Height)
        ##Wait for Inkscape o process image, otherwise it stops responding
        Start-Sleep -s 2

    }

    Remove-Item($tempFile);
}
