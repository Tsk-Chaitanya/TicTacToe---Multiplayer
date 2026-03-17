# 🎮 TicTacToe — Multiplayer

A 5×5 Tic Tac Toe game with **local play**, **AI opponents**, and **online multiplayer** via room codes. Built in Unity 6 with Photon PUN2.

> **Get four in a row** — horizontally, vertically, or diagonally — to win!

---

## 🎯 How to Play

- The board is **5×5**. The first player to place **4 pieces in a row** wins.
- Players take turns clicking an empty board space to place their piece.
- On an AI's turn, click anywhere on the board to make the AI move.

---

## 🌐 Playing Online Multiplayer

### What your friend needs

Your friend **does not need Unity or anything special installed**. You just need to share the built game files:

1. In Unity, go to **File → Build and Run** (or **Build Settings → Build**)
2. This creates a folder with:
   - `3D_TicTacToe.exe` — the game executable
   - `3D_TicTacToe_Data/` — required game data folder
3. **Zip both** `3D_TicTacToe.exe` and `3D_TicTacToe_Data/` together and send the zip to your friend
4. Your friend extracts the zip, keeps both items **in the same folder**, and double-clicks `3D_TicTacToe.exe` to play

> ⚠️ Both files must stay together — the `.exe` won't work without the `_Data` folder next to it.

---

### Step-by-step: Starting an online game

#### Host (you — the one creating the room):

1. Launch the game and click **Online Multiplayer** from the main menu
2. Enter your **display name** and click **Continue**
3. Check that it says **Server: asia** under your name — both players must be on the same server
4. Click **Create Room**
5. A **5-character room code** will appear on screen (e.g. `K7MNP`)
6. Share this code with your friend via text/chat/etc.
7. Wait for your friend to join — you'll see "Opponent Found!" when they connect
8. Click **START GAME!** to begin

#### Guest (your friend — the one joining):

1. Launch the game and click **Online Multiplayer**
2. Enter your **display name** and click **Continue**
3. Check that it says **Server: asia** under your name — must match the host's server
4. Click **Join Room**
5. Type in the **5-character room code** your friend shared
6. Click **Join →**
7. Wait for the host to start the game

---

### Troubleshooting: "Room not found"

If you see "Room not found" when trying to join, check these things in order:

- **Server mismatch** — Both players must show the same server (e.g. `asia`) on the lobby screen. If they differ, the rooms are invisible to each other.
- **Wrong code** — Double-check the room code. It's case-insensitive but must be exactly 5 characters. Avoid confusing characters — the game uses only `A-Z` and `2-9` (no `I`, `O`, `0`, `1`).
- **Host navigated away** — If the host pressed Back or quit the lobby, the room is destroyed. The host must stay on the "Waiting for Opponent" screen until the guest joins.
- **Old build** — Make sure both players are running the same (latest) version of the game.

---

### After the game ends

- **Rematch?** — Both players can click "Rematch?" to play again. Once both click it, a new game starts automatically.
- **Leave Game** — Returns you to the main menu and disconnects from the room.

---

## 🤖 AI Opponents

You can play against AI instead of a human. Three difficulty levels are available:

| Difficulty | Behavior |
|------------|----------|
| **Easy**   | Plays randomly — good for beginners |
| **Medium** | Mixes random and strategic moves |
| **Hard**   | Uses minimax-based strategy — tough to beat |

---

## 📁 Project Structure

```
TicTacToe-master/
├── Assets/
│   ├── Scripts/
│   │   ├── Network/
│   │   │   ├── NetworkManager.cs       # Photon connection & room management
│   │   │   ├── NetworkGameSync.cs      # RPC-based move synchronization
│   │   │   └── NetworkLobbyUI.cs       # Online lobby UI screens
│   │   ├── UIStyles.cs                 # Shared pastel UI styles
│   │   ├── UserInterface.cs            # Main menu & player selection
│   │   ├── EndScreen.cs                # End game / rematch screen
│   │   ├── Player.cs                   # Player logic & online turn handling
│   │   ├── GameInfo.cs                 # Turn tracking & win detection
│   │   ├── History.cs                  # Player profile & win/loss records
│   │   └── ...
│   ├── Scenes/
│   │   ├── Menu                        # Main menu
│   │   ├── OnlineLobby                 # Online multiplayer lobby
│   │   ├── Gameplay                    # The actual game board
│   │   └── End                         # End screen
│   └── Photon/                         # Photon PUN2 SDK (not included in repo)
├── .gitignore
└── README.md
```

---

## 🛠 Developer Setup

### Requirements

- **Unity 6** (6000.x) — [Download Unity Hub](https://unity.com/download)
- **Photon PUN2** — Free tier supports up to 20 concurrent users

### First-time setup

1. Clone this repository
2. Open the project in Unity 6 via Unity Hub
3. Import **Photon PUN2** from the Unity Asset Store (free)
   - Delete the following folders after import (they cause compile errors):
     - `Assets/Photon/PhotonUnityNetworking/UtilityScripts`
     - `Assets/Photon/PhotonUnityNetworking/Demos`
     - `Assets/Photon/PhotonChat`
     - `Assets/Photon/PhotonRealtime/Demos`
4. Create a free account at [dashboard.photonengine.com](https://dashboard.photonengine.com)
5. Create a new **Realtime** app and copy your **App ID**
6. In Unity: go to `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings`
7. Paste your App ID into the **App Id Realtime** field and save

### Build & Run

1. Open **File → Build Settings**
2. Make sure all scenes are added in this order:
   - Menu, OnlineLobby, Gameplay, End, (+ any AI variant scenes)
3. Click **Build and Run**

---

## 🎨 Credits

Original game design by Robert Rabel & Sean Cary (AI logic) — 2014 college project.
Upgraded to Unity 6 with online multiplayer, pastel UI overhaul, and Photon PUN2 integration.
