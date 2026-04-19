using UnityEngine;

//Developer: Bailey Escritor
//Derived class for summon cards
public class BaseSummonCard : BaseCard
{
    //public int value;
    public Summons summon;
    public ScriptableUnit summonData;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        summon = card.summon;
        //DeckManager.instance.gets
    }

    public override void SelectCard()
    {
        base.SelectCard();

        //Highlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            if (!(player.GetComponent<HandManager>().actionPoints < cost))
            {
                player.canSummon = true;
            }
            player.dmg = value;
            player.moveRange = range;
            Tile currentTile = player.OccupiedTile;

            foreach (Tile t in player.GetTilesInMoveRange())
            {
                t.ShowHighlight(true, Tile.targetableColor);
            }
            currentTile.ShowHighlight(false, Tile.nonwalkableColor);
            UnitManager.Instance.targeting = true;
        }
    }

    public override void DeselectCard()
    {
        base.DeselectCard();
        
        //Unhighlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;
        UnitManager.Instance.targeting = false;

        if(player != null)
        {
            player.canSummon = false;
            foreach (Tile t in player.GetTilesInMoveRange())
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            player.moveRange = 0;
        }
    }

    public BasePlayer SummonUnit(Tile spawnTile)
    {
        ScriptableUnit summonToSpawn = DeckManager.instance.GetSummonByName(summon.ToString());
        foreach (BaseUnit unit in UnitManager.Instance.playersSpawned)
        {
            if (unit.name == summonToSpawn.name)
            {
                unit.absorb = 0;
                unit.defiant = false;
                unit.takeDamage(999999, false, false);
                break;
            }
        }
        BasePlayer spawnedSummon = Instantiate((BasePlayer)summonToSpawn.UnitPrefab, spawnTile.transform.position, Quaternion.identity);
        spawnedSummon.name = $"{summon}";
        spawnTile.setUnit(spawnedSummon);
        spawnedSummon.summoned = true;
        spawnedSummon.lifetime = value + 1;
        UnitManager.Instance.playersSpawned.Add(spawnedSummon);
        UnitManager.Instance.playerUnitCount++;
        //spawnedSummon.GetComponent<HandManager>().NextTurn();
        return spawnedSummon;
    }


}
