using System;
using Microsoft.Xna.Framework.Graphics;
using Tetris.GUI;
using Tetris.Main;
using Tetris.Main.Player;
using Tetris.Multiplayer;
using Tetris.Multiplayer.Network;
using Tetris.Settings;
using Tetris.Sound;

namespace Tetris.Other
{
    public class Instance
    {
        private static GameGlobal _gameGlobal;
        private static Random _rand;
        private static TetrisPlayer _ply;
        private static Movement _movement;
        private static NextShape _nextShape;
        private static Sfx _sfx;
        private static ScoreHandler _scoreHandler;
        private static HoldShape _holdShape;
        private static RowCheck _rowCheck;
        private static Rotate _rotate;
        private static RotateCheck _rotateCheck;
        private static KeyboardInput _keyListener;
        private static Prediction _predict;
        private static GameSettings _gameSettings;
        private static Gui _gui;
        private static GuiSettings _guiSettings;
        private static GuiMultiplayer _guiMultiplayer;
        private static MultiplayerHandler _multiplayerHandler;
        private static Packet _packet;
        public static Texture2D CurrentLevelUpImage = Globals.levelUpTextures[0];
        public static bool InMultiplayer = false;
        public static bool AllPlayersConnected = false;
        
        public Instance()
        {
            _gameGlobal = new GameGlobal();
            _rand = new Random();
            _sfx = new Sfx();
            _holdShape = new HoldShape();
            _predict = new Prediction();
            _movement = new Movement();
            _rowCheck = new RowCheck();
            _scoreHandler = new ScoreHandler();
            _nextShape = new NextShape();
            _rotate = new Rotate();
            _rotateCheck = new RotateCheck();
            _ply = new TetrisPlayer();
            _keyListener = new KeyboardInput();
            _gameSettings = new GameSettings();
            _gui = new Gui();
            _guiSettings = new GuiSettings();
            _guiMultiplayer = new GuiMultiplayer();
            _packet = new Packet();
            _multiplayerHandler = new MultiplayerHandler();
        }

        public static Packet GetPacket()
        {
            return _packet;
        }

        public static GuiMultiplayer GetGuiMultiplayer()
        {
            return _guiMultiplayer;
        }
        
        public static bool IsMultiplayerMode()
        {
            return InMultiplayer;
        }

        public static bool IsPlayerConnected()
        {
            return AllPlayersConnected;
        }
        
        public static MultiplayerHandler GetMultiplayerHandler()
        {
            return _multiplayerHandler;
        }

        public static GameGlobal GetGame()
        {
            return _gameGlobal;
        }
        
        public static TetrisPlayer GetPlayer()
        {
            return _ply;
        }

        public static Gui GetGui()
        {
            return _gui;
        }

        public static GuiSettings GetGuiSettings()
        {
            return _guiSettings;
        }
        
        public static HoldShape GetHoldShape()
        {
            return _holdShape;
        }

        public static GameSettings GetGameSettings()
        {
            return _gameSettings;
        }
        
        public static RowCheck ResetRow()
        {
            return _rowCheck = new RowCheck();
        }
        
        public static Random GetRandom()
        {
            return _rand;
        }
        
        public static Sfx GetSound()
        {
            return _sfx;
        }

        public static Prediction GetPredict()
        {
            return _predict;
        }

        public static Movement GetMovement()
        {
            return _movement;
        }

        public static NextShape GetNextShape()
        {
            return _nextShape;
        }
        
        public static ScoreHandler GetScoreHandler()
        {
            return _scoreHandler;
        }
        
        public static RowCheck GetRowCheck()
        {
            return _rowCheck;
        }
        
        public static Rotate GetRotate()
        {
            return _rotate;
        }

        public static RotateCheck GetRotateCheck()
        {
            return _rotateCheck;
        }

        public static KeyboardInput GetKeyListener()
        {
            return _keyListener;
        }
    }
}