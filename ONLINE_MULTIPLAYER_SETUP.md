# Online Multiplayer Setup Guide

This guide explains how to set up the online multiplayer feature for TicTacToe using **Photon PUN2** (Photon Unity Networking 2).

---

## Step 1: Install Photon PUN2

1. Open your project in **Unity Editor**
2. Go to **Window → Asset Store** (or open the Unity Asset Store in your browser)
3. Search for **"PUN 2 - FREE"** by Exit Games
4. Click **Download** and then **Import** into your project
5. When prompted, import all files

Alternatively, download from: https://assetstore.unity.com/packages/tools/network/pun-2-free-119922

## Step 2: Create a Photon Account & Get App ID

1. Go to https://www.photonengine.com/ and create a free account
2. In the Photon Dashboard, click **"Create a New App"**
3. Select **Photon Type: "PUN"**
4. Give it a name (e.g., "TicTacToe")
5. Copy the **App ID** that's generated

## Step 3: Configure Photon in Unity

1. After importing PUN2, a setup wizard should appear automatically
   - If not, go to **Window → Photon Unity Networking → PUN Wizard**
2. Paste your **App ID** into the field and click **Setup Project**
3. This creates a `PhotonServerSettings` asset in your project

Alternatively, go to `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings` and paste your App ID there.

## Step 4: Create the "OnlineLobby" Scene

1. In Unity, go to **File → New Scene**
2. Save it as **"OnlineLobby"** in your Scenes folder
3. Create an empty GameObject called **"NetworkManager"**
4. Add these components to it:
   - `NetworkManager` script (from Assets/Scripts/Network/)
   - `NetworkGameSync` script (from Assets/Scripts/Network/)
   - `PhotonView` component (from Photon)
5. Create another empty GameObject called **"LobbyUI"**
6. Add the `NetworkLobbyUI` script to it

## Step 5: Add Scene to Build Settings

1. Go to **File → Build Settings**
2. Make sure ALL scenes are added in this order:
   - Menu
   - Start
   - Gameplay
   - End
   - Scores
   - About
   - **OnlineLobby** ← Add this new scene!
3. Click **Add Open Scenes** if any are missing

## Step 6: Ensure NetworkManager Persists

The `NetworkManager` and `NetworkGameSync` objects use `DontDestroyOnLoad`, so they will persist across all scenes. However, you should only have them in the **OnlineLobby** scene (they'll carry over to Gameplay and End scenes automatically).

## Step 7: Build and Test

1. **Build** the project: File → Build Settings → Build
2. Run **two instances** of the game:
   - One on your computer
   - One on your friend's computer (or a second instance on the same machine for testing)
3. In both instances, click **"Online Multiplayer"** from the main menu
4. **Player 1 (Host):**
   - Enter a name
   - Click **"Create Room"**
   - Share the 5-character room code with your friend
5. **Player 2 (Guest):**
   - Enter a name
   - Click **"Join Room"**
   - Enter the room code
6. Once both players are connected, the host clicks **"START GAME!"**
7. Play! Host is X (goes first), Guest is O.

---

## How It Works

### Architecture
- **NetworkManager.cs** — Handles Photon server connection, room creation/joining with 5-character codes
- **NetworkGameSync.cs** — Syncs game moves between players using Photon RPCs (Remote Procedure Calls)
- **NetworkLobbyUI.cs** — The lobby interface for creating/joining rooms
- **Player.cs** (modified) — Detects online vs local mode; only allows input when it's your turn; sends moves over network
- **PlayerClass.cs** (modified) — Tracks last move coordinates so they can be sent to the remote player
- **UserInterface.cs** (modified) — Added "Online Multiplayer" button to main menu
- **EndScreen.cs** (modified) — Supports networked rematch functionality

### Game Flow (Online)
1. Main Menu → "Online Multiplayer" → OnlineLobby scene
2. Connect to Photon cloud servers
3. Host creates room (gets 5-char code) / Guest joins room (enters code)
4. Host clicks Start → both clients load Gameplay scene
5. Host plays as X (first turn), Guest plays as O
6. Each move is sent via RPC to the other player
7. Win/draw detection happens locally on both clients
8. End screen shows rematch option (both must agree)

### Files Changed
| File | Change |
|------|--------|
| `Assets/Scripts/Player.cs` | Added online mode with `UpdateOnline()`, network event handling |
| `Assets/Scripts/PlayerClass.cs` | Added `lastMoveX`/`lastMoveZ` tracking and getter methods |
| `Assets/Scripts/UserInterface.cs` | Added "Online Multiplayer" button, renamed "Start" to "Local Play" |
| `Assets/Scripts/EndScreen.cs` | Added networked rematch with mutual agreement |

### New Files
| File | Purpose |
|------|---------|
| `Assets/Scripts/Network/NetworkManager.cs` | Photon connection & room management |
| `Assets/Scripts/Network/NetworkGameSync.cs` | Move synchronization via RPCs |
| `Assets/Scripts/Network/NetworkLobbyUI.cs` | Online lobby GUI |

---

## Troubleshooting

### "Not connected to server"
- Check your internet connection
- Verify your Photon App ID is correctly set in PhotonServerSettings
- Photon free tier allows 20 concurrent users

### "Room not found"
- Make sure the host has created the room first
- Room codes are case-insensitive (auto-converted to uppercase)
- Room codes are 5 characters (letters and numbers, no I/O/0/1)

### Moves not syncing
- Ensure the NetworkManager GameObject has a `PhotonView` component
- Make sure `NetworkGameSync` is on the same GameObject as the `PhotonView`
- Check the Unity console for any RPC errors

### "Application.LoadLevel is obsolete" warnings
- The original project uses `Application.LoadLevel()` which is deprecated in newer Unity versions
- If using Unity 5.3+, you can replace with `UnityEngine.SceneManagement.SceneManager.LoadScene()`
- The code will still work with the deprecated calls, just with warnings

---

## Free Tier Limits

Photon PUN2 Free plan includes:
- **20 CCU** (Concurrent Users) — plenty for personal/friend games
- Unlimited messages
- Hosted globally (auto-selects best server region)
- No credit card required

For more info: https://www.photonengine.com/pun
