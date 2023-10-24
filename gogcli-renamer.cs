using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace gogcli_renamer
{
    internal class gogcli_renamer
    {
        static void Main(string[] args)
        {
            string manifestFile;
            string renameType;
            bool dryrun = false;
            string assemblyFileName = Path.GetFileName(Assembly.GetAssembly(typeof(gogcli_renamer)).Location);

            var manifestRegexPattern = new Regex(@".Id.: (\d+),\n.+.Slug.: .([a-z0-9_-]+).,", RegexOptions.Multiline);

            Dictionary<string, string> IdSlugPair = new Dictionary<string, string>();

            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    manifestFile = args[0];
                }
                else
                {
                    Console.WriteLine("Manifest filename must be first argument and be placed in same directory.");
                    Help(assemblyFileName);
                    return;
                }

                if (args.Length > 1 && (args[1] == "slug" || args[1] == "full" || args[1] == "fullrev"))
                {
                    renameType = args[1];
                } else
                {
                    Console.WriteLine("Wrong renameType argument.");
                    Help(assemblyFileName);
                    return;
                }

                if (args.Length > 2 && args[2] == "dryrun")
                {
                    dryrun = true;
                }
            }
            else
            {
                Help(assemblyFileName);
                return;
            }

            Console.WriteLine("Runing: {0} {1} {2} {3}", assemblyFileName, manifestFile, renameType, dryrun ? "dryrun" : "");
            var manifestData = File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + manifestFile, System.Text.Encoding.UTF8);

            MatchCollection matches = manifestRegexPattern.Matches(manifestData);

            foreach (Match m in matches.Cast<Match>())
                IdSlugPair.Add(m.Groups[1].Value, m.Groups[2].Value);

            Console.WriteLine("Found {0} matches", matches.Count);

            foreach (KeyValuePair<string, string> pair in IdSlugPair)
            {
                string newName = pair.Value;
                switch (renameType)
                {
                    case "slug":
                        newName = pair.Value;
                        break;
                    case "full":
                        newName = $"{pair.Key}-{pair.Value}";
                        break;
                    case "fullrev":
                        newName = $"{pair.Value}-{pair.Key}";
                        break;
                }

                Console.Write("Trying to rename '{0}' to '{1}' ... ", pair.Key, newName);
                if (dryrun)
                    Console.WriteLine("done (dryrun)");
                else
                {
                    if (Directory.Exists(pair.Key))
                    {
                        Directory.Move(pair.Key, newName);
                        Console.WriteLine("done");
                    }
                    else
                    {
                        Console.WriteLine("source directory missing.");
                    }
                }
            }

            Console.WriteLine("All done.");
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void Help(string assemblyFileName)
        {
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("    {0} fileName.json renameType dryrun", assemblyFileName);
            Console.WriteLine("Where: ");
            Console.WriteLine("    FileName.json enter manifest.json file name (in current directory)");
            Console.WriteLine("    renameType change rename type possible values: 'slug', 'full', 'fullrev'");

            Console.WriteLine("        'slug'   = just slug eg. 'fallout_tactics'");
            Console.WriteLine("        'full'   = ID-SLUG   eg. '3-fallout_tactics'");
            Console.WriteLine("        'fullrev = SLUG-ID   eg. 'fallout_tactics-3'");
            Console.WriteLine("    dryrun for dry run (no rename occurs) type 'dryrun' as last argument (optional)");
            Console.WriteLine("");
            Console.WriteLine("eg. {0} manifest.json slug", assemblyFileName);
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
