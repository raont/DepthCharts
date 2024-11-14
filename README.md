# DepthCharts


## Running the Application
### Pre-requisites:
Visual Studio with Asp.Net Core 8<br>
Sqlitestudio or any Sqlite db manager (Not required but helpful if you want to have a look at the db)<br>
Windows pc<br>


### How to run locally
Checkout code from https://github.com/raont/DepthCharts.git <br>
This application is a asp.net core 8 web api so you can launch it using latest visual studio by clicking on PlayerDepths.sln <br>
After launch go to WebApi\appsettings.json and change WebApiDatabase setting. Add absolute path to the PlayerDepths db. <br>
PlayerDepths.db is located under Infrastructure\PlayerDepths.db<br>
Then build it. <br>
If you build in debug mode it should launch Swagger in a browser <br>
else go to WebApi\bin\Debug\net8.0 and click WebApi.exe, then launch swagger url http://localhost:5269/swagger/index.html in a browser.

### How to use application
If you are familiar with Swagger it is fairly easy to use. I have already populated all the data so mere invoking the endpoints should work.<br>
I have setup NFL game with game Id 1 so use this id for testing the depth chart.<br>
There is endpoint /Game/FullDepthChart/{Game} for displaying the full chart. I have already added nfl game so just enter NFL in the Game field and execute.<br>

**Notes:**<br>
Wherever using Position consider the count starts from please start from 0 (Highest position). Example a player at position 2 will be at the 3rd row with 2 players in front.

### Extendibility
This application is designed to support depth charts for multiple games. Just add a new game using end point /Game/create-game/{name}, it will return game id. Use this game id to add new depth positions and player to depth position.<br>
We have NFL setup already with game Id 1.<br>
You can add a new Depth positions Lane anytime by adding a new player with new Depth using endpoint /PlayerDepths/{GameId}/add-player-depth<br>


