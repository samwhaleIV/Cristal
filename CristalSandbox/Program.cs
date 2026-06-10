using ScottPlot;

namespace CristalSandbox {

    internal class Program {

        static void MaterialDataDensityAnalysis(
            int nanometerStart,
            int nanometerEnd,
            int bucketLength,
            int bucketBitDepth
        ) {
            int nanometerRange = Math.Abs(nanometerStart - nanometerEnd);
            float bucketCount = (float)nanometerRange / bucketLength;
            float numberOfBytes = bucketCount * bucketBitDepth / 8;
            Console.WriteLine($"Start: {nanometerStart}, end: {nanometerEnd}, length: {bucketLength}, depth: {bucketBitDepth}");
            Console.WriteLine($"Range: {nanometerRange}, buckets: {bucketCount}, size: {numberOfBytes} bytes");
        }

        static void MaterialDataDensityAnalysisInteractive() {

            int start = 400;
            int end = 700;
            int bucketLength = 4;
            int bitDepth = 8;

            Restart:

            Console.WriteLine("1: -start, 2: +start, 3: -end, 4: +end, 5: -length, 6: +length, 7: shift left, 8: shift right");
            MaterialDataDensityAnalysis(start,end,bucketLength,bitDepth);

            char key = Console.ReadKey(true).KeyChar;
            Console.Clear();
            switch(key) {
                case '1':   start   -= bucketLength;    break;
                case '2':   start   += bucketLength;    break;
                case '3':   end     -= bucketLength;    break;
                case '4':   end     += bucketLength;    break;
                case '5':   bucketLength--;             break;
                case '6':   bucketLength++;             break;
                case '7':
                    start -= bucketLength;
                    end -= bucketLength;
                    break;
                case '8':
                    start += bucketLength;
                    end += bucketLength;
                    break;
            }

            goto Restart;
        }

        static  QuantizeSpectralData(ReadOnlySpan<SpectralPowerPoint> points) {

        }

        static void IngestExternalSpectralData(string root,string spectralFile) {

            // Wavelength profile matching based on the substring in the spectral file
            string? wavelengthsFile = Path.GetFileNameWithoutExtension(spectralFile).Split('_')[^2][0..^1] switch {
                "ASDFR" =>  "splib07b_Wavelengths_ASDFR_0.35-2.5microns_2151ch.txt",
                "AVIRIS" => "splib07b_Wavelengths_BECK_Beckman_interp._3961_ch.txt",
                "BECK" =>   "splib07b_Wavelengths_BECK_Beckman_interp._3961_ch.txt",
                "NIC4" =>   "splib07b_Wavelengths_NIC4_Nicolet_1.12-216microns.txt",
                _ => null
            };

            if(wavelengthsFile is null) {
                Console.WriteLine($"Could not identify a wavelength profile for spectral file '{spectralFile}'");
                return;
            }

            string[] reflectanceArr =   File.ReadAllLines(Path.Combine(root,spectralFile));
            string[] wavelengthArr =    File.ReadAllLines(Path.Combine(root,wavelengthsFile));

            if(reflectanceArr.Length != wavelengthArr.Length) {
                throw new Exception($"Spectral file '{spectralFile}' does not match wavelengths file");
            }

            SpectralPowerPoint[] points = new SpectralPowerPoint[reflectanceArr.Length - 1];

            for(int i = 1;i<reflectanceArr.Length;i++) {
                points[i] = new SpectralPowerPoint() {
                    Wavelength =    double.Parse(wavelengthArr[i]) * 1000,
                    Reflectance =   Math.Max(double.Parse(reflectanceArr[i]), 0.0f),
                };
            }

            Plot plot = new();
            plot.Add.ScatterLine(
                points.Select(p => p.Wavelength).ToArray(),
                points.Select(p => p.Reflectance).ToArray()
            );

            plot.XLabel("Wavelength (nm)");
            plot.YLabel("Reflectance %");

            if(!Directory.Exists("SPD Charts")) {
                Directory.CreateDirectory("SPD Charts");
            }

            string imagePath = Path.Combine("SPD Charts",Path.ChangeExtension(Path.GetFileName(spectralFile),".webp"));

            plot.SaveWebp(imagePath,1600,1200,90);

            Console.WriteLine($"Success! Plot saved to '{imagePath}'.");
        }

        static void Main(string[] args) {
            IngestExternalSpectralData(
                root:           "C:\\ASCIIdata_splib07b\\",
                spectralFile:   "ChapterS_SoilsAndMixtures\\splib07b_Halloysite_CU91-242D_ASDFRb_AREF.txt"
            );
        }
    }
}
