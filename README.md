# StreamFinder

StreamFinder is a personal Windows desktop application developed in C# using WinForms and .NET Framework 4.8.

It allows users to search for movies and TV shows, view enriched details and availability, save favorites, maintain a local search history, and configure streaming services.

Movie and TV metadata is retrieved using the TMDb API.

This project is intended for personal, educational, and non-commercial use.

## Features

* Search for movies and TV shows
* Display title, original title, release year, rating, genres, overview and poster
* Filter between movies and TV shows
* Filter by year, genre and enabled services
* Sort results by title, year or rating
* Load additional search pages
* Save favorites locally
* Local search history
* Configurable streaming services
* Local INI configuration
* TMDb streaming availability by configured country
* Optional conservative public-page fallback for Pluto TV

## Technologies

* C#
* Windows Forms
* .NET Framework 4.8
* TMDb API
* Newtonsoft.Json
* System.Net.Http

## First run and configuration

Run `StreamFinder.WinForms.exe` and open **Configurações**. Enter your own TMDb API key, choose the country and save. The application creates `config.ini` next to the executable when it does not exist. You may also copy `config.example.ini` to that location before the first run.

Then configure your own TMDb API key under `[Api]`:

```ini
TmdbApiKey=YOUR_API_KEY
```

Do not commit your API keys to the repository. The local `config.ini` file is ignored by Git.

The default country is `BR`. Provider checkboxes control local filtering only; enabling a service does not create availability for a title. Favorites, history and cache are stored locally and are not part of a distribution package.

The TMDb API is the primary availability source. Optional public-page fallback is controlled by `EnableScraping` and currently supports only Pluto TV conservatively. Pages that require JavaScript may produce no detection. The application does not bypass login, CAPTCHA or anti-bot protections.

## Building

Open `StreamFinder.sln` in Visual Studio and build using `Debug | Any CPU` or `Release | Any CPU`.

The project targets .NET Framework 4.8.

## TMDb

This product uses the TMDb API but is not endorsed or certified by TMDb.

## Project Status

StreamFinder is currently under development.

Implemented:

* TMDb search
* Media cards
* Poster loading
* Favorites
* Search history
* Local INI settings and cache

* Detailed media screen
* TMDb streaming providers
* Local filters and paginated search
* Optional Pluto TV public-page fallback

The YouTube API key field is reserved for a future integration; this application does not currently provide YouTube Data API functionality.

## Manual distribution

For a clean manual distribution, copy the Release output files required by the executable, including:

* `StreamFinder.WinForms.exe`
* `StreamFinder.WinForms.exe.config`, when generated
* `Newtonsoft.Json.dll`
* any other assembly present in the Release output that is required by the build

Do not distribute `config.ini`, `favorites.json`, `history.json`, `Cache`, `.vs`, `obj`, `.git`, `.git.corrompido.backup`, temporary files, logs, or API keys. Configure the application after copying the Release files.

## License

This project is currently intended for personal and educational use.
