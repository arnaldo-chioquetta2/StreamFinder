# StreamFinder

StreamFinder is a personal Windows desktop application developed in C# using WinForms and .NET Framework 4.8.

It allows users to search for movies and TV shows, view basic information, save favorites, maintain a local search history, and configure streaming services.

Movie and TV metadata is retrieved using the TMDb API.

This project is intended for personal, educational, and non-commercial use.

## Features

* Search for movies and TV shows
* Display title, release year, rating and poster
* Filter between movies and TV shows
* Save favorites locally
* Local search history
* Configurable streaming services
* Local INI configuration
* Planned local cache
* Planned streaming provider availability
* Planned YouTube integration
* Planned fallback scraping for public pages where appropriate

## Technologies

* C#
* Windows Forms
* .NET Framework 4.8
* TMDb API
* Newtonsoft.Json
* System.Net.Http

## Configuration

Copy `config.example.ini` to `config.ini` next to the application executable.

Then configure your own TMDb API key under `[Api]`:

```ini
TmdbApiKey=YOUR_API_KEY
```

Do not commit your API keys to the repository. The local `config.ini` file is ignored by Git.

## Building

Open `StreamFinder.sln` in Visual Studio and build the `StreamFinder.WinForms` project using Debug or Release configuration.

The project targets .NET Framework 4.8.

## TMDb

This product uses the TMDb API but is not endorsed or certified by TMDb.

## Project Status

StreamFinder is currently under development.

Implemented so far:

* TMDb search
* Media cards
* Poster loading
* Favorites
* Search history
* Local INI settings

Planned:

* Local cache improvements and management controls
* Detailed media screen
* Streaming providers
* YouTube integration
* Public-page fallback scraping where appropriate

## License

This project is currently intended for personal and educational use.
