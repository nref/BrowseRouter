
# Sponsor

BrowseRouter is sponsored by **[Linklever](https://linklever.net)**. 

Linklever is like BrowseRouter, but it's cross-platform and has a user interface. 

It's made by the same author, [@nref](https://github.com/nref/), so you can expect the same quality and support. 

**[Try Linklever today](https://linklever.net)** and save 25% with the code `BROWSEROUTER2025`

| Feature                             | BrowseRouter                          | Linklever                            |
|-------------------------------------|---------------------------------------|--------------------------------------|
| Registers as default browser        | ✅                                    | ✅                                  |
| Rules                               | ✅                                    | ✅                                  |
| Source Apps                         | ✅                                    | ✅                                  |
| Filters                             | ✅                                    | ✅                                  |
| Windows                             | ✅                                    | ✅                                  |
| macOS                               | ❌                                    | ✅                                  |
| Linux                               | ❌                                    | ✅                                  |
| Detects installed browsers          | ❌                                    | ✅                                  |
| GUI                                 | ❌                                    | ✅                                  |
| Browser extension                   | ❌                                    | ✅                                  |
| Configuration                       | via `config.json` and `filters.json`  | via GUI                            |

# BrowseRouter 

<p align="center">
    <img src="https://github.com/nref/BrowseRouter/blob/main/BrowseRouter/logo.png" width="256">
</p>

In Windows, launch a different browser depending on the url.

## Usage

```
    BrowseRouter.exe [-h | --help]
        Show help.

    BrowseRouter.exe
        Automatic registration. 
        Same as --register if not already registered, otherwise --unregister.
        If the app has moved or been renamed, updates the existing registration.

    BrowseRouter.exe [-r | --register]
        Register as a web browser, then open Settings. 
        The user must choose BrowseRouter as the default browser.
        No need to run as admin.

    BrowseRouter.exe [-u | --unregister]
        Unregister as a web browser. 

    BrowseRouter.exe https://example.org/ [...more URLs]
        Launch one or more URLs
```

## Setting Up

1. Download the latest release.
2. Open `config.json` and `filters.json` and customize as desired.
3. Run `BrowseRouter.exe` without arguments. No need to run as admin.
   It will register with Windows as a web browser and open the Settings app.
   To unregister, run it again.
   If you later move BrowseRouter to a new folder, or rename the exe, run it again to update the registration.
4. In the settings app, set `BrowseRouter` as the default browser.

<img width="600" alt="Settings" src="https://github.com/user-attachments/assets/a96a1715-498f-4830-b39d-8a3b98467d2d">

## Supported Platforms

- Windows 10, x64 and arm64
- Windows 11, x64 and arm64

## Why?

`BrowseRouter` becomes your default "browser". When you click a link, it decides which real browser to launch. If you have multiple browsers installed, this is very useful. Example use cases:

- *Compatibility*. Some sites you visit work better in specific browsers. You don't care which browser opens, just that the loaded page works.
- *Privacy*. For example, one browser is configured to use a proxy while another isn't. 
- *Workplace*. You access an intranet site through a specific browser while you prefer to use another browser for the rest of the internet. 
- *Browser wars*. You're tired of browsers jostling to be the default. You're tired of changing the default browser.

## Security

`BrowseRouter` is clean software. I, [@nref](https://github.com/nref/), rest my reputation on it.

- The code is open-source and publicly reviewable. 
- I provide pre-built binaries whichs are cryptographically signed with an Extended Validation code-signing certificate. What that means is I regularly navigate a lot of bureaucracy which keeps me legally accountable, and I pay DigiCert a lot of money to verify my identity and purpose. Moreover, you can know that the binaries are from me and not modified by some middleman.
- If you don't trust the the binaries, you can build them from source. 
- Windows Defender currently reports no problems, as of version 0.8.0.0
- A small number of malware scanners are reporting false positives as of version 0.8.0.0. I cannot do much about these as the scanners do not provide specific complaints.
  - [65/68 VirusTotal Report for x64](https://www.virustotal.com/gui/file/a83b01de823c3698af768d51903aa6d6cbbc04dc965e6626e748e877cdbb33a9)
  - [66/68 VirusTotal Report for arm64](https://www.virustotal.com/gui/file/498b0a33f6508613a2e8c2592586dccad78d83e434542e0191cf8d28c900acfb)
- Possible reasons a scanner may not like BrowseRouter:
  - It adds registry keys. It must in order to register as a browser. 
  - It launches other browsers.

## Privacy

BrowseRouter contains no tracking, and it makes no network connections of its own whatsoever. 

Your system administrator could know which pages you are visiting by auditing process start logs e.g. `BrowseRouter.exe http://some-naughty-site.example`. They would have the same information for any browser.

## Notifications

By default, `BrowseRouter` will show a desktop notification when it opens a link. You can disable this in `config.json`.

<img width="400" alt="Notification" src="https://github.com/user-attachments/assets/da647dc6-407e-4eda-81b9-bf258e82214b">

## Config

**Important: `config.ini` is deprecated. Use `config.json`**

For now, the app falls back to `config.ini` if `config.json` does not exist. This will be removed in a future version.

Example `config.json`:

```json
{
  "notify": {
    "enabled": true
  },
  "log": {
    "enabled": true,
    "comment": "Default file is C:\\Users\\<user>\\AppData\\Local\\BrowseRouter\\yyyy-MM-dd.log"
    "#file": "C:\\Users\\<user>\\Desktop\\BrowseRouter.log"
  },
  "browsers": {
    "ff": "%ProgramFiles%\\Mozilla Firefox\\firefox.exe",
    "chrome": "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
    "chromenw": "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe --new-window",
    "edge": "%ProgramFiles(x86)%\\Microsoft\\Edge\\Application\\msedge.exe",
    "opera": "%UserProfile%\\AppData\\Local\\Programs\\Opera\\opera.exe"
  },
  "sources": {
    "* - Notepad -> notepad": "chrome",
    "Slack | Test*": "chromenw",
    " -> AutoHotkey64": "ff"
  },
  "urls": {
    "*google.com": "chrome",
    "*microsoft.com": "edge",
    "*mozilla.org": "ff"
  },
  "filtersFile": "filters.json"
}
```

Example `config.ini` file (old, deprecated):

```ini
[notify]
# Show a desktop notification when opening a link. Defaults to true
enabled = true
# Should the windows notification sound be silenced when displaying it. Defaults to true
#silent = false

[log]
# Write log entries to a file. Defaults to false
enabled = true
# Default file is C:\Users\<user>\AppData\Local\BrowseRouter\yyyy-MM-dd.log
#file = "C:\Users\<user>\Desktop\BrowseRouter.log"

# Default browser is first in list
# Use `{url}` to specify UWP app browser details (not currently working, see following issue: https://github.com/nref/BrowseRouter/issues/10)
# Environment variables (like %ProgramFiles% for example) can be used 
[browsers]
ff = %ProgramFiles%\Mozilla Firefox\firefox.exe
# Open in a new window
chrome = C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
chromenw = "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --new-window
edge = %ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe
opera = %UserProfile%\AppData\Local\Programs\Opera\opera.exe

# Source preferences.
# - Only * is treated as a special character (wildcard).
# - Will take precedence over any URL preferences.
# - Matches on window title and specific process of the application used to open the link, like so "WindowTitle -> ProcessName".
[sources]
* - Notepad -> notepad = chrome
Slack | Test* = chromenw
# Source with no window (background processes)
 -> AutoHotkey64 = ff 
# Default case. Added automatically
# * = ff

# Url preferences.
# - Only * is treated as a special character (wildcard).
# - Only domains are matched. Don't include protocols e.g. "https://" or paths e.g. "/some/path?query=value"
# - Be aware that subdomains don't match automatically, e.g. "youtube.com = chrome" would not launch Chrome for "www.youtube.com"
#   For that reason, you'll often want a leading "*." e.g. "*.youtube.com". 
#   Note that "*youtube.com" would also match e.g. "notyoutube.com".
[urls]
*google.com = chrome
*microsoft.com = edge
*mozilla.org = ff

[filters]
file = filters.json
```

### Browsers

- Browsers must either be :
  - fully-qualified paths to the executable e.g. `C:\Program Files (x86)\Google\Chrome\Application\chrome.exe`.
  - full path to the executable with environment variable e.g. `%ProgramFiles(x86)%\Google\Chrome\Application\chrome.exe`.
  - in your PATH environment variable (you will just have to set the name of the .exe then) e.g. `chrome.exe` with `%ProgramFiles(x86)%\Google\Chrome\Application` added to the PATH variable.
- Arguments are optional. However, if you provide arguments the path _must_ be enclosed in quotes. For example, `"chrome.exe" --new-window`
- If there are no arguments, then the paths do not need to be quoted. For example, `chrome.exe` will work.

By default the URL to open is added as the last argument after the call to the executable.
But if you want it to be called differently, or only partially, you can use specific tags in the arguments you provide.
These tag will be replaced by their corresponding value in the URL :
- `{url}` the full, untruncated URL
- `{userinfo}` the userinfo part of the URL, might be blank if not present in the URL
- `{host}` the host of the URL, most often this will be a domain name (subdomain included)
- `{port}` the specific port of the URL, might be blank if not present in the URL
- `{authority}` the combination of userinfo, host and port separated by their respective delimiters if needed
- `{path}` the path of the URL, might be only `/` if the link targets the root of the domain
- `{query}` the query of the URL with the leading `?`, might be blank if not present in the URL
- `{fragment}` the fragment of the URL with the leading `#`, might be blank if not present in the URL

For example if you want a browser which strip the query from the opened links, you can add this line: 
`noQueryFF = "firefox.exe" "{authority}{path}{fragment}"`

[More details and example about URI composition is available here!](https://en.wikipedia.org/wiki/Uniform_Resource_Identifier#Example_URIs)

### Sources

- You can specify a "source preference" which matches the window title and the process name of the application used to open the link.
  - For example, with this in the previous example `config.json`:

    ```json
    "sources": {
      "* - Notepad -> notepad": "ff",
      "Slack | Test*": "chrome",
      " -> AutoHotkey64": "ff"
    }
    ```

    Then clicking a link in Notepad (end of the windows title ending with " - Notepad" with the process named "notepad") will open the link in Firefox, regardless of the URL.
    
- Wildcards and full regular expressions may be used to match source window titles and process name the same way urls are. [See Urls section](#Urls).

- Sources preferences takes precedence over all URLs preferences, so in the case of a conflict between a source preference and a URL preference, the source preference wins.

### Urls

There are two ways to specify an Url. You can use simple wildcards or full regular expressions.

**Simple wildcards:**

    "microsoft.com": "ie"
    "*.microsoft.com": "ie"

- Only `*` is treated as a special character in URL patterns, and matches any characters (equivalent to the `.*` regex syntax).
- Only the domain part (or IP address) of a URL is checked.
- There is no implied wildcard at the start or end, so you must include these if you need them, but be aware that "microsoft.\*" will not only match "microsoft.com" and "microsoft.co.uk" but also "microsoft.somethingelse.com".

**Full regular expressions:**

```regex
  "/sites\.google\.com/a/myproject.live\.com/": "chrome"
```
- Full regular expressions are specified by wrapping it in /'s.
- The domain _and_ path are used in the Url comparison.
- The regular expression syntax is based on the Microsoft .NET implementation.

## Filters

BrowseRouter can filter URLs before sending them to the browser. This is useful e.g. to remove tracking or deobfuscate URLs.

- If a URL is filtered, it says so in the notification and log.

  ![image](https://github.com/user-attachments/assets/f234089e-4b7c-4e60-a6f6-2813675614d5)

  ```2025-06-14 22:18:07 BrowseRouter: Filtered URL: example.org -> example.com```

- Filters are defined in `filters.json`. Filters consist of a Find regex pattern and Replace template pattern. In the case of multiple matches, `priority` determines which pattern is used. Note that `priority` is inverted, e.g. `1` is takes precedences over `2`.

```json
[
  {
    "name": "change org to com",
    "find": "(.*)org(.*)",
    "_comment": "Below, the replacement is the first capture group + com + second capture group"
    "replace": "$1com$2",
    "priority": 4
  }
]
```

- There is a special macro `unescape(...)` to un-urlescape obfuscated URLs before sending them to the browser. This is useful e.g. for Teams SafeLinks or Outlook URL Protection. It's not strictly necessary, since the browser unescapes the URL anyway, but it cleans up the notification.

```json
[
  {
    "name": "Bypass Teams Safelinks",
    "find": ".*teams\\.cdn\\.office\\.net.*url=([^&]+).*",
    "replace": "unescape($1)",
    "priority": 2
  }
]
```

See `filters.json` for more examples.

## Logs

Logs are stored by default in `%localappdata%/BrowseRouter/`. For example, if you user name is `joe`, then the logs will be in `C:\Users\joe\AppData\Local\BrowseRouter\`.

You can change the directory in the `log` section of `config.json`.

You can enable disable or log files by setting `"enabled": "true"` or `false` in the `log` section of `config.json`.
If `enabled` is missing or doesn't equal `true`, logs will not be written.

Log entries are also written to the console and can be seen if e.g. if launched from Command Prompt, PowerShell, or Windows Terminal.

## Credit

This is a fork of [BrowserSelector](https://github.com/DanTup/BrowserSelector/). That version is no longer mantained. This version carries on the vision, fixing bugs and adding new features.

## Support

If you like BrowseRouter, let me know in [a discussion](https://github.com/nref/BrowseRouter/discussions/new?category=general). BrowseRouter is just a hobby. You can help support continued development by "buying me a coffee."

<a href="https://www.buymeacoffee.com/slater1" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

You can help determine what happens next with BrowseRouter by filling out [this survey](https://forms.gle/Bh5z472CZUN6qdon9).
