using System;
using System.Collections.Generic;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPlayground 
    {
        /// <summary>
        /// Callbacks count
        /// </summary>
        int CallbacksCount { get; }

        /// <summary>
        /// Game item prefab path
        /// </summary>
        String ItemPrefabName { get; }

        /// <summary>
        /// Game item background texture path
        /// </summary>
        String ItemBackgroundTextureName { get; }

        /// <summary>
        /// 
        /// </summary>
        IGameSettingsHelper Preferenses { get; }

        /// <summary>
        /// Disabled field reference
        /// </summary>
        System.Object DisabledItem { get; }

        /// <summary>
        /// Field size
        /// </summary>
        int FieldSize { get; }

        /// <summary>
        /// Timer multiple
        /// </summary>
        float MoveTimerMultiple { get; }

        /// <summary>
        /// Update time counter
        /// </summary>
        void UpdateTime();

        /// <summary>
        /// Can DisabledItem be swaped with simple Item
        /// </summary>
        bool isDisabledItemActive { get; }

        /// <summary>
        /// Savedata of playground
        /// </summary>
        IPlaygroundSavedata SavedataObject { get; }
        
        /// <summary>
        /// Distance from static GameItem position to position for exchange
        /// </summary>
        float DeltaToExchange { get; }

        /// <summary>
        /// Game over flag
        /// </summary>
        bool IsGameOver { get; set; }

        /// <summary>
        /// Checks if field is mixing
        /// </summary>
        bool IsMixing { get;}

        /// <summary>
        /// Distance from static GameItem position to position for moving
        /// </summary>
        float DeltaToMove { get; }

        /// <summary>
        /// Current game score
        /// </summary>
        int CurrentScore { get; }

        /// <summary>
        /// Absolute size of game item
        /// </summary>
        float GameItemSize { get; }

        /// <summary>
        /// Position of first game item
        /// </summary>
        RealPoint InitialGameItemPosition { get; }

        /// <summary>
        /// Maximum level of initial elements
        /// </summary>
        GameItemType MaxInitialElementType { get; set; }

        /// <summary>
        /// Playground items collection
        /// </summary>
        System.Object[][] Items { get; set; }

        /// <summary>
        /// Available directions for GameItem
        /// </summary>
        Dictionary<MoveDirections,Vector2> AvailableMoveDirections { get; }

        /// <summary>
        /// Gets field cell coordinates
        /// </summary>
        Vector3 GetCellCoordinates(int col, int row);

        /// <summary>
        /// Get all exist lines on playground
        /// </summary>
        /// <returns>Returns collection of Line objects</returns>
        IList<Line> GetAllLines();

        /// <summary>
        /// Reset dynamic fields of playground
        /// </summary>
        void ResetPlayground();

        /// <summary>
        /// Check the element for line
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="orientation">Direction of check</param>
        /// <returns>Returns count of elements line</returns>
        int CheckForLine(int x, int y, LineOrientation orientation, bool includeMovingItemsInLine = true);

        /// <summary>
        /// Clearing chains and shifting game items
        /// </summary>
        int ClearChains();

        /// <summary>
        /// Generating menu when game is over
        /// </summary>
        void GenerateGameOverMenu(bool isWinning = false);

        /// <summary>
        /// Checking if it is available to move item to selected direction
        /// </summary>
        /// <param name="col">Items X coordinate</param>
        /// <param name="row">Items Y coordinate</param>
        /// <param name="mdir">Moving direction</param>
        bool IsItemMovingAvailable(int col, int row, MoveDirections mdir);

        /// <summary>
        /// Moving item in conformity with playground rules
        /// </summary>
        /// <param name="x1">Source X coordinate</param>
        /// <param name="y1">Source Y coordinate</param>
        /// <param name="x2">Destination X coordinate</param>
        /// <param name="y2">Destination Y coordinate</param>
        bool TryMakeMove(int x1, int y1, int x2, int y2);

        /// <summary>
        /// Generate new items on field
        /// </summary>
        /// <param name="completeCurrent">Possible lines check is off</param>
        /// <param name="mixCurrent">Mix current field instead of generating new</param>
        void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false, LabelAnimationFinishedDelegate callback = null);

        /// <summary>
        /// Generate new visual item of random type
        /// </summary>
        /// <param name="i">Item column</param>
        /// <param name="j">Item row</param>
        /// <param name="deniedTypes">Types of items, deny to be created</param>
        /// <param name="generateOnY">?</param>
        /// <returns></returns>
        GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null);

        /// <summary>
        /// Exchange two visual items
        /// </summary>
        /// <param name="x1">First exchange item X coordinate</param>
        /// <param name="y1">First exchange item Y coordinate</param>
        /// <param name="x2">Second exchange item X coordinate</param>
        /// <param name="y2">Second exchange item Y coordinate</param>
        /// <param name="speed">Exchange speed</param>
        /// <param name="isReverse">Is exchange must be revert</param>
        /// <param name="exchangeCallback">Exchange callback method</param>
        bool GameItemsExchange(int x1,  int y1,  int x2,  int y2, float speed, bool isReverse, MovingFinishedDelegate exchangeCallback = null);

        /// <summary>
        /// Shift down items on field to empty places
        /// </summary>
        void Drop();

        /// <summary>
        /// Inspect field for available move
        /// </summary>
        /// <returns>Is move exist</returns>
        bool CheckForPossibleMoves();

        /// <summary>
        /// Just revert moved item without effect
        /// </summary>
        /// <param name="col">Item coll coordinate</param>
        /// <param name="row">Item row coordinate</param>
        void RevertMovedItem(int col, int row, MovingFinishedDelegate callback = null);
    }
}

