# NUX MG-30 AI Tone Generator

An AI-powered web application that generates guitar presets for the **NUX MG-30** multi-effects processor from natural-language descriptions.

The project combines a React/Vite frontend with an ASP.NET Core API and OpenAI structured outputs. API keys remain server-side and are never exposed to the browser.

![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=flat&logo=typescript&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?style=flat&logo=react&logoColor=61DAFB)
![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=flat&logo=dotnet&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=flat&logo=tailwind-css&logoColor=white)

## Features

- 🤖 **AI Tone Generation** - Describe a guitar tone in natural language and get a complete preset
- 🎛️ **Interactive Signal Chain Editor** - Visual signal chain with parameter controls for every effect
- 📦 **Equipment Catalog** - Complete catalog of MG-30 amplifiers, cabinets, and effects
- 💾 **Preset Library** - Save, load, and manage your generated presets
- 📤 **Export** - JSON export (verified), .mg30patch export (experimental)
- 🔍 **Reverse-Engineered Format** - Based on analysis of the actual .mg30patch binary format

## Architecture

```
src/
├── backend/
│   ├── NuxToneGenerator.Api/          # ASP.NET Core Web API
│   ├── NuxToneGenerator.Core/         # Domain models & interfaces
│   └── NuxToneGenerator.Infrastructure/ # Services & implementations
└── frontend/                          # React + TypeScript + Vite
```

### Backend Modules

| Module | Description |
|--------|-------------|
| **Equipment Catalog** | Verified MG-30 amp, cabinet, and effect models from official documentation |
| **AI Tone Generation** | OpenAI GPT-4o integration with structured outputs for preset generation |
| **Preset Validation** | Validates AI selections against the equipment catalog |
| **Patch Engine** | Binary serialization/deserialization of .mg30patch format |
| **Preset Library** | JSON-based preset storage and management |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 20+](https://nodejs.org/)
- [OpenAI API Key](https://platform.openai.com/)

## Quick Start

### 1. Configure the API Key

Store the key with .NET User Secrets so it stays outside the repository:

```bash
cd src/backend
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key"
```

For deployment, use the `OpenAI__ApiKey` environment variable instead. Never put a real key in `appsettings.json`, the frontend, or committed `.env` files.

### 2. Start the backend

```bash
cd src/backend
dotnet run --project NuxToneGenerator.Api
```

The API runs on `http://localhost:5177`.

### 3. Start the frontend

```bash
cd src/frontend
npm install
npm run dev
```

The frontend runs on `http://localhost:5173` with API proxy to the backend.

### 4. Open the app

Navigate to `http://localhost:5173` in your browser.

## Usage

### AI Chat
Type a description like:
- *"Create a David Gilmour-style Comfortably Numb solo tone with long sustain and atmospheric reverb"*
- *"Metallica Master of Puppets heavy rhythm with tight low end"*
- *"John Mayer clean blues tone with slight compression and spring reverb"*

The AI will select appropriate MG-30 equipment and configure all parameters.

### Signal Chain Editor
- Click any effect block to view and modify its parameters
- Toggle effects on/off with the enable switch
- Change the selected effect model from the dropdown
- All changes are reflected in the preset in real-time

### Export Options
- **JSON Export** - Download the preset configuration as JSON for reference
- **Manual Configuration** - Use the displayed settings to configure your MG-30 manually
- **.mg30patch Export** ⚠️ - Experimental binary export (not yet validated with QuickTone)

## Equipment Catalog

The catalog includes exact model names from MG-30 documentation:

- **29 Amplifier Models** (Fender, Marshall, Mesa, Vox, Soldano, Diezel, etc.)
- **8 Cabinet Simulations** (1x12, 2x12, 4x10, 4x12 configurations)
- **15 Overdrive/Distortion** effects
- **14 Modulation** effects  
- **8 Delay** types
- **6 Reverb** types
- **3 Compressors**
- **4 EQ** types
- **6 Wah/Volume** options

## Patch File Format

Key findings:
- 128 presets × 9,902 bytes = 1,267,456 bytes
- 3 scenes per preset (142 bytes each)
- Embedded 48kHz/32-bit WAV impulse response per preset
- Parameters stored as single bytes (0-100)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/tone/generate` | Generate preset from text prompt |
| GET | `/api/catalog` | Get equipment catalog |
| GET | `/api/presets` | List saved presets |
| POST | `/api/presets` | Save a preset |
| DELETE | `/api/presets/{id}` | Delete a preset |
| POST | `/api/patch/export` | Export as .mg30patch |
| POST | `/api/patch/export-json` | Export as JSON |
| POST | `/api/patch/import` | Import .mg30patch file |

## Development

Run the backend and frontend in separate terminals:

```bash
cd src/backend
dotnet test NuxToneGenerator.slnx
```

```bash
cd src/frontend
npm run build
```

### Project Structure

```
NuxToneGenerator.Core/
├── Models/          # Domain models (Preset, Scene, Effects)
└── Interfaces/      # Service contracts

NuxToneGenerator.Infrastructure/
├── Services/
│   ├── EquipmentCatalogService.cs   # Hard-coded MG-30 catalog
│   ├── ToneGenerationService.cs     # OpenAI integration
│   ├── PatchSerializer.cs           # Binary format handler
│   └── PresetLibraryService.cs      # JSON persistence
└── DependencyInjection.cs

NuxToneGenerator.Api/
├── Endpoints/       # Minimal API endpoint groups
└── Program.cs       # App configuration
```

## License

MIT
