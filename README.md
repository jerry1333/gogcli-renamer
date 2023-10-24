This tool was created to rename game directories created from gogcli tool (https://github.com/Magnitus-/gogcli).

Place exe inside your library folder (where manifest.json and game directories are) and follow below instructions.

```
Usage:
    gogcli-renamer.exe fileName.json renameType dryrun
Where:
    FileName.json enter manifest.json file name (in current directory)
    renameType change rename type possible values: 'slug', 'full', 'fullrev'
        'slug'   = just slug eg. 'fallout_tactics'
        'full'   = ID-SLUG   eg. '3-fallout_tactics'
        'fullrev = SLUG-ID   eg. 'fallout_tactics-3'
    dryrun for dry run (no rename occurs) type 'dryrun' as last argument (optional)

eg. gogcli-renamer.exe manifest.json slug
```
