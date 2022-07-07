# Browser Selector 

In Windows, launch a different browser depending on the url.

## Credit

This is a fork of [BrowseSelector](https://github.com/DanTup/BrowserSelector/). That version is no longer mantained, so I ported to .net6.0. 

## Setting Up

1. Clone and extract to a folder somewhere on your PC.
2. Open `BrowserSelector.ini` and customize paths to your browsers and domain patterns (see below).
3. Run `BrowserSelector.exe --register` from this folder to register the tool in Windows as a web browser.
4. Open the "Choose a default browser" screen in Windows (you can simply search for "default browser" from the start screen).
5. Select `BrowserSelector` as the default browser.

Tested on:

- Windows 10 Pro

## Usage

```
    BrowserSelector.exe --register
        Register as a web browser.

    BrowserSelector.exe --unregister
        Unregister as a web browser.
        Once you have registered the app as a browser, you should use visit "Set Default Browser" in Windows to set this app as the default browser.

    BrowserSelector.exe http://example.org/
        Launch a URL
```

## Config

Config is a poor mans INI file:

```ini
; Default browser is first in list
; Use `{url}` to specify UWP app browser details
[browsers]
chrome = C:\Program Files (x86)\Google\Chrome\Application\chrome.exe
ff = C:\Program Files (x86)\Mozilla Firefox\firefox.exe
edge = microsoft-edge:{url}
ie = iexplore.exe

; Url preferences.
; Only * is treated as a special character (wildcard).
; Matches are domain-only. Protocols and paths are ignored.
; Use "*.blah.com" for subdomains, not "*blah.com" as that would also match "abcblah.com".
[urls]
microsoft.com = ie
*.microsoft.com = ie

google.com = chrome
visualstudio.com = edge
```

### Browsers

- Browser exes must be exact paths to the browser executable.
- Arguments are optional. However, if you provide arguments the exe _must_ be enclosed in quotes.
- If there are no arguments, then the exe paths do not need to be quoted.

**Special cases:**

- For special browsers, you can include the `{url}` flag. This allows better control over the browser command-line arguments.
- This is required when specifying UWP app's such as Microsoft Edge.
- By default, the url is used as an argument when launching the exe. If the `{url}` flag is specified, it will not be added to the arguments. (In other words, it _won't_ be added twice..)

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
