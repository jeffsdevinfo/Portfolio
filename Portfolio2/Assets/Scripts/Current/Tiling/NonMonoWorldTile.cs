// Copyright (c) 2022 Jeff Simon
// Distributed under the MIT/X11 software license, see the accompanying
// file license.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;

public class NonMonoWorldTile
{
    public int DatabaseRecordId = -1;
    public int DatabaseTileIndex = 0;
    public bool OverwriteExistingDBTile = false;
    public float LoadDistance = 0;    
    public List<NonMonoDBGameObject> worldDBGameObjects = new List<NonMonoDBGameObject>();
    public NonMonoDBTerrain worldDBTerrain = new NonMonoDBTerrain();
}
