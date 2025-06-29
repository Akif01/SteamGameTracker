# 🎮 Steam Game Tracker

Steam Game Tracker is a Blazor application designed to track and display featured Steam games, retrieve current player statistics, and present game information using the Steam Web API. It implements caching and clean architecture practices to ensure performant and maintainable code.

## ✨ Features

- 🔍 View **featured games** from Steam
- 📊 Track **current number of players** per game
- 📦 Retrieve **game details** (description, genres, price, etc.)
- 🚀 Blazor UI with component-based structure
- ⚡ Redis-like **distributed caching** to minimize API calls
- 🔧 Modular and testable service-based architecture

---

## 🧪 Running the Project with 🐳 Docker
### Prerequisites
- [Docker](https://docs.docker.com/get-docker/)

### Steps

1. Clone the repo:

   ```bash
   git clone https://github.com/Akif01/SteamGameTracker.git
   cd SteamGameTracker
   
2. Start all services
   
   ```bash
   cd SteamGameTracker
   docker-compose up --build
   
4. Open your browser
   
   App: http://localhost
   
   Redis UI: http://localhost:8081
