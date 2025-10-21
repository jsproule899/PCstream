# 🎬 ASP.NET MVC Home Media Streaming App

A full-stack **ASP.NET MVC** application for streaming video content from your local PC over a LAN network.

## 🧑‍🍳 Features Overview

- **Automatic Library Scan** – Detects and indexes video files from your local library.  
- **Smart Title Parsing** – Extracts movie and TV show titles from filenames.  
- **TMDB Integration** – Uses the TMDB API to fetch metadata such as posters, descriptions, and ratings.  
- **SQLite Database Storage** – Saves all movie and show details locally for fast access.  
- **Intuitive Web UI** – Responsive, searchable interface with poster cards, star ratings, and TV show organization by season.  
- **Custom Video Player** – Streams content efficiently by sending video data in chunks for smooth playback.  
- **Watch Progress Tracking** – Saves the current playback position in local storage.  
- **Subtitle Support** – Automatically loads and displays available subtitle files.  

## 🧰 Tech Stack

- **Backend:** ASP.NET MVC (C#), TMDB API  
- **Database:** SQLite  
- **Frontend:** HTML5, CSS, JavaScript  
- **Video Player:** Custom-built chunked streaming player  

## 🚀 Future Enhancements (Optional)

- User authentication  
- Remote access outside LAN  
- Multiple user libraries  
- Enhanced subtitle and metadata editing  

## 📁 .env Setup

Create a `.env` file in the project root:

```bash
TMDB_API_KEY=<your-api-key>
```

## 🛠️ Build the app

Initialise Database

```bash
dotnet ef database update
```

Build
```bash
dotnet build
```
Run

```bash
dotnet run
```

Publish

```bash
dotnet publish -c Release -o ./publish
```
