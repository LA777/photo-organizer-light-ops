using ImageMagick;
using Microsoft.Extensions.Options;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters.Handler;
using Polo.Extensions;
using Polo.Parameters;
using Polo.Parameters.Handler;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Polo.Commands;

public class CloneExifAndTimestampsCommand : ICommand
{
    // TODO LA - Create Rename-To-Original-File-Name Command

    private const string NameLong = "clone-exif-and-timestamps";
    private const string NameShort = "ceat";
    private readonly ApplicationSettingsReadOnly _applicationSettings;
    private readonly ILogger _logger;

    private const string ExifToolPath = "c:\\Prime\\Progs\\exiftool-13.30_64\\exiftool.exe";



    public CloneExifAndTimestampsCommand(IOptions<ApplicationSettingsReadOnly> applicationOptions, ILogger logger)
    {
        _applicationSettings = applicationOptions.Value ?? throw new ArgumentNullException(nameof(applicationOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Name => NameLong;

    public string ShortName => NameShort;

    public string Description => "Clones all EXIF data and file timestamps.";

    public IParameterHandler ParameterHandler => new ParameterHandler
    {
        // TODO LA - Pass to constructor Desription of Parameter and isRequired: true, false
        SourceParameter = new SourceParameter(), // source (original) files folder full path // required parameter
        DestinationParameter = new DestinationParameter(), // destination files folder full path or current folder
        OutputFolderNameParameter = new OutputFolderNameParameter() // output subfolder name - files with cloned data will be saved to this folder

        // Delimiter for file name ('-', ' ', '(', '_')

        // Owerwrite output file (true, false)

        // Clone all or Some EXIF parameters

        // Clone Timestamp of File (true, false)
    };

    public async Task ActionAsync(IReadOnlyDictionary<string, string> parameters = null!, IEnumerable<ICommand> commands = null!)
    {
        // TODO LA - Cover with UTs
        try
        {
            var sourceFolderPath = ParameterHandler.SourceParameter.Initialize(parameters, string.Empty);
            var destinationFolder = ParameterHandler.DestinationParameter!.Initialize(parameters, Environment.CurrentDirectory);
            var outputFolderName = ParameterHandler.OutputFolderNameParameter!.Initialize(parameters, _applicationSettings.OutputSubfolderName);
            var outputFolderPath = Path.GetFullPath(outputFolderName, destinationFolder);

            // source files folder -- originalFilesFolder
            // current files folder -- filesToUpdateFolder // sourceFolderPath
            // output folder -- destinationFolder

            var imagesForProcess = new List<string>();
            _applicationSettings.FileForProcessExtensions.Distinct().ToList()
                .ForEach(x => imagesForProcess.AddRange(Directory.EnumerateFiles(destinationFolder, $"*{x}", SearchOption.TopDirectoryOnly)));
            imagesForProcess.SortByFileName();

            if (imagesForProcess.Any() && !Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            foreach (var imageForProcessFullPath in imagesForProcess)
            {
                var fileNameWithExtension = Path.GetFileName(imageForProcessFullPath);
                var sourceFileFullPath = Path.GetFullPath(fileNameWithExtension, sourceFolderPath);

                if (!File.Exists(sourceFileFullPath))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithExtension);
                    var split = fileNameWithoutExtension.Split('-'); // TODO LA - Improve Delimiter intake from Parameters
                    var splittedName = split[0];
                    var extension = Path.GetExtension(fileNameWithExtension); // '.jpg'
                    var splittedNameWithExtension = $"{splittedName}{extension}";
                    sourceFileFullPath = Path.GetFullPath(splittedNameWithExtension, sourceFolderPath);

                    if (!File.Exists(sourceFileFullPath))
                    {
                        _logger.Error("File not found: {FileName}", sourceFileFullPath);
                        return;
                    }
                }

                //CloneExif(sourceFileFullPath, imageForProcessFullPath, outputFolderPath);
                ProcessFiles(sourceFileFullPath, imageForProcessFullPath, outputFolderPath);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
        }
    }

    private void ProcessFiles(string sourceFileFullPath, string destinationFileFullPath, string outputFolderFullPath)
    {
        if (!File.Exists(sourceFileFullPath))
        {
            _logger.Error("Source file not found at {SourceFileFullPath}", sourceFileFullPath);
            return;
        }

        if (!File.Exists(destinationFileFullPath))
        {
            _logger.Error("Destination file not found at {DestinationFileFullPath}", destinationFileFullPath);
            return;
        }

        try
        {
            if (!Directory.Exists(outputFolderFullPath))
            {
                Directory.CreateDirectory(outputFolderFullPath);
            }

            var destinationFileName = Path.GetFileName(destinationFileFullPath);
            var outputFileFullPath = Path.Combine(outputFolderFullPath, destinationFileName);

            if (!File.Exists(outputFileFullPath))
            {
                File.Copy(destinationFileFullPath, outputFileFullPath);
            }

            // Clone all tags
            var isSuccessfullCopied = CopyAllTags(sourceFileFullPath, outputFileFullPath);
            if (!isSuccessfullCopied)
            {
                _logger.Error("Error while copy all tags. From: {SourceFile} to {DestinationGFile}", sourceFileFullPath, outputFileFullPath);
                return;
            }

            //Set tags manually
            var tags = new Dictionary<string, string>()
            {
                { "Orientation", "" },

                { "Artist", "Alex Lazarenko" },
               { "Creator", "Alex Lazarenko" },
               { "Copyright", "Alex Lazarenko, LA.xdna@gmail.com" },
               { "Title", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },
               { "Location", "Vintana Wine + Dine" },
               { "Country", "USA" },
               { "State", "California" },
               { "City", "Escondido" },

               { "Software", "RawTherapee 5.11" },

               { "GPSVersionID", "2.3.1.0" },
               { "GPSLatitude", "33.10704045077452" },
               { "GPSLatitudeRef", "N" },
               { "GPSLongitude", "-117.09718202245202" },
               { "GPSLongitudeRef", "W" },
               { "GPSAltitude", "137.0" },
               { "GPSAltitudeRef", "0" },
               //{ "GPSDateStamp", "2025:05:25" }, // TODO
               //{ "GPSTimestamp", "" }, // TODO

               { "IPTC:Contact", "LA.xdna@gmail.com; powaypadre@stjohnofdamascus.org" },
               { "IPTC:Copyright", "Alex Lazarenko, LA.xdna@gmail.com" },
               { "IPTC:Writer", "Alex Lazarenko" },

               { "IPTC:ObjectName", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },
               { "IPTC:Headline", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },
               { "IPTC:CopyrightNotice", "Alex Lazarenko, LA.xdna@gmail.com" },


               { "XMP-iptcCore:CiEmailWork", "LA.xdna@gmail.com; powaypadre@stjohnofdamascus.org" },

               { "XMP-dc:Creator", "Alex Lazarenko" },
               { "XMP-dc:Date", "2025-05-25" },
               { "XMP-dc:Description", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },
               { "XMP-dc:Subject", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },

               { "Description", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },

               { "XMP-photoshop:History", "RawTherapee 5.11 processing applied." },
               { "UserComment", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" },
               { "ImageDescription", "St John of Damascus Orthodox Church - 50th Anniversary, May 2025" }
            };

            var dateTimeTaken = ReadExifTag(outputFileFullPath, "DateTimeOriginal");
            if (dateTimeTaken is not null)
            {
                var isParsed = DateTime.TryParseExact(
                    dateTimeTaken,
                    "yyyy:MM:dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDateTime);

                if (isParsed)
                {
                    var offset = TimeSpan.FromHours(-8);
                    var offsetExif = ReadExifTag(outputFileFullPath, "OffsetTimeOriginal");
                    if (offsetExif is not null)
                    {
                        const string format = @"hh:mm"; // "-08:00"
                        var isOffsetParsed = TimeSpan.TryParseExact(offsetExif, format, CultureInfo.InvariantCulture, out var offsetParsed);
                        if (isOffsetParsed)
                        {
                            offset = offsetParsed;
                        }
                    }

                    var dateTimeOffset = new DateTimeOffset(parsedDateTime, offset);
                    var utcDateTime = dateTimeOffset.ToUniversalTime().DateTime;

                    var dateStamp = utcDateTime.ToString("yyyy:MM:dd");
                    var timeStamp = utcDateTime.ToString("HH:mm:ss");

                    tags.TryAdd("GPSDateStamp", dateStamp);
                    tags.TryAdd("GPSTimestamp", timeStamp);
                }
            }

            var isBatchSetSuccessfull = BatchSetExifTags(outputFileFullPath, tags);
            if (!isBatchSetSuccessfull)
            {
                _logger.Error("Error while batch set tags. To {DestinationGFile}", outputFileFullPath);
                return;
            }
        }
        catch (OutOfMemoryException ex)
        {
            _logger.Error(ex, "Error: The image is not a valid JPEG format or is corrupted.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access Denied: You may not have permission to write to the file or folder. {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Converts a decimal coordinate (latitude or longitude) into an array of Rational values
    /// suitable for EXIF GPS tags (degrees, minutes, seconds).
    /// </summary>
    /// <param name="coordinate">The decimal latitude or longitude.</param>
    /// <returns>An array of three Rational objects: [degrees, minutes, seconds].</returns>
    private static Rational[] ConvertDecimalToExifRational(double coordinate)
    {
        // Handle negative coordinates by taking absolute value, reference tag will handle direction.
        double absCoordinate = Math.Abs(coordinate);

        // Calculate degrees
        uint degrees = (uint)absCoordinate;
        double remaining = absCoordinate - degrees;

        // Calculate minutes
        double minutesDecimal = remaining * 60;
        uint minutes = (uint)minutesDecimal;
        remaining = minutesDecimal - minutes;

        // Calculate seconds
        double secondsDecimal = remaining * 60;
        // Scale seconds to avoid floating point issues and ensure precision in Rational
        // We'll store seconds as a rational like (seconds_integer * 1000) / 1000
        uint secondsNumerator = (uint)(secondsDecimal * 10000); // Using 10000 for good precision
        uint secondsDenominator = 10000;

        return
        [
            new Rational(degrees, 1),
            new Rational(minutes, 1),
            new Rational(secondsNumerator, secondsDenominator)
        ];
    }

    private void CloneExif(string sourceFileFullPath, string destinationFileFullPath, string outputFolderFullPath)
    {
        if (!File.Exists(sourceFileFullPath))
        {
            _logger.Error("Source file not found at {SourceFileFullPath}", sourceFileFullPath);
            return;
        }

        if (!File.Exists(destinationFileFullPath))
        {
            _logger.Error("Destination file not found at {DestinationFileFullPath}", destinationFileFullPath);
            return;
        }

        try
        {
            if (!Directory.Exists(outputFolderFullPath))
            {
                Directory.CreateDirectory(outputFolderFullPath);
            }

            var destinationFileName = Path.GetFileName(destinationFileFullPath);
            var outputFileFullPath = Path.Combine(outputFolderFullPath, destinationFileName);

            if (File.Exists(outputFileFullPath))
            {
                // TODO LA - Rename file
            }

            // Clone EXIF

            using var sourceImage = new MagickImage(sourceFileFullPath);
            var sourceExifProfile = sourceImage.GetExifProfile();

            if (sourceExifProfile == null)
            {
                _logger.Warning("No EXIF data found in source image: {SourceFileFullPath}", sourceFileFullPath);
            }
            else
            {
                var cloneAllExifParameters = false;

                // Load the destination image
                using var destinationImage = new MagickImage(destinationFileFullPath);

                if (!cloneAllExifParameters)
                {
                    //List<string> exifParametersToClone = ["ISO Speed Ratings", "Exposure Program", "Metering Mode", "35mm Equivalent", "GPS"];

                    var destinationExifProfile = destinationImage.GetExifProfile();
                    if (destinationExifProfile is not null)
                    {
                        // Clone All - Except some

                        // Copy additional parameters
                        var softwear = destinationExifProfile.GetValue(ExifTag.Software);
                        if (softwear is not null)
                        {
                            sourceExifProfile.SetValue(ExifTag.Software, softwear.Value);
                        }

                        var lensModel = destinationExifProfile.GetValue(ExifTag.LensModel);
                        if (lensModel is not null)
                        {
                            sourceExifProfile.SetValue(ExifTag.LensModel, lensModel.Value);
                            sourceExifProfile.SetValue(ExifTag.LensMake, "Panasonic");
                        }

                        // Add Artist and Copyright
                        sourceExifProfile.SetValue(ExifTag.Artist, "Alex Lazarenko");
                        //sourceExifProfile.SetValue(ExifTag.OwnerName, "Alex Lazarenko");
                        sourceExifProfile.SetValue(ExifTag.Copyright, "Alex Lazarenko LA.xdna@gmail.com");


                        sourceExifProfile.SetValue(ExifTag.XPTitle, [1, 0, 0, 1, 2]);
                        sourceExifProfile.SetValue(ExifTag.SubjectLocation, [2, 1, 2, 1, 2]);
                        //sourceExifProfile.SetValue(ExifTag.Country, [2, 1, 2, 1, 2]);
                        //sourceExifProfile.SetValue(ExifTag.State, [2, 1, 2, 1, 2]);
                        //sourceExifProfile.SetValue(ExifTag.City, [2, 1, 2, 1, 2]);


                        // GPS EXIF

                        // Example GPS Coordinates (e.g., Eiffel Tower in Paris)
                        double latitude = 33.10704045077452;  // North Latitude
                        double longitude = -117.09718202245202;   // East Longitude
                        double? altitude = 137.0;    // Altitude in meters (Eiffel Tower height) (Optional)

                        sourceExifProfile.SetValue(ExifTag.GPSVersionID, [2, 3, 1, 0]);

                        // GPS Latitude Ref (ExifTag.GPSLatitudeRef, Type.Ascii, Length 2 for char + null terminator)
                        string latitudeRef = latitude >= 0 ? "N" : "S";  // 'N' for North, 'S' for South
                        sourceExifProfile.SetValue(ExifTag.GPSLatitudeRef, latitudeRef);

                        // GPS Latitude (ExifTag.GPSLatitude, Type.Rational, Length 24 for 3 Rationals)
                        // Degrees, Minutes, Seconds
                        var latitudeRationals = ConvertDecimalToExifRational(latitude);
                        sourceExifProfile.SetValue(ExifTag.GPSLatitude, latitudeRationals);


                        // GPS Longitude Ref (ExifTag.GPSLongitudeRef, Type.Ascii, Length 2 for char + null terminator)
                        // 'E' for East, 'W' for West
                        string longitudeRef = longitude >= 0 ? "E" : "W";
                        sourceExifProfile.SetValue(ExifTag.GPSLongitudeRef, longitudeRef);

                        // GPS Longitude (ExifTag.GPSLongitude, Type.Rational, Length 24 for 3 Rationals)
                        // Degrees, Minutes, Seconds
                        sourceExifProfile.SetValue(ExifTag.GPSLongitude, ConvertDecimalToExifRational(longitude));

                        if (altitude.HasValue)
                        {
                            // GPS Altitude Ref (ExifTag.GPSAltitudeRef, Type.Byte, Length 1)
                            // 0 for Above Sea Level, 1 for Below Sea Level
                            sourceExifProfile.SetValue(ExifTag.GPSAltitudeRef, altitude >= 0 ? (byte)0 : (byte)1);

                            // GPS Altitude (ExifTag.GPSAltitude, Type.Rational, Length 8 for 1 Rational)
                            // Value is meters. Store as a rational (e.g., meters * 100 / 100 for precision)
                            uint altNumerator = (uint)(Math.Abs(altitude.Value) * 100);
                            uint altDenominator = 100;
                            sourceExifProfile.SetValue(ExifTag.GPSAltitude, new Rational(altNumerator, altDenominator));
                        }


                        // Set GPS Date and Time (important for accurate geotagging)
                        // GPSDateStamp (ExifTag.GPSDateStamp, Type.Ascii) - UTC date "YYYY:MM:DD"
                        // GPS time should always be UTC
                        var dateTimeTaken = sourceExifProfile.GetValue(ExifTag.DateTime);
                        if (dateTimeTaken is not null)
                        {
                            var isParsed = DateTime.TryParseExact(
                                dateTimeTaken.Value,
                                "yyyy:MM:dd HH:mm:ss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out var parsedDateTime);

                            if (isParsed)
                            {
                                var offset = TimeSpan.FromHours(-8);

                                var offsetExif = sourceExifProfile.GetValue(ExifTag.OffsetTime);
                                if (offsetExif is not null)
                                {
                                    const string format = @"hh\:mm";
                                    var isOffsetParsed = TimeSpan.TryParseExact(offsetExif.Value, format, CultureInfo.InvariantCulture, out var offsetParsed);
                                    if (isOffsetParsed)
                                    {
                                        offset = offsetParsed;
                                    }
                                }

                                var dateTimeOffset = new DateTimeOffset(parsedDateTime, offset);
                                var utcDateTime = dateTimeOffset.ToUniversalTime().DateTime;

                                var dateStamp = utcDateTime.ToString("yyyy:MM:dd");
                                sourceExifProfile.SetValue(ExifTag.GPSDateStamp, dateStamp);

                                // GPSTimeStamp (ExifTag.GPSTimeStamp, Type.Rational) - UTC time (hours, minutes, seconds)
                                sourceExifProfile.SetValue(ExifTag.GPSTimestamp,
                                [
                                    new Rational((uint)utcDateTime.Hour, 1),
                                new Rational((uint)utcDateTime.Minute, 1),
                                new Rational((uint)utcDateTime.Second, 1)
                                ]);
                            }
                        }

                        //destinationImage.SetProfile(sourceExifProfile);
                    }

                    //var isoSpeedRating = sourceExifProfile.GetValue(ExifTag.ISOSpeedRatings);
                    //if (isoSpeedRating != null)
                    //{
                    //    destinationExifProfile.SetValue(ExifTag.ISOSpeedRatings, isoSpeedRating.Value);
                    //}

                    //var exposureProgram = sourceExifProfile.GetValue(ExifTag.ExposureProgram);
                    //if (exposureProgram != null)
                    //{
                    //    destinationExifProfile.SetValue(ExifTag.ExposureProgram, exposureProgram.Value);
                    //}

                    //var meteringMode = sourceExifProfile.GetValue(ExifTag.MeteringMode);
                    //if (meteringMode != null)
                    //{
                    //    destinationExifProfile.SetValue(ExifTag.MeteringMode, meteringMode.Value);
                    //}

                    //var thirtyFiveMmEquivalent = sourceExifProfile.GetValue(ExifTag.FocalLengthIn35mmFilm);
                    //if (thirtyFiveMmEquivalent != null)
                    //{
                    //    destinationExifProfile.SetValue(ExifTag.FocalLengthIn35mmFilm, thirtyFiveMmEquivalent.Value);
                    //}
                }

                // Remove any existing EXIF profile from the destination image
                destinationImage.RemoveProfile("exif");
                // Set the EXIF profile from the source to the destination image
                destinationImage.SetProfile(sourceExifProfile);

                // Save the modified destination image to output image
                var extension = Path.GetExtension(outputFileFullPath); // '.jpg'
                var magickFormat = extension.GetMagickFormatByFileExtension();
                destinationImage.Write(outputFileFullPath, magickFormat);
                _logger.Information("EXIF data successfully cloned from '{SourceFileFullPath}' to '{OutputFileFullPath}' using Magick.NET.", sourceFileFullPath, outputFileFullPath);

            }

            var isCloneTimestamp = false; // TODO LA - Use Parameter
            if (isCloneTimestamp)
            {
                // Clone Timestamp

                // Get the creation and modification times of the source file
                var sourceCreationTime = File.GetCreationTime(sourceFileFullPath);
                var sourceLastWriteTime = File.GetLastWriteTime(sourceFileFullPath);
                //var sourceLastAccessTime = File.GetLastAccessTime(sourceFileFullPath); // Often useful to clone this too

                // Set the creation and modification times of the destination file
                File.SetCreationTime(sourceFileFullPath, sourceCreationTime);
                File.SetLastWriteTime(sourceFileFullPath, sourceLastWriteTime);
                //File.SetLastAccessTime(destinationFilePath, sourceLastAccessTime); // Set last access time
            }
        }
        catch (MagickException ex)
        {
            _logger.Error(ex, "Magick.NET Error: {Message}", ex.Message);
        }
        catch (OutOfMemoryException ex)
        {
            _logger.Error(ex, "Error: The image is not a valid JPEG format or is corrupted.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access Denied: You may not have permission to write to the file or folder. {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An unexpected error occurred: {Message}", ex.Message);
        }
    }



    /// <summary>
    /// Executes the ExifTool command-line utility with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to exiftool.exe.</param>
    /// <returns>A tuple containing the standard output, standard error, and exit code.</returns>
    private (string output, string error, int exitCode) RunExifTool(string arguments)
    {
        using var process = new Process();

        process.StartInfo.FileName = ExifToolPath;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;       // Do not use OS shell to start process
        process.StartInfo.RedirectStandardOutput = true; // Capture stdout
        process.StartInfo.RedirectStandardError = true;  // Capture stderr
        process.StartInfo.CreateNoWindow = true;         // Do not show a console window for ExifTool

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (output.Trim(), error.Trim(), process.ExitCode);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error running ExifTool: {Message}", ex.Message);
            return (string.Empty, $"Exception: {ex.Message}", -1);
        }
    }

    /// <summary>
    /// Reads the value of a specific EXIF tag from a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="tagName">The name of the EXIF tag (e.g., "DateTimeOriginal", "GPSLatitude").
    /// Case-insensitive for ExifTool, but recommended to use standard capitalization.</param>
    /// <returns>The string value of the tag, or null if the tag is not found or an error occurs.</returns>
    public string ReadExifTag(string filePath, string tagName)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return null;
        }

        // -TAGNAME: Specify the tag to read.
        // -s3: Output only the value, stripped of tag name and groups.
        // -n: Output numerical values (e.g., GPS coordinates) as numbers, not formatted text.
        string arguments = $"-{tagName} -s3 -n \"{filePath}\"";

        Console.WriteLine($"\nReading tag '{tagName}' from '{Path.GetFileName(filePath)}'...");
        (string output, string error, int exitCode) result = RunExifTool(arguments);

        if (result.exitCode == 0)
        {
            // The output will contain the tag value, potentially with a newline. Trim it.
            string tagValue = result.output.Trim();
            Console.WriteLine($"'{tagName}': {tagValue}");
            return tagValue;
        }
        else
        {
            Console.WriteLine($"Failed to read tag '{tagName}'. ExifTool Exit Code: {result.exitCode}");
            if (!string.IsNullOrEmpty(result.output)) Console.WriteLine("ExifTool Output: " + result.output);
            if (!string.IsNullOrEmpty(result.error)) Console.WriteLine("ExifTool Error: " + result.error);
            return null;
        }
    }

    /// <summary>
    /// Sets the value of a specific EXIF tag in a single file.
    /// This method is primarily for single tag setting. For multiple tags/files, use BatchSetExifTags.
    /// </summary>
    /// <param name="filePath">The path to the file to modify.</param>
    /// <param name="tagName">The name of the EXIF tag (e.g., "Artist", "GPSLatitude").</param>
    /// <param name="value">The value to set the tag to. ExifTool handles quoting values,
    /// but internal quotes in the value string should be escaped for safety.</param>
    /// <returns>True if the tag was set successfully, false otherwise.</returns>
    public bool SetExifTag(string filePath, string tagName, string value)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return false;
        }

        // Escape internal double quotes in the value to prevent command-line parsing issues
        string escapedValue = value.Replace("\"", "\\\"");

        // -TAGNAME="VALUE": Sets the tag to the specified value.
        // -overwrite_original: Overwrites the original file instead of creating a backup.
        // -P: Preserves the original file's modification date/time (highly recommended).
        string arguments = $"-fast -m -q -{tagName}=\"{escapedValue}\" -overwrite_original -P \"{filePath}\"";

        Console.WriteLine($"\nSetting tag '{tagName}' to '{value}' in '{Path.GetFileName(filePath)}'...");
        (string output, string error, int exitCode) result = RunExifTool(arguments);

        if (result.exitCode == 0)
        {
            Console.WriteLine("Tag set successfully.");
            Console.WriteLine("ExifTool Output: " + result.output);
            return true;
        }
        else
        {
            Console.WriteLine($"Failed to set tag '{tagName}'. ExifTool Exit Code: {result.exitCode}");
            if (!string.IsNullOrEmpty(result.output)) Console.WriteLine("ExifTool Output: " + result.output);
            if (!string.IsNullOrEmpty(result.error)) Console.WriteLine("ExifTool Error: " + result.error);
            return false;
        }
    }

    /// <summary>
    /// Batch writes multiple EXIF tags with specified values to multiple files or an entire directory.
    /// </summary>
    /// <param name="paths">A list of file paths or a single directory path to process.
    /// If a directory is specified, ExifTool will process all supported files within it.</param>
    /// <param name="tagsToSet">A dictionary where key is the tag name and value is the tag value (as string).</param>
    /// <returns>True if the batch write was successful for all specified paths, false otherwise.</returns>
    public bool BatchSetExifTags(string filePath, Dictionary<string, string> tagsToSet)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return false;
        }

        if (tagsToSet == null || !tagsToSet.Any())
        {
            Console.WriteLine("No tags to set provided for batch tag setting.");
            return false;
        }

        // Build the tag arguments part for ExifTool
        StringBuilder tagArgs = new StringBuilder();
        tagArgs.Append("-m -q ");
        foreach (var tag in tagsToSet)
        {
            // Escape any double quotes within the value itself
            string escapedValue = tag.Value.Replace("\"", "\\\"");
            tagArgs.Append($"-{tag.Key}=\"{escapedValue}\" ");
        }

        // Build the file paths/directory arguments part
        StringBuilder fileArgs = new StringBuilder();
        fileArgs.Append($"\"{filePath}\" "); // Wrap each path in quotes for spaces

        if (fileArgs.Length == 0)
        {
            Console.WriteLine("No valid files or directories found to process for batch tag setting.");
            // If some paths were skipped but others were valid, consider returning false here
            // or modify the return logic based on your exact requirements (e.g., partial success).
            return false;
        }

        // Combine all arguments: tags, overwrite, preserve timestamp, and file/directory paths
        string arguments = $"{tagArgs.ToString().Trim()} -overwrite_original -P {fileArgs.ToString().Trim()}";

        (string output, string error, int exitCode) result = RunExifTool(arguments);

        // ExifTool exit code 0 means success.
        // Exit code 1 means some files were processed successfully, but some had warnings or errors.
        // It's often safer to consider anything other than 0 as a partial or full failure depending on strictness.
        if (result.exitCode == 0)
        {
            Console.WriteLine("Batch tags set successfully.");
            Console.WriteLine("ExifTool Output: " + result.output);
            return true;
        }
        else
        {
            Console.WriteLine($"Failed to batch set tags. ExifTool Exit Code: {result.exitCode}");
            if (!string.IsNullOrEmpty(result.output)) Console.WriteLine("ExifTool Output: " + result.output);
            if (!string.IsNullOrEmpty(result.error)) Console.WriteLine("ExifTool Error: " + result.error);
            // Return false if ExifTool did not exit cleanly (exit code != 0) or if no valid paths were provided initially.
            return false;
        }
    }

    /// <summary>
    /// Copies all EXIF tags (and other metadata like IPTC, XMP) from a source file to a destination file.
    /// The destination file's image data is NOT re-encoded, preserving image quality.
    /// </summary>
    /// <param name="sourceFilePath">The path to the file to copy tags from.</param>
    /// <param name="destinationFilePath">The path to the file to copy tags to.</param>
    /// <returns>True if tags were copied successfully, false otherwise.</returns>
    public bool CopyAllTags(string sourceFilePath, string destinationFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            Console.WriteLine($"Source file not found: {sourceFilePath}");
            return false;
        }
        if (!File.Exists(destinationFilePath))
        {
            Console.WriteLine($"Destination file not found: {destinationFilePath}");
            Console.WriteLine("Note: ExifTool will NOT create the destination file if it doesn't exist.");
            Console.WriteLine("Please ensure the destination file exists (e.g., by copying the source file first).");
            return false;
        }

        // -tagsFromFile: Copies all tags from the source file.
        // -all:all Copies all available tags as blocks (EXIF, IPTC, XMP, etc.).
        // -P: Preserves the original file's modification date/time of the destination file (important for consistency).
        string arguments = $"-fast -m -q -tagsFromFile \"{sourceFilePath}\" -all:all -P \"{destinationFilePath}\"";

        Console.WriteLine($"\nCopying all tags from '{Path.GetFileName(sourceFilePath)}' to '{Path.GetFileName(destinationFilePath)}'...");
        (string output, string error, int exitCode) result = RunExifTool(arguments);

        if (result.exitCode == 0)
        {
            Console.WriteLine("Tags copied successfully.");
            Console.WriteLine("ExifTool Output: " + result.output);
            return true;
        }
        else
        {
            Console.WriteLine($"Failed to copy tags. ExifTool Exit Code: {result.exitCode}");
            if (!string.IsNullOrEmpty(result.output)) Console.WriteLine("ExifTool Output: " + result.output);
            if (!string.IsNullOrEmpty(result.error)) Console.WriteLine("ExifTool Error: " + result.error);
            return false;
        }
    }
}