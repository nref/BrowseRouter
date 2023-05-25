# BrowseRouter 

In Windows, launch a different browser depending on the url.

## Support

If you like BrowseRouter, let me know in [a discussion](https://github.com/slater1/BrowseRouter/discussions/new?category=general). I work a full time job. BrowseRouter is just a hobby. You can help support continued development by "buying me a coffee."

<a href="https://www.buymeacoffee.com/slater1" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>

## Why?

`BrowseRouter` becomes your default "browser". When you click a link, it decides which real browser to launch. If you have multiple browsers installed, this is very useful. Example use cases:

- *Workplace*. You access an intranet site through a specific browser while you prefer to use another browser for the rest of the internet. 
- *Proxies*. One browser is configured to use a proxy while another isn't. For example, my employer is in another country. It's too slow to proxy all of my web traffic through their network. Some sites load in the wrong language and currency because they ignore system locale settings and localize based on IP address. Even worse, some sites or pages are blocked.
- *Compatibility*. Some sites you visit work better in specific browsers. You don't care which browser opens, just that the loaded page works.
- *Browser wars*. You're tired of browsers jostling to be the default. You're tired of changing the default browser to accomodate different workflows.

## Security

`BrowseRouter` does no tracking and makes no network connections of its own whatsoever. 

Your system administrator could know which pages you are visiting by auditing process start logs e.g. `BrowseRouter.exe http://some-naughty-site.com`. They would have the same information for any browser.

## Credit

This is a fork of [BrowseSelector](https://github.com/DanTup/BrowserSelector/). That version is no longer mantained, so I ported to .net6.0. 

## Setting Up

1. Clone and extract to a folder somewhere on your PC.
2. Open `config.ini` and customize paths to your browsers and domain patterns (see below).
3. *As administrator*, Run `BrowseRouter.exe --register` from this folder to register the tool in Windows as a web browser.
4. (Optional) Open default browser chooser: from the Start Menu, search for "default browser" or "default apps". Select `BrowseRouter` as the default browser.
  - ![image](https://user-images.githubusercontent.com/2089725/180062311-1393df79-7ba1-4fa8-b280-5643c0dcdd50.png)
5. If you skipped step 4, the first time you click on a link after registring BrowseRouter as a browser, you will be presented with the Open With dialog.
  - ![Open With Dialog](OpenWith.png)

Tested on:

- Windows 10 Pro

## Usage

```
    BrowseRouter.exe --register
        Register as a web browser.

    BrowseRouter.exe --unregister
        Unregister as a web browser.
        Once you have registered the app as a browser, you should use visit "Set Default Browser" in Windows to set this app as the default browser.

    BrowseRouter.exe http://example.org/
        Launch a URL
```

## Config

Config is a poor man's INI file:

```ini
; Default browser is first in list
; Use `{url}` to specify UWP app browser details
[browsers]
ff = C:\Program Files\Mozilla Firefox\firefox.exe
# Open in a new window
#chrome = "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --new-window
chrome = C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
edge = C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe

; Url preferences.
; - Only * is treated as a special character (wildcard).
; - Only domains are matched. Don't include protocols e.g. "https://" or paths e.g. "/some/path?query=value"
; - Beware that subdomains don't match automatically, e.g. "youtube.com = chrome" would not launch Chrome for "www.youtube.com"
;   For that reason, you'll often want a leading "*." e.g. "*.youtube.com". 
;   Note: Don't use "*youtube.com" as that would also match e.g. "notyoutube.com".
[urls]
*.google.com = chrome
*.youtube.com = chrome
*.visualstudio.com = edge
*.mozilla.org = ff

; Source preferences.
; Only * is treated as a special character (wildcard).
; Matches on window title of application used to open link.
; Applied only if no url preference match is found.
[sources]
* - Notepad = ff
Slack | Test = chrome
; Default case. Added automatically
; * = whatever
```

### Browsers

- Browsers must either be in your path or be fully-qualified paths to the executable e.g. `C:\Program Files (x86)\Google\Chrome\Application\chrome.exe`.
- Arguments are optional. However, if you provide arguments the path _must_ be enclosed in quotes. For example, `"chrome.exe" --new-window`
- If there are no arguments, then the paths do not need to be quoted. For example, `chrome.exe` will work.

## Sources

- You can optionally specify a "source preference" which matches the window title of the application used to open the link.
  - For example, with this in the previous example `config.ini`:

    ```ini
    [sources]
    *Microsoft Teams* = ff
    ```

    Then clicking a link in Microsoft Teams will open the link in Firefox, regardless of the URL.

- In the case of a conflict between a source preference and a URL preference, the source preference wins.

### Urls

There are two ways to specify an Url. You can use simple wildcards or full regular expressions.

**Simple wildcards:**

    microsoft.com = ie
    *.microsoft.com = ie

- Only `*` is treated as a special character in URL patterns, and matches any characters (equivalent to the `.*` regex syntax).
- Only the domain part (or IP address) of a URL is checked.
- There is no implied wildcard at the start or end, so you must include these if you need them, but be aware that "microsoft.\*" will not only match "microsoft.com" and "microsoft.co.uk" but also "microsoft.somethingelse.com".

**Full regular expressions:**

```regex
  /sites\.google\.com/a/myproject.live\.com/ = chrome
```
- Full regular expressions are specified by wrapping it in /'s.
- The domain _and_ path are used in the Url comparison.
- The regular expression syntax is based on the Microsoft .NET implementation.

### Sources

Wildcares and full regular expressions may also be used to match source window titles.
