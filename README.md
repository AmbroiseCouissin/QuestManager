# QuestManager

QuestManager is a quest / milestones simulator coded in C#, ASP.NET Core.

To try the service, run the web service, and consume the following api:

- Url: /api/progress
- Method: POST
- URL Parameters: None
- Data Parameters:
```
{
 "PlayerId": [string],
 "PlayerLevel": [number],
 "ChipAmountBet": [number]
}
```

**CURL:**
```
curl -X POST --header 'Content-Type: application/json' --header 'Accept: application/json' -d '{ \ 
   "PlayerId": [string], \ 
   "PlayerLevel": [number], \ 
   "ChipAmountBet": [number], 
 }' 'http://localhost:50930/api/progress'
```

**POWERSHELL:**
```
Invoke-Restmethod -Method Post "http://localhost:50930/api/progress" -ContentType "application/json" -Body "{'PlayerId': [string], 'PlayerLevel': [number], 'ChipAmountBet': [number]}"
```

## REPOSITORIES

The quest / milestones repository is in a Json configuration file:
```
'.\QuestManager.Quests.Repositories.Json\QuestConfiguration.json':
```
```
{
  "RateFromBet": 0.1,
  "LevelBonusRate": 0.2,
  "TotalPointNeededToComplete": 1000,
  "Milestones": [
    {
      "PointsNeededToComplete": 250,
      "MilestoneIndex": 0,
      "ChipAwarded": 100
    },
  ]
}
```

The player scoring repository is in a local MSSQL database using EF Core as an ORM. 
The player's scoring db schema is very simple:
```
CREATE TABLE [dbo].[Scorings] (
    [PlayerId]                       NVARCHAR (450)  NOT NULL,  
    [CompletedMilestoneIndices]      NVARCHAR (MAX)  NULL,  
    [Score]                          DECIMAL (18, 2) NOT NULL,    
    CONSTRAINT [PK_Scorings]         PRIMARY KEY CLUSTERED ([PlayerId] ASC)    
); 
```

This local database will be automatically created on startup of the web service:
```
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuestManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

The solution is equipped with a C# SDK, a xunit integration test consuming the sdk and a xunit unit test.

## SEQUENCE

To put it simply, this is what the service does:
On startup:
- deserialize the json file and load it as a quest configuration object into a dependency injected singleton.
- Set the dependency injection of the db context and put it (still DI) in a transient service.
- Check that the database is here. If not, create it automatically.

When consuming the api:
- Defensive code / guard clause
- Get the quest configuration (via DI)
- Get the current player's scoring (current score + completed milestones)
- Check if, after adding '(ChipAmountBet * RateFromBet) + (PlayerLevel * LevelBonusRate)' to the current score, the player achieves milestones.
- Upsert the new player's scoring (new score + completed milestones)
- Send back the correct reponse



