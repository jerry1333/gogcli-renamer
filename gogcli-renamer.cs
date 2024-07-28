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
            string version = "1.1";

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
                    Help(assemblyFileName, version);
                    return;
                }

                if (args.Length > 1 && (args[1] == "slug" || args[1] == "slug-id" || args[1] == "id-slug" || args[1] == "id"))
                {
                    renameType = args[1];
                } else
                {
                    Console.WriteLine("Wrong renameType argument.");
                    Help(assemblyFileName, version);
                    return;
                }

                if (args.Length > 2 && args[2] == "dryrun")
                {
                    dryrun = true;
                }
            }
            else
            {
                Help(assemblyFileName, version);
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
                string sourceDir;

                switch (renameType)
                {
                    case "slug":
                        newName = pair.Value;
                        break;
                    case "id-slug":
                        newName = $"{pair.Key}-{pair.Value}";
                        break;
                    case "slug-id":
                        newName = $"{pair.Value}-{pair.Key}";
                        break;
					case "id":
						newName = $"{pair.Key}";
						break;
                }
                Console.WriteLine("Working on entry '{0}'", pair.Key);
                
                // try all folder names 
                if (Directory.Exists(pair.Key))
                    // id
                    sourceDir = pair.Key;
                else if (Directory.Exists($"{pair.Value}"))
                    // slug
                    sourceDir = $"{pair.Value}";
                else if (Directory.Exists($"{pair.Key}-{pair.Value}"))
                    // id-slug
                    sourceDir = $"{pair.Key}-{pair.Value}";
                else if (Directory.Exists($"{pair.Value}-{pair.Key}"))
                    // slug-id
                    sourceDir = $"{pair.Value}-{pair.Key}";
                else
                {
                    Console.WriteLine("  Cant find source directory for entry '{0}' '{1}'. Skipping!", pair.Key, pair.Value);
                    Console.WriteLine("");
                    continue;
                }
                Console.WriteLine("  Source directory found '{0}'", sourceDir);
                Console.Write("  Trying rename '{0}' to '{1}' ... ", sourceDir, newName);

				if (!dryrun)
				{
                    try
                    {
                        Directory.Move(sourceDir, newName);
                        Console.WriteLine("done");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e}");
                        Console.WriteLine("Skipping!");
                        continue;
                    }
				} else 
					Console.WriteLine("done (dryrun)");
                Console.WriteLine("");
            }

            Console.WriteLine("All done.");
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void Help(string assemblyFileName, string version)
        {
            Console.WriteLine("{0} {1}", assemblyFileName, version);
            Console.WriteLine("Usage:");
            Console.WriteLine("    {0} fileName.json renameType dryrun", assemblyFileName);
            Console.WriteLine("Where: ");
            Console.WriteLine("    FileName.json enter manifest.json file name (in current directory)");
            Console.WriteLine("    renameType change rename type possible values: 'slug', 'id-slug', 'slug-id', 'id'");
            Console.WriteLine("        'slug'    = just slug eg. 'fallout_tactics'");
            Console.WriteLine("        'id-slug' = ID-SLUG   eg. '3-fallout_tactics'");
            Console.WriteLine("        'slug-id' = SLUG-ID   eg. 'fallout_tactics-3'");
			Console.WriteLine("        'id'      = just ID   eg. '3'");
            Console.WriteLine("    dryrun for dry run (no rename occurs) type 'dryrun' as last argument (optional)");
            Console.WriteLine("");
            Console.WriteLine("eg. {0} manifest.json slug", assemblyFileName);
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
